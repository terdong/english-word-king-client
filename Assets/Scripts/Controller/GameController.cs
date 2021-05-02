//#define LOG_TRACE_INFO
//#define LOG_EXTRA_INFO

using UnityEngine;
using System.Collections;
using TeamGehem;
using WebSocketSharp;
using TeamGehem.DataModels.Protocols;
using System;

//------------------------------------------------------------------------------
// class definition
//------------------------------------------------------------------------------
public class GameController : MonoBehaviour
{
    private static readonly string Right_Button_Text_Ready = "준비";
    private static readonly string Right_Button_Text_Play = "게임중";
    private static readonly string Right_Button_Text_Ready_Cancel = "취소";

    public enum GameMode{
        None = 0,
        Quiz,
        Fight,
        Wrong_Answer,
        Mode_Count
    }
	public static GameController gameController;

    CommonUI common_ui_;
    NetworkManager network_manager_;

    Fight fight_;
    Quiz quiz_;
    ReadyRoom ready_room_;
    GameResult game_result_;

	//--------------------------------------------------------------------------
	// public static methods
	//--------------------------------------------------------------------------

    public static void ChangeGameMode(GameMode game_mode)
    {
        switch(game_mode)
        {
            case GameMode.Quiz:

                break;
            case GameMode.Fight:

                break;
            case GameMode.Wrong_Answer:

                break;
        }
    }

	//--------------------------------------------------------------------------
	// protected mono methods
	//--------------------------------------------------------------------------
	protected void Awake()
	{
		gameController = this;

        common_ui_ = MainController.GetCommonUI();
        common_ui_.SetTextRightButtonLabel(Right_Button_Text_Ready);
        common_ui_.Right_Button_Delegate = RequestGameReady;

        network_manager_ = NetworkManager.Instance;

        fight_      = transform.FindChild( "00_Fight" ).GetComponent<Fight>();
        ready_room_ = transform.FindChild( "01_ReadyRoom" ).GetComponent<ReadyRoom>();
        quiz_       = transform.FindChild( "02_Quiz" ).GetComponent<Quiz>();

        game_result_ = transform.FindChild( "03_Result" ).GetComponent<GameResult>();
	}

	protected void OnDestroy()
	{
		if(gameController != null)
		{
			gameController = null;
		}
	}

    protected void OnGUI()
    {
        //if (GUI.Button(new Rect(1,470,80,20), "Disconnect"))
        //{
        //    NetworkManager.Instance.closeSocket();
        //}

        //if (GUI.Button(new Rect(110, 470, 80, 20), "Connect"))
        //{
        //    if (!NetworkManager.Instance.SocketReady)
        //    {
        //        NetworkManager.Instance.setupSocket();
        //    }
        //}

        //if (GUI.Button(new Rect(210, 470, 80, 20), "GetQuiz"))
        //{
        //    string req = string.Format("{0}", NetworkManager.Protocol.req_quiz_question.GetHashCode());
        //    NetworkManager.Instance.writeSocket(req);
        //}
    }

	//--------------------------------------------------------------------------
	// private methods
	//--------------------------------------------------------------------------

    void RequestGameReady()
    {
        // 서버로 게임 준비 요청 전송.[Terdong : 2014-08-04]
        Debug.Log("RequestGameReady");

        SocketPackage sp;
        bool isSp = network_manager_.GetSocketPackage(UrlPath.game.ToString(), out sp);
        if (!isSp)
        {
            EventHandlerGroup eg = new EventHandlerGroup(OpenGameReady, ReceiveGameReady, CloseGameReady);
            UrlMaker url_path = UrlMaker.CreateUrlMaker("sample", UrlPath.game);
            WebSocket ws = network_manager_.ResisterSocket(url_path, eg);
            //network_manager_.AddOnMessageListener(

            UserInfo user_info = DataManager.User_Info;
            ws.SetCredentials(user_info.Guest_Id.ToString(), user_info.First_Session_Id, false);
            ws.Connect();
        }
    }

    void OpenGameReady(object sender, EventArgs e)
    {
        //SocketPackage sp;
        //bool isSocketPackage = network_manager_.GetSocketPackage(UrlPath.game.ToString(), out sp);

        //Person person = new Person();
        //person.id = 123;
        //person.name = "test";
        //MemoryStream stream = new MemoryStream();
        //ProtoBuf.Serializer.Serialize<Person>(stream, person);
        //sp.Send(stream.ToArray());

        NetworkManager.Instance.EnqueueAction(() =>
            {
                ResponseGameReady(true);
            }
        );
    }

    void DispatchQuizInfo( IEwkProtocol ewk )
    {
        QuizInfo quiz_info = ewk.GetData<QuizInfo>();
        Debug.Log( quiz_info );
        quiz_.gameObject.SetActive( true );
        StartCoroutine( quiz_.SetContents( quiz_info ) );
    }

    void DispatchFightInfo( IEwkProtocol ewk )
    {
        FightInfo fight_info = ewk.GetData<FightInfo>();
        fight_.UpdateFightInfo( fight_info );
    }

