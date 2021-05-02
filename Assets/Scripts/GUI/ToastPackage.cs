using UnityEngine;
using System.Collections;

public class ToastPackage : MonoBehaviour {
    private readonly static string the_margin_for_icon = "        {0}";
    private readonly static string icon_full_path = "icon_{0}";
#if UNITY_EDITOR
    private readonly static float valid_toast_width = 800.0f;
#else
    //private readonly static float valid_toast_width = System.Convert.ToSingle(Screen.currentResolution.width) * 0.6f;
    private readonly static float valid_toast_width = 800.0f;
#endif

    public UILabel label_text_;
    public UISprite sprite_icon_;
    public GameObject gameobject_icon_;

    public void SetContent(string message)
    {
        SetContent(0, message);
    }

    public void SetContent(int toast_type, string message)
    {
        bool is_active_icon = !toast_type.Equals(0);

        if (is_active_icon)
        {
            label_text_.text = string.Format(the_margin_for_icon, message);
            sprite_icon_.spriteName = string.Format(icon_full_path, toast_type);
        }
        else
        {
            label_text_.text = message;
        }

        gameobject_icon_.SetActive(is_active_icon);

        CheckValidToastWidth();
    }

    private void CheckValidToastWidth()
    {
        if (label_text_.width > valid_toast_width)
        {
            label_text_.overflowMethod = UILabel.Overflow.ResizeHeight;
            label_text_.width = System.Convert.ToInt32(valid_toast_width);
        }
    }
}
