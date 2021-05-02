using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamGehem;
using WebSocketSharp;
using TeamGehem.DataModels.Protocols;
using System.Reflection;
using WebSocketSharp.Net;
using Assets.Scripts.DataModels;

public class NetworkManager : MonoBehaviour
{
    //private static readonly string URL = "wss://localhost:10012/{0}";
    private static readonly string URL = "ws://192.168.0.2:10012/{0}";
    //private static readonly string URL = "ws://localhost:10012/{0}";
    //private static readonly string URL = "ws://128.199.246.163:10012/{0}";
    //private static readonly string URL = "ws://10.0.0.112:10012/{0}";
    //private static readonly string URL = "ws://112.146.169.59:10012/{0}";

    private static readonly string DispatchText_Method_Name_ = "DispatchText";
    private static readonly Queue<Action> Execute_On_MainThread_ = new Queue<Action>();
    private static readonly BindingFlags Binding_Flags_ = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    //   private IDictionary<string, Action<IEwkProtocol>> dispatch_listener_dic_;

    private HashSet<object> onmessage_listener_set_;

    private int socket_count_ = 0;

    public static NetworkManager Instance { get; private set; }

    public IDictionary<string, SocketPackage> socket_pocket_ { get; private set; }

    public static bool IsAbleToSendData()
    {
        return NetworkManager.Instance && DataManager.IsLogin();
    }

    public static void AddOnMessageListener(object listener_object)
    {
        if (Instance)
        {
            Instance.onmessage_listener_set_.Add(listener_object);
        }
    }
    public static bool RemoveOnMessageListener(object listener_object)
    {
        bool result = false;
        if (Instance)
        {
            result = Instance.onmessage_listener_set_.Remove(listener_object);
            if (!result) { Debug.LogError("Can't Remove this gameObject."); }
        }
        return result;
    }

    public static void DisconnectSocket(UrlPath path)
    {
        if (Instance) { Instance.DisconnectSocket(path.ToString()); }
    }

    //public void AddOnMessageListener( string key, Action<IEwkProtocol> value )
    //{
    //    dispatch_listener_dic_[key] += value;
    //}

    public bool GetSocketPackage(string key, out SocketPackage socket_package)
    {
        return socket_pocket_.TryGetValue(key, out socket_package);
    }

    public bool GetSocketPackage(UrlPath url_path, out SocketPackage socket_package)
    {
        return socket_pocket_.TryGetValue(url_path.ToString(), out socket_package);
    }

    public bool IsAliveSocket(UrlPath path)
    {
        SocketPackage socket_package = null;
        if (GetSocketPackage(path, out socket_package))
        {
            return socket_package.Web_Socket.IsAlive;
        }
        else
        {
            return false;
        }
    }

    public bool IsSocketPocketContainsKey(string key) { return socket_pocket_.ContainsKey(key); }

    public WebSocket ConnectSocket(UrlPath url_path)
    {
        return null;
        //return ConnectSocket(url_path, null);
    }

    public WebSocket ConnectSocket(UrlPath url_path, EventHandlerGroup eg)
    {
        UserInfo user_info = DataManager.User_Info;

        //UrlMaker url_maker = new UrlMaker(url_path);
        UrlMaker url_maker = UrlMaker.CreateUrlMaker("sample", url_path);

        Debug.Log("user_info.Game_Url_Path = " + user_info.Game_Url_Path);
        WebSocket ws = new WebSocket(user_info.Game_Url_Path, url_maker.GetProtocols());

        SetWebSocket(ws, url_maker, eg);

        ws.SetCredentials(user_info.Guest_Id.ToString(), user_info.First_Session_Id, false);

        ws.ConnectAsync();
        return ws;
    }

    //public WebSocket ResisterSocket( UrlMaker url_maker, EventHandlerGroup event_handler_group )
    //{
    //    return ResisterSocket( url_maker, event_handler_group, url_maker.Path.ToString() );
    //}

    public WebSocket ResisterSocket(UrlMaker path, EventHandlerGroup event_handler_group)
    {
        int socket_index = socket_count_++;

        string str_path = string.Format(URL, path.ToString());
        Debug.Log("str_path = " + str_path);
        WebSocket ws = new WebSocket(str_path, path.GetProtocols());

        SetWebSocket(ws, path, event_handler_group);

        return ws;
    }

    /**
        @brief      소켓 종료시 이 메소드 호출.  
        @author     terdong
        @date       2014/12/19
        @param      string key
        @return     void
        @remarks
    */
    public void DisconnectSocket(string key)
    {
        EnqueueAction(() =>
            {
                if (socket_pocket_.ContainsKey(key))
                {
                    SocketPackage sb = socket_pocket_[key];
                    socket_pocket_.Remove(key);
                    sb.DisconnectAsyncSocket();
                }
            }
        );
    }

    public void EnqueueAction(Action action)
    {
        Execute_On_MainThread_.Enqueue(action);
    }

    public void AddEventHandler(WebSocket ws, EventHandlerGroup event_handler_group)
    {
        if (event_handler_group.On_Open != null) ws.OnOpen += event_handler_group.On_Open;
        if (event_handler_group.On_Message != null) ws.OnMessage += event_handler_group.On_Message;
        if (event_handler_group.On_Close != null) ws.OnClose += event_handler_group.On_Close;
        if (event_handler_group.On_Error != null) ws.OnError += event_handler_group.On_Error;
    }

