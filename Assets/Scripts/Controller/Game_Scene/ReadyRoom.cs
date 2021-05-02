using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using TeamGehem;
using System.IO;
using TeamGehem.DataModels.Protocols;
using System.Text;
using System.Runtime.Serialization;
//using ProtoBuf.Meta;

public class ReadyRoom : MonoBehaviour
{
    private static readonly string Socket_Key = UrlPath.chat.ToString();

    SocketPackage chat_socket_package_;
    public SocketPackage Chat_Socket_Package_ { get { return chat_socket_package_; } private set { chat_socket_package_ = value; } }

    CommonUI common_ui_;
    GehemChatInput chat_input_;
    GameObject chat_window_;

    NetworkManager network_manager_;


    public void RequestTalkMessage(string message)
    {
        Chat_Socket_Package_.Send(message);
    }

    public void SetActiveChatWindow(bool is_active)
    {
        chat_window_.SetActive(is_active);
    }

    public void PrintChatInput(string message)
    {
        chat_input_.AddChatMessageToTextList(message);
    }

    void Awake()
    {
        common_ui_ = MainController.GetCommonUI();
        chat_window_ = transform.FindChild("Chat Window").gameObject;
        chat_input_ = chat_window_.transform.FindChild("Chat Input").GetComponent<GehemChatInput>();
        network_manager_ = NetworkManager.Instance;
    }

    void Start()
    {
        //common_ui_.InitializeButtonsLabel();

        EventHandlerGroup eventhandler_group = new EventHandlerGroup(OpenChatRoom, ReceiveMessage, CloseChatRoom);
        //UrlMaker url_path = new UrlMaker(UrlPath.chat);
        UrlMaker url_path = UrlMaker.CreateUrlMaker("sample", UrlPath.chat);

        UserInfo user_info = DataManager.User_Info;
        url_path.AddParams("name", user_info.Guest_Name);
        WebSocket ws = network_manager_.ResisterSocket(url_path, eventhandler_group);
        ws.SetCredentials(user_info.Guest_Id.ToString(), user_info.First_Session_Id, false);
        ws.Connect();
    }

    void OnDisable()
    {
        if ( common_ui_ != null ) { common_ui_.InitializeButtonsLabel(); }
    }

    void OnDestroy()
    {
        //TODO: 이거 나중에 제대로 처리해주자. [by Terdong : 2014-12-08]
        //NetworkManager.DisconnectSocket(Socket_Key);
        NetworkManager.DisconnectSocket(UrlPath.game);
    }

    void OpenChatRoom(object sender, EventArgs e)
    {
        network_manager_.GetSocketPackage(Socket_Key, out chat_socket_package_);
    }

    void ReceiveMessage(object sender, MessageEventArgs e)
    {
        if (e.Type == Opcode.BINARY)
        {

        }
        else
        {
            network_manager_.EnqueueAction(() =>
            {
                chat_input_.AddChatMessageToTextList(e.Data);
            });
        }
    }

    void CloseChatRoom(object sender, CloseEventArgs e)
    {

    }
}
