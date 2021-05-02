//#define LOG_TRACE_INFO
//#define LOG_EXTRA_INFO

using UnityEngine;
using System.Collections;
using Assets.Scripts.DataModels;
using TeamGehem;
using WebSocketSharp;
using System;
using TeamGehem.DataModels.Protocols;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

//------------------------------------------------------------------------------
// class definition
//------------------------------------------------------------------------------
public class MenuController : MonoBehaviour
{
	private static MenuController menuController;

    public UIInput Input_Login_;

    bool is_login_;

    CommonUI common_ui_;
    UILabel label_login_;

	//--------------------------------------------------------------------------
	// public static methods
	//--------------------------------------------------------------------------
    public void TestMyClickFunction()
    {
        SoundManager.PlayEffect_Touch();
        MainController.SwitchScene( "Game_Scene" );
        SoundManager.PlayBGM("BGM_Kaze_ni_Naru_ar");
    }

    public void MoveToTrainingScene()
    {
        SoundManager.PlayEffect_Touch();
        //Toast.ShowToast("준비 중 입니다.");
        MainController.SwitchScene("Single_Scene");
    }

    public void MoveToDuelMenuScene()
    {
        SoundManager.PlayEffect_Touch();
        SocketPackage sp = null;
        //if (NetworkManager.Instance && NetworkManager.Instance.GetSocketPackage(UrlPath.login, out sp))
        //{
        //    sp.Send( EwkProtoFactory.CreateEwkProtocol<string>( ProtocolEnum.Req_Change_Scene, Scene_List.Duel_Menu_Scene.ToString() ).GetBytes );
        //}

        if (NetworkManager.IsAbleToSendData() && NetworkManager.Instance.GetSocketPackage(UrlPath.lobby, out sp))
        {
            sp.Send(EwkProtoFactory.CreateEwkProtocol<SceneListEnum>(ProtocolEnum.Req_Change_Scene, SceneListEnum.Duel_Menu_Scene).GetBytes);
            //TODO: 로비서버 연결. [by Terdong : 2014-12-03]
            //NetworkManager.Instance.ConnectSocket(UrlPath.lobby);
            //MainController.SwitchScene(Scene_List.Duel_Menu_Scene);
        }
        else
        {
            Toast.ShowToast("로그인 후 입장하실 수 있습니다.");
        }
    }

    public void ConnectSampleServer()
    {
        SoundManager.PlayEffect_Touch();
        // TODO : 임시로 UNITY_ANDROID 지정함. 나중에 빼야함.(googleplay 연동을 위해)
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if ( is_login_ )
        {
            NetworkManager.DisconnectSocket( UrlPath.lobby );
        }
        else
        {
            Debug.LogWarning( "You can't sign in Google Play Service" );
            EventHandlerGroup eg = new EventHandlerGroup(OpenLogin, CloseLogin);
            NetworkManagerV2.Instance.ConnectServer(UrlPath.login);


//           EventHandlerGroup eg = new EventHandlerGroup( OpenLogin, MessageLogin, CloseLogin );
            //UrlMaker url_path = new UrlMaker( UrlPath.login );
            //UrlMaker url_path = UrlMaker.CreateUrlMaker("sample", UrlPath.login);

            //WebSocket ws = NetworkManager.Instance.ResisterSocket( url_path, eg );
           // ws.SetCredentials( "guest", "password", false );
            //ws.ConnectAsync();

        }

#elif UNITY_ANDROID || UNITY_IOS // for GooglePlayGameService

        

        Social.localUser.Authenticate((bool success) =>
        {
            //gpgd.Authenticating = false;
            if (success)
            {
                // if we signed in successfully, load data from cloud
                //LoadFromCloud();
                Debug.Log("success, id = " + Social.localUser.id);
            }
            else
            {
                // no need to show error message (error messages are shown automatically
                // by plugin)
                Debug.LogWarning("Failed to sign in with Google Play Games.");
            }
        });


        #region legacy
        //GooglePlayGamesData gpgd = DataManager.Instance.Google_Play_Data;

        //if(gpgd.IsAuthenticate())
        //{
        //    return;
        //}

        //// Enable/disable logs on the PlayGamesPlatform
        //PlayGamesPlatform.DebugLogEnabled = GameConsts.PlayGamesDebugLogsEnabled;

        //// Activate the Play Games platform. This will make it the default
        //// implementation of Social.Active
        //PlayGamesPlatform.Activate();

        //// Set the default leaderboard for the leaderboards UI
        //((PlayGamesPlatform)Social.Active).SetDefaultLeaderboardForUI(GameIds.LeaderboardId);


        //// Sign in to Google Play Games
        //gpgd.Authenticating = true;
        //Social.localUser.Authenticate((bool success) =>
        //{
        //    gpgd.Authenticating = false;
        //    if (success)
        //    {
        //        // if we signed in successfully, load data from cloud
        //        //LoadFromCloud();
        //        Debug.Log("success, id = " + Social.localUser.id);
        //    }
        //    else
        //    {
        //        // no need to show error message (error messages are shown automatically
        //        // by plugin)
        //        Debug.LogWarning("Failed to sign in with Google Play Games.");
        //    }
        //});
        #endregion

#endif
    }