    void Awake()
    {
        Instance = this;
        socket_pocket_ = new Dictionary<string, SocketPackage>();
        //        dispatch_listener_dic_ = new Dictionary<string, Action<IEwkProtocol>>();

        onmessage_listener_set_ = new HashSet<object>();
    }

    void Update()
    {
        while (Execute_On_MainThread_.Count > 0)
        {
            Execute_On_MainThread_.Dequeue().Invoke();
        }
    }

    void OnDestroy()
    {
        if (socket_pocket_.Count > 0)
        {
            foreach (SocketPackage sp in socket_pocket_.Values)
            {
                sp.DisconnectSocket();
            }
            socket_pocket_.Clear();
        }
        if (Instance != null) { Instance = null; }
    }

    private void CallInvokeMethod<T>(object obj, string method_name, T t)
    {
        Type type = obj.GetType();
        MethodInfo mothod_info = type.GetMethod(method_name, Binding_Flags_);

        //TODO: 다운 캐스팅 코드는 추후 MonoBehaviour 테이블 관리로 바꿔서 자원낭비 줄이자.[by Terdong : 2014-12-08]
        if (mothod_info != null && (obj as MonoBehaviour).gameObject.activeSelf)
        {
            mothod_info.Invoke(obj, new object[] { t });
        }
    }

    private void SetWebSocket(WebSocket ws, UrlMaker path, EventHandlerGroup event_handler_group)
    {
        string key = path.Path.ToString();

        //TODO: 이거 적용하면 왜 exception 에러나는지 모르겠다. [Terdong : 2014-09-15]
        ws.Compression = CompressionMethod.DEFLATE;

        ws.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
        {
            Debug.Log(String.Format("\n{0}\n{1}", certificate.Issuer, certificate.Subject));
            return true; // If the server cert is valid
        };

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("ws.OnOpen :: Connection Success!, isOpen = " + (ws.ReadyState == WebSocketState.OPEN));

            SocketPackage sp_ = new SocketPackage(key, path, ws);
            socket_pocket_.Add(key, sp_);
        };
        ws.OnMessage += (sender, e) =>
        {
            //            Debug.Log(string.Format("cookie num = {0}, string = {1}", ws.Cookies.ToList<Cookie>().Count, ws.Cookies.ToString()));

            if (e.Type == Opcode.TEXT)
            {
                Debug.Log(string.Format("ws.OnMessage :: e.Data = {0}", e.Data));

                EnqueueAction(() =>
                {
                    object[] stack_array = onmessage_listener_set_.ToArray<object>();
                    for (int i = 0; i < stack_array.Length; ++i)
                    {
                        CallInvokeMethod<string>(stack_array[i], DispatchText_Method_Name_, e.Data);

                        #region legacy : Reflection 방법으로 메소드 호출.
                        //object target = stack_array[i];
                        //Type t = target.GetType();
                        //MethodInfo mothod_info = t.GetMethod( "DispatchText", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
                        //if ( mothod_info != null )
                        //{
                        //    object[] param_array = new object[1];
                        //    param_array[0] = e.Data;
                        //    mothod_info.Invoke( target, param_array );
                        //}
                        #endregion
                    }
                });

            }
            else if (e.Type == Opcode.BINARY)
            {
                IEwkProtocol ewk_protocol = EwkProtoSerilazer.DeserializeForProtobuf(e.RawData);
                string dispatch_key = ewk_protocol.Protocol_Enum.ToString();
                string method_name = string.Format("Dispatch{0}", dispatch_key);
                EnqueueAction(() =>
                {
                    object[] stack_array = onmessage_listener_set_.ToArray<object>();
                    for (int i = 0; i < stack_array.Length; ++i)
                    {
                        CallInvokeMethod<IEwkProtocol>(stack_array[i], method_name, ewk_protocol);
                    }
                });
            }

        };
        ws.OnError += (sender, e) =>
        {
            Debug.LogError(string.Format("ws.OnError :: channel = {0}, ws.IsAlive = {1}, exception = {2}", key, ws.IsAlive, e.Message));
            DisconnectSocket(key);
        };
        ws.OnClose += (sender, e) =>
        {
            if (e.WasClean)
            {
                Debug.Log(string.Format("ws.OnClose :: Disconnect Success!, channel = {0}, isClosed = {1}", key, (ws.ReadyState == WebSocketState.CLOSED)));
                DisconnectSocket(key);
            }
            else
            {
                string error_str = string.Format("ws.OnClose :: channel = {0}, e.Code = {1}, e.Reason= {2}", key, e.Code, e.Reason);
                Debug.LogError(error_str);
                EnqueueAction(() =>
                {
                    // TODO : Toast 교체. [4/8/2015 terdong]
                    //Toast.ShowToast(string.Format("네트워크 연결 에러.\n{0}", error_str), 3.0f);
                });
            }
        };

        //if (event_handler_group != null)
        //{
        //    AddEventHandler(ws, event_handler_group);
        //}
    }
}
