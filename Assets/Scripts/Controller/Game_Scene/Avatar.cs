using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

    struct AvatarStat
    {
        public void SetAvatarStat(
            string a_name,
            float a_health,
            int a_score,
            string a_record
        )
        {
            name = a_name;
            health = a_health;
            score = a_score;
            record = a_record;
        }
        public string name;
        public float health;
        public int score;
        public string record;
    }

    UILabel label_name;
    UILabel label_score;
    UILabel label_record;
    UISlider slider_health;

    AvatarStat avatar_stat_;
    public void UpdateGUI()
    {
        label_name.text = avatar_stat_.name;
        label_score.text = avatar_stat_.score.ToString();
        label_record.text = avatar_stat_.record.ToString();
        SetHealth(avatar_stat_.health);
    }

    public void SetAvatarInfo(
        string a_name,
        string a_record
        )
    {
        avatar_stat_.name = a_name;
        avatar_stat_.record = a_record;
        UpdateGUI();
    }

    public void SetFightInfo( float a_health, int a_score)
    {
        avatar_stat_.health = a_health;
        avatar_stat_.score = a_score;
        UpdateGUI();
    }

    public void SetAvatar(
        string a_name,
        float a_health,
        int a_score,
        string a_record)
    {
        avatar_stat_.SetAvatarStat( a_name, a_health, a_score, a_record );
    }
    public void SetHealth(float add_health)
    {
        //avatar_stat_.health += add_health;
        //slider_health.value = avatar_stat_.health * 0.01f;
        slider_health.value = add_health * 0.01f;
    }

    void Awake()
    {
        label_name = transform.FindChild("Label_Name").GetComponent<UILabel>();
        label_score = transform.FindChild("Label_Score").GetComponent<UILabel>();
        label_record = transform.FindChild("Label_Record").GetComponent<UILabel>();
        slider_health = transform.FindChild("Control_Slider").GetComponent<UISlider>();

        avatar_stat_ = new AvatarStat();
//        avatar_stat_.health = 100.0f;
    }

	void Start ()
    {
	}
}
