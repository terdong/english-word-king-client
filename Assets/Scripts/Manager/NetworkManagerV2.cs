using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TeamGehem;
using TeamGehem.DataModels.Protocols;
using UnityEngine;
using WebSocketSharp;

public class NetworkManagerV2 : MonoBehaviour
{
    public static NetworkManagerV2 Instance { get; private set; }
    private static readonly BindingFlags Binding_Flags_ = BindingFlags.Instance | BindingFlags.NonPublic;

    private Queue<Action> execute_on_main_thread_queue;
    private HashSet<MonoBehaviour> onmessage_listener_set_;
    private IDictionary<UrlPath, string> server_address_dic_;
    private IDictionary<UrlPath, WebSocket> socket_dic_;

    private SocketProvider socket_provider_;

    public bool IsConnectedServer(UrlPath url_path)
    {
        return socket_dic_[url_path].IsAlive;
    }

    public void ConnectServer(UrlPath url_path)
    {
        ConnectServer(url_path, null);
    }

    public void ConnectServer(UrlPath url_path, Nullable<EventHandlerGroup> event_handler_group)
    {
        if (server_address_dic_.ContainsKey(url_path))
        {
            UrlMaker url_maker = UrlMaker.CreateUrlMaker(server_address_dic_[url_path], url_path);
            ConnectServer(url_maker, event_handler_group);
        }
        else
        {
            Debug.LogError(string.Format("server_address_.ContainsKey({0}) is invalid.", url_path));
        }
    }

    public void ConnectServer(UrlMaker url_maker, Nullable<EventHandlerGroup> event_handler_group)
    {
        WebSocket ws = socket_provider_.CreateWebSocket(url_maker.ToString());

        ws.OnMessage += CallbackOnMessageEvent;

        if(event_handler_group.HasValue)
        {
            event_handler_group.Value.AddEventHandler(ws);
        }

        if (url_maker.Path.Equals(UrlPath.login))
        {
            ws.SetCredentials("guest", "password", false);
        }

        socket_dic_.Add(url_maker.Path, ws);

        ws.ConnectAsync();

        Toast.ShowToast(string.Format(StringFormat.Server_Connecting));
    }

    public void DisConnectServer(UrlPath url_path)
    {
        if(socket_dic_.ContainsKey(url_path))
        {
            socket_dic_[url_path].CloseAsync();
            socket_dic_.Remove(url_path);
        }
    }

    public void AddServerAddress(UrlPath url_path, string address)
    {
        server_address_dic_[url_path] = address;
    }

    public void AddOnMessageListener(MonoBehaviour listener_object)
    {
        onmessage_listener_set_.Add(listener_object);
    }

    public bool RemoveOnMessageListener(MonoBehaviour listener_object)
    {
        bool result = false;
        result = onmessage_listener_set_.Remove(listener_object);
        if (!result) { Debug.LogError("Can't Remove this gameObject."); }
        return result;
    }

    public void InvokeAction(Action action)
    {
        execute_on_main_thread_queue.Enqueue(action);
    }

    private void Initialization()
    {
        execute_on_main_thread_queue = new Queue<Action>();
        onmessage_listener_set_ = new HashSet<MonoBehaviour>();
        server_address_dic_ = new Dictionary<UrlPath, string>();
        socket_dic_ = new Dictionary<UrlPath, WebSocket>();

        socket_provider_ = new SocketProvider();

        server_address_dic_[UrlPath.login] = GameInfo.Login_Url;
    }

    private void Release()
    {
        execute_on_main_thread_queue.Clear();
        onmessage_listener_set_.Clear();
        server_address_dic_.Clear();
        socket_dic_.Clear();

        execute_on_main_thread_queue = null;
        onmessage_listener_set_ = null;
        server_address_dic_ = null;
        socket_dic_ = null;

        socket_provider_ = null;
    }

    private void CallbackOnMessageEvent(object sender, MessageEventArgs e)
    {
        #region legacy_ when 'Opcode.Text' call InvokeAction
        // if (e.Type == Opcode.TEXT)
        //{
        //    Debug.Log(string.Format("ws.OnMessage :: e.Data = {0}", e.Data));

            //InvokeAction(() =>
            //{
            //    object[] stack_array = onmessage_listener_set_.ToArray<object>();
            //    for (int i = 0; i < stack_array.Length; ++i)
            //    {
            //        CallInvokeMethod<string>(stack_array[i], DispatchText_Method_Name_, e.Data);
            //    }
            //});
        //}
        #endregion
        if (e.Type == Opcode.BINARY)
        {
            IEwkProtocol ewk_protocol = EwkProtoSerilazer.DeserializeForProtobuf(e.RawData);
            string dispatch_key = ewk_protocol.Protocol_Enum.ToString();
            string method_name = string.Format("Dispatch{0}", dispatch_key);
            InvokeAction(() =>
            {
                MonoBehaviour[] stack_array = onmessage_listener_set_.ToArray<MonoBehaviour>();
                for (int i = 0; i < stack_array.Length; ++i)
                {
                    CallInvokeMethod<IEwkProtocol>(stack_array[i], method_name, ewk_protocol);
                }
            });
        }
    }

    private void CallInvokeMethod<T>(MonoBehaviour obj, string method_name, T t)
    {
        Type type = obj.GetType();
        MethodInfo mothod_info = type.GetMethod(method_name, Binding_Flags_);
        
        //TODO: 다운 캐스팅 코드는 추후 MonoBehaviour 테이블 관리로 바꿔서 자원낭비 줄이자.[by Terdong : 2014-12-08]
        if (mothod_info != null && obj.gameObject.activeSelf)
        {
            mothod_info.Invoke(obj, new object[] { t });
        }
    }

    void Awake()
    {
        Instance = this;

        Initialization();
    }
    void Update()
    {
        while (execute_on_main_thread_queue.Count > 0)
        {
            execute_on_main_thread_queue.Dequeue().Invoke();
        }
    }
    void OnDestroy()
    {
        Release();

        if (Instance != null) { Instance = null; }
    }
}
