using UnityEngine;
using System.Collections;
using System.Text;
using TeamGehem;

public enum Toast_Type
{
    None = 0,
    Success,
    Failure,
    Warnning
}

public class Toast : MonoBehaviour {
    private static readonly int Default_Tost_Depth = DepthGroup.Toast;
    private static readonly float Fixed_Duration = 2.0f;
    private static readonly string Debug_Log_Message = "Toast_Msg = {0}";

    public MonoBehaviour Parent_ = null;

    private static Toast Instance_ = null;
    public ToastPackage toast_package_origin_;

    float fade_duration_ = 0.5f;
    int toast_count_ = 0;
    int toast_depth_count_ = Default_Tost_Depth;

    public static void ShowToast(int toast_type, string message)
    {
        if (Instance_ != null && Instance_.Parent_ != null)
        {
            Instance_.Parent_.StartCoroutine(Instance_.ShowToast_(toast_type, message, Fixed_Duration));
            Debug.Log(string.Format(Debug_Log_Message, message));
        }
    }

    public static void ShowToast(Toast_Type toast_type, string message)
    {
        ShowToast(toast_type.GetHashCode(), message);
    }

    public static void ShowToast(string message)
    {
        ShowToast(Toast_Type.None.GetHashCode(), message);
    }

    //public static void ShowToast(MessageEnum m_e)
    //{
    //    tpl_message message = SqliteDataManager.GetMessage(m_e);
    //    ShowToast(message.icon_type, message.message);
    //}

    public static void OnToastSuccess(string message)
    {
        ShowToast(Toast_Type.Success.GetHashCode(), message);
    }

    //public static void OnToastError(ErrorResource error)
    //{
    //    ShowToast(Toast_Type.Failure.GetHashCode(), error.message);
    //    CommonUI.ToggleLoadingIndicator(false);
    //}

    public static void OnToastError(string msg)
    {
        ShowToast(msg);
    }

    public IEnumerator ShowToast_(int toast_type, string message, float toast_duration)
    {
        CheckToastCountToShowToastUI();

        GameObject toast_go = NGUITools.AddChild(this.gameObject, toast_package_origin_.gameObject);
        ToastPackage toast_package = toast_go.GetComponent<ToastPackage>();
        toast_package.GetComponent<UIPanel>().depth = toast_depth_count_++;

        AddToastCount( 1 );

        toast_package.SetContent(toast_type, message);

        toast_go.gameObject.SetActive(true);

        yield return StartCoroutine("FadeIn", toast_go);

        yield return new WaitForSeconds( toast_duration );

        yield return StartCoroutine("FadeOut", toast_go);

        Destroy(toast_go);

        AddToastCount( -1 );
        CheckToastCountToHideToastUI();
    }

    void AddToastCount( int val )
    {
        toast_count_ += val;
    }

    void CheckToastCountToShowToastUI()
    {
        if ( toast_count_ <= 0 )
        {
            transform.gameObject.SetActive( true );
            toast_depth_count_ = Default_Tost_Depth;
        }
    }

    void CheckToastCountToHideToastUI()
    {
        if ( toast_count_ <= 0 )
        {
            transform.gameObject.SetActive( false );
        }
    }

    IEnumerator FadeIn(GameObject game_object)
    {
        TweenAlpha.Begin(game_object, fade_duration_, 1.0f);
        yield return new WaitForSeconds(fade_duration_);
    }

    IEnumerator FadeOut(GameObject game_object)
    {
        TweenAlpha.Begin(game_object, fade_duration_, 0.0f);
        yield return new WaitForSeconds(fade_duration_);
    }

    void Awake()
    {
        Instance_ = this;

        //toast_package_origin_ = Resources.Load<ToastPackage>("GUIPrefabs/Toast_Package");

        gameObject.SetActive(false);

    }

    void OnDestroy()
    {
        if (Instance_ != null) { Instance_ = null; }

        toast_package_origin_ = null;
        Resources.UnloadUnusedAssets();
    }
}
