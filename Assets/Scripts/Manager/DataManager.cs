using UnityEngine;
using System.Collections;
using Assets.Scripts.DataModels;
using TeamGehem.DataModels.Protocols;

public class DataManager : MonoBehaviour {

    public static DataManager Instance { get; private set; }

    /// <summary>
    /// 버전 규칙(X.Y.Z)
    /// X : 대규모 업데이트 시 +1
    /// Y : 달(Month) 기준 업데이트시 +1
    /// Z : VCS 커밋 기준 업데이트시 +1
    /// </summary>
    public string Version{ private set; get; }

    public GooglePlayGamesData Google_Play_Data { get; private set; }

    UserData user_data_;

    UserInfo user_info_ = null;

    public static string UserName
    {
        get { return Instance.user_data_.User_Name_; }
        private set { Instance.user_data_.User_Name_ = value; }
    }

    public static string GuestId
    {
        get { return Instance.user_data_.Guest_Id_; }
        set { Instance.user_data_.Guest_Id_ = value; }
    }

    public static UserInfo User_Info
    {
        get { return Instance.user_info_; }
        set { Instance.user_info_ = value; }
    }

    public static bool IsLogin()
    {
        return Instance.user_info_ != null;
    }

    void Awake()
    {
        Instance = this;

        Version = "v1.0.70";

        Google_Play_Data = new GooglePlayGamesData();

        user_data_ = new UserData();
        user_data_.User_Name_ = "guest";
    }

    void OnDestroy()
    {
        if (Instance != null)
        {
            Instance = null;
        }
    }
}
