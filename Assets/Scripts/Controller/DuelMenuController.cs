using UnityEngine;
using System.Collections;
using TeamGehem;
using TeamGehem.DataModels.Protocols;

public class DuelMenuController : MonoBehaviour {


    public void MoveToTemp()
    {
        Toast.ShowToast("준비 중 입니다.");
    }

    public void MoveToDuelGameScene()
    {
        SoundManager.PlayEffect_Touch();

        SocketPackage sp = null;
        if (NetworkManager.IsAbleToSendData() && NetworkManager.Instance.GetSocketPackage(UrlPath.lobby, out sp))
        {
            if (sp != null)
            {
                sp.Send(EwkProtoFactory.CreateEwkProtocol<SceneListEnum>(ProtocolEnum.Req_Change_Scene, SceneListEnum.Game_Scene).GetBytes);
            }
        }
        else
        {
            Toast.ShowToast("로그인 후 입장하실 수 있습니다.");
        }
    }

    //void Awake()
    //{

    //}

    void Start()
    {
//        NetworkManager.Instance.ConnectSocket(UrlPath.lobby);
    }

    //void OnEnable()
    //{

    //}

    //void OnDisable()
    //{

    //}

    void OnDestroy()
    {
        //NetworkManager.DisconnectSocket(UrlPath.lobby);
    }
}