    public void ConnectSampleServer2()
    {
        SoundManager.PlayEffect_Touch();
        ConnectServerTest();
        //if(NetworkManager.Instance.SocketReady)
        //{
        //    NetworkManager.Instance.closeSocket();
        //    label_login_.text = "로그인";
        //}else
        //{
        //    ConnectServer();
        //    label_login_.text = "로그아웃";
        //}
    }

    public void SetLoginButton(bool is_login)
    {
        is_login_ = is_login;
        label_login_.text = is_login ? "로그아웃" : "로그인";
    }


	//--------------------------------------------------------------------------
	// protected mono methods
	//--------------------------------------------------------------------------
	protected void Awake()
	{
		menuController = this;

        label_login_ = transform.FindChild("Button_Login/Label").GetComponent<UILabel>();
        common_ui_ = MainController.GetCommonUI();

        if (DataManager.Instance)
        {
            transform.FindChild("Label_Version").GetComponent<UILabel>().text = DataManager.Instance.Version;
        }
	}

    protected void Start()
    {
        bool result = false;
        if(NetworkManager.Instance)
        { 
            result = NetworkManager.Instance.IsAliveSocket( UrlPath.login );
        }
        SetLoginButton( result);
    }

    protected void OnEnable()
    {
        if ( common_ui_ ) { common_ui_.setVisibleSideButton( false ); }
        NetworkManager.AddOnMessageListener( this );

        //if(NetworkManager.IsAbleToSendData)
    }

    protected void OnDisable()
    {
        if ( common_ui_ ) { common_ui_.setVisibleSideButton( true ); }
        NetworkManager.RemoveOnMessageListener( this );
    }

    protected void OnDestroy()
    {
        if ( menuController != null )
        {
            menuController = null;
        }
    }

    //protected void Update()
    //{
    //}

	//--------------------------------------------------------------------------
	// private methods
	//--------------------------------------------------------------------------

    void ConnectServer()
    {
        //MainController.GetNetworkManager().ConnectSocket( TeamGehem.Path.echo );
        //NetworkManager instance = NetworkManager.Instance;
        //instance.setupSocket();
        //yield return new WaitForSeconds(1.0f);
        //instance.writeSocket("Hello?, " + instance.GetMyIP());
    }

    void ConnectServerTest()
    {
        //MainController.GetNetworkManager().ConnectSocket2( TeamGehem.Path.echo );
    }

    void OpenLogin( object sendwr, EventArgs e )
    {
        NetworkManager.Instance.EnqueueAction( () =>
            {
                MenuController.menuController.SetLoginButton(true);
            }
        );
    }

    void DispatchText( string text )
    {
        Toast.ShowToast( text );

        //SocketPackage sp = null;
        //if ( NetworkManager.Instance.GetSocketPackage( UrlPath.login, out sp ) )
        //{
            //sp.Web_Socket.OnMessage -= MessageLogin;
            //sp.Web_Socket.OnMessage += ControllManager.Instance.MessageControll;
        //}
    }

    void DispatchUserInfo( IEwkProtocol ewk )
    {
        UserInfo user_info = ewk.GetData<UserInfo>();
        if (user_info != null)
        {
            DataManager.User_Info = user_info;
            NetworkManager.Instance.EnqueueAction(() =>{
                Debug.Log(string.Format("MessageLogin :: Opcode.BINARY = {0}", user_info));
       
                SocketPackage sp = null;
                if (NetworkManager.Instance.GetSocketPackage(UrlPath.login, out sp))
                {
                    sp.DisconnectAsyncSocket();
                }
            });
        }
    }

    void MessageLogin( object sender, MessageEventArgs e )
    {
        if ( e.Type == Opcode.BINARY )
        {
            UserInfo user_info = null;
            user_info = EwkProtoSerilazer.DeserializeForData<UserInfo>( e.RawData );
            if (user_info != null)
            {
                Debug.Log(string.Format("MessageLogin :: Opcode.BINARY = {0}", user_info));
                DataManager.User_Info = user_info;
                NetworkManager.Instance.EnqueueAction(() =>
                {
                    SocketPackage sp = null;
                    if (NetworkManager.Instance.GetSocketPackage(UrlPath.login, out sp))
                    {
                        sp.DisconnectAsyncSocket();
                    }
                });
            }
        }
        else
        {
            NetworkManager.Instance.EnqueueAction( () =>
            {
                Toast.ShowToast( e.Data );

                SocketPackage sp = null;
                if ( NetworkManager.Instance.GetSocketPackage( UrlPath.login, out sp ) )
                {
                    sp.Web_Socket.OnMessage -= MessageLogin;
                    sp.Web_Socket.OnMessage += ControllManager.Instance.MessageControll;
                }
            }
            );
        }
        //NetworkManager.Instance.DisconnectSocket( UrlPath.login );
    }

    void CloseLogin( object sender, CloseEventArgs e )
    {
        if (e.WasClean)
        {
            NetworkManager.Instance.EnqueueAction(() =>
               {
                   NetworkManager.Instance.ConnectSocket(UrlPath.lobby);
                   //if (MenuController.menuController != null)
                   //{
                   //    MenuController.menuController.SetLoginButton(false);
                   //}
                   //MainController.ShowToast(string.Format("로그아웃 하셨습니다."), 1.5f);
                   //DataManager.GuestId = null;
                   //
               }
           );
        }
    }
}