    void DispatchAvatarInfo( IEwkProtocol ewk )
    {
        AvatarInfo avatar_info = ewk.GetData<AvatarInfo>();
        fight_.UpdateAvatarInfo( avatar_info );
    }

    void DispatchRes_Game_Ready_Ok( IEwkProtocol ewk )
    {
        ready_room_.Chat_Socket_Package_.Send(
            EwkProtoFactory.CreateEwkProtocol(ProtocolEnum.Req_Notify_Game_Ready).GetBytes
        );
    }

    void DispatchRes_Game_Change_Game_Mode( IEwkProtocol ewk )
    {
         ready_room_.SetActiveChatWindow(false);
        quiz_.gameObject.SetActive(true);
        common_ui_.SetTextRightButtonLabel(Right_Button_Text_Play);
    }

    void DispatchRes_Game_Change_Result_Mode( IEwkProtocol ewk )
    {
        game_result_.gameObject.SetActive( true );
        quiz_.gameObject.SetActive( false );
        ready_room_.SetActiveChatWindow( true );
    }

    void DispatchAvatar_Direction_Left( IEwkProtocol ewk )
    {
        fight_.My_Avatar_Direction_ = ewk.Protocol_Enum;
    }

    void DispatchText(string text)
    {
        ready_room_.PrintChatInput( text );
    }

    void ReceiveGameReady(object sender, MessageEventArgs e)
    {
        if (e.Type == Opcode.BINARY)
        {
            byte[] bytes = e.RawData;
            try
            {
                IEwkProtocol ewk_protocol = EwkProtoSerilazer.DeserializeForProtobuf(bytes);
                if(ewk_protocol.Protocol_Enum == ProtocolEnum.QuizInfo)
                {
                    QuizInfo quiz_info = ewk_protocol.GetData<QuizInfo>();
                    NetworkManager.Instance.EnqueueAction(() =>
                        {
                            Debug.Log(quiz_info);
                            quiz_.gameObject.SetActive( true );
                            StartCoroutine(quiz_.SetContents(quiz_info));
                            //quiz_.setContents(quiz_info);
                        }
                    );

                }
                else if (ewk_protocol.Protocol_Enum == ProtocolEnum.FightInfo)
                {
                    FightInfo fight_info = ewk_protocol.GetData<FightInfo>();
                    NetworkManager.Instance.EnqueueAction(() =>
                        {
                            //Debug.Log(fight_info);
                            fight_.UpdateFightInfo( fight_info );
                        }
                    );
                }
                else if (ewk_protocol.Protocol_Enum == ProtocolEnum.AvatarInfo)
                {
                    AvatarInfo avatar_info = ewk_protocol.GetData<AvatarInfo>();
                    NetworkManager.Instance.EnqueueAction(() =>
                        {
                            //Debug.Log(avatar_info);
                            fight_.UpdateAvatarInfo( avatar_info );
                        }
                    );
                }
                else if (ewk_protocol.Protocol_Enum == ProtocolEnum.Res_Game_Ready_Ok)
                {
                    NetworkManager.Instance.EnqueueAction( () =>
                        {
                            ready_room_.Chat_Socket_Package_.Send(
                                EwkProtoFactory.CreateEwkProtocol(ProtocolEnum.Req_Notify_Game_Ready).GetBytes
                            );
                        }
                    );
                }
                else if (ewk_protocol.Protocol_Enum == ProtocolEnum.Res_Game_Change_Game_Mode)
                {
                    NetworkManager.Instance.EnqueueAction(() =>
                        {
                            ready_room_.SetActiveChatWindow(false);
                            quiz_.gameObject.SetActive(true);
                            common_ui_.SetTextRightButtonLabel(Right_Button_Text_Play);
                        }
                    );
                }
                else if (ewk_protocol.Protocol_Enum == ProtocolEnum.Res_Game_Change_Result_Mode)
                {
                    NetworkManager.Instance.EnqueueAction( () =>
                        {
                            game_result_.gameObject.SetActive( true );
                            quiz_.gameObject.SetActive( false );
                            ready_room_.SetActiveChatWindow( true );
                        }
                    );
                }
                else if ( ewk_protocol.Protocol_Enum == ProtocolEnum.Avatar_Direction_Left || ewk_protocol.Protocol_Enum == ProtocolEnum.Avatar_Direction_Right )
                {
                    NetworkManager.Instance.EnqueueAction( () =>
                        {
                            fight_.My_Avatar_Direction_ = ewk_protocol.Protocol_Enum;
                        }
                    );
                }

                Debug.Log("ReceiveGameReady::Protocol_Enum = " + ewk_protocol.Protocol_Enum.ToString());
            }
            catch (Exception ee)
            {
                Debug.Log(ee.Message);
            }
        }
        else
        {
            NetworkManager.Instance.EnqueueAction(() =>
                {
                    ready_room_.PrintChatInput(e.Data);
                }
            );
        }
    }

    void CloseGameReady(object sender, CloseEventArgs e)
    {
        NetworkManager.Instance.EnqueueAction(() =>
        {
            ResponseGameReady(false);
        }
        );
    }
    void ResponseGameReady(bool isDisableRightButton)
    {
        common_ui_.SetDisableRightButton(isDisableRightButton);
    }
}
