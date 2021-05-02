using UnityEngine;
using System.Collections;
using TeamGehem;

public class CommonUI : MonoBehaviour {
    private static readonly string Left_Button_Text_Default = "뒤로";
    private static readonly string Right_Button_Text_Default = "확인";

    public delegate void CommonButtonEvent();

    GameObject side_button_group_;
    GameObject right_button_;
    GameObject left_button_;
    UILabel label_left_button_;
    UILabel label_right_button_;
    Toast toast_;

    public CommonButtonEvent Left_Button_Delegate
    {
        private get;
        set;
    }

    CommonButtonEvent right_button_delegate_ = null;
    public CommonButtonEvent Right_Button_Delegate
    {
        private get { return right_button_delegate_; }
        set
        {
            right_button_delegate_ = value;
            if ( right_button_ != null ) { right_button_.SetActive( value != null ); }
        }
    }

    public void SetTextLeftButtonLabel( string label_name )
    {
        label_left_button_.text = label_name;
    }
    public void SetTextRightButtonLabel( string label_name )
    {
        label_right_button_.text = label_name;
    }
    public void InitializeButtonsLabel()
    {
        label_left_button_.text = Left_Button_Text_Default;
        label_right_button_.text = Right_Button_Text_Default;

        SetDisableRightButton( false );

        Right_Button_Delegate = null;
    }

    public void SetDisableRightButton( bool isDisable )
    {
        if ( right_button_ != null ) { right_button_.GetComponent<UIButton>().isEnabled = !isDisable; }
        label_right_button_.color = !isDisable ? Color.black : Color.gray;
    }

    public void CallLeftDelegate()
    {
        if ( Left_Button_Delegate != null )
        {
            SoundManager.PlayEffect_Touch();
            Left_Button_Delegate();
        }
    }

    public void CallRightDelegate()
    {
        if ( Right_Button_Delegate != null )
        {
            SoundManager.PlayEffect_Touch();
            Right_Button_Delegate();
        }
    }

    public void OpenOptionPopup()
    {
        SoundManager.PlayEffect_Touch();
        Debug.Log("called OpenOptionPopup");
        Toast.ShowToast("준비 중 입니다.");
    }

    public void OpenWordBookScene()
    {
        SoundManager.PlayEffect_Touch();
        Debug.Log("called OpenWordBookScene");
        Toast.ShowToast("준비 중 입니다.");
    }

    public void setVisibleSideButton(bool isVisible)
    {
        side_button_group_.GetComponent<UIWidget>().alpha = isVisible ? 1.0f : 0;
    }

    void Awake()
    {
        side_button_group_ = transform.FindChild( "Bottom_UI/Side_Button_Group" ).gameObject;
        left_button_ = side_button_group_.transform.FindChild("Button_Left").gameObject;
        right_button_ = side_button_group_.transform.FindChild("Button_Right").gameObject;
        label_left_button_ = left_button_.transform.FindChild("Label").GetComponent<UILabel>();
        label_right_button_ = right_button_.transform.FindChild("Label").GetComponent<UILabel>();

        Left_Button_Delegate = MoveToPrevScene;
        Right_Button_Delegate = null;

//        toast_ = transform.FindChild("Toast_UI").GetComponent<Toast>();

        InitializeButtonsLabel();
    }

    //void Start()
    //{
    //}

    void MoveToPrevScene()
    {
        MainController.MovePrevScene();
    }
}
