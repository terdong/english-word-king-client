using UnityEngine;
using System.Collections;
using WebSocketSharp;
using TeamGehem;
using TeamGehem.DataModels.Protocols;

public class ControllManager : MonoBehaviour {

    public static ControllManager Instance { get; private set; }

    SocketPackage socket_package_ = null;

    void DispatchRes_Change_Scene( IEwkProtocol ewk )
    {
        SceneListEnum scene_name = ewk.GetData<SceneListEnum>();
        MainController.SwitchScene( scene_name );
    }

    public void MessageControll( object sender, MessageEventArgs e )
    {
        Debug.Log( string.Format( "ControllManager:: received message = {0}", e.Data ) );

        NetworkManager.Instance.EnqueueAction( () =>
        {
            if(e.Data.Equals("invalid"))
            {
                Toast.ShowToast("입장하실수 없습니다.\n로그인을 먼저 하셔야 합니다.");
            }
            else
            {
                //MainController.SwitchScene( "Duel_Menu_Scene" );
                if (e.Data.Equals("Game_Scene"))
                {
                    //NetworkManager.Instance.DisconnectSocket(socket_package_.Key);
                }
                MainController.SwitchScene( e.Data );
            }
        }
);
    }

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        NetworkManager.AddOnMessageListener( this );
    }

    void OnDisable()
    {
        NetworkManager.RemoveOnMessageListener( this );
    }

    void OnDestroy()
    {
        if ( Instance != null )
        {
            Instance = null;
        }
    }
}
