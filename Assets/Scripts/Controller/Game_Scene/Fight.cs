using UnityEngine;
using System.Collections;
using TeamGehem.DataModels.Protocols;

public class Fight : MonoBehaviour {
    private static readonly string Record_Message = "W : {0}  L : {1}  D : {2}";

    public ProtocolEnum My_Avatar_Direction_ { get; set; }

    Avatar avatar_left_;
    Avatar avatar_right_;

    public void UpdateFightInfo(FightInfo fight_info)
    {
        avatar_left_.SetFightInfo( fight_info.Hp[0], fight_info.Score[0] );
        avatar_right_.SetFightInfo( fight_info.Hp[1], fight_info.Score[1] );
    }

    public void UpdateAvatarInfo( AvatarInfo avatar_info )
    {
        string left_data = string.Format( Record_Message, avatar_info.Left_Data[0], avatar_info.Left_Data[1], avatar_info.Left_Data[2] );
        string right_data = string.Format( Record_Message, avatar_info.Right_Data[0], avatar_info.Right_Data[1], avatar_info.Right_Data[2]);
        if ( My_Avatar_Direction_ == ProtocolEnum.Avatar_Direction_Left )
        {
            avatar_left_.SetAvatarInfo( "나(임시)", left_data );
            avatar_right_.SetAvatarInfo( "상대방", right_data );
        }
        else
        {
            avatar_left_.SetAvatarInfo( "상대방", left_data );
            avatar_right_.SetAvatarInfo( "나(임시)", right_data );
        }
    }

    void Awake()
    {
        avatar_left_ = transform.FindChild("Avatar_Left").GetComponent<Avatar>();
        avatar_right_ = transform.FindChild("Avatar_Right").GetComponent<Avatar>();
        My_Avatar_Direction_ = ProtocolEnum.Avatar_Direction_Left;
    }

	void Start () {
        avatar_left_.SetAvatar(
            "몽키.D.루피",
            100.0f,
            0,
            string.Format(Record_Message,0,0,0));
        avatar_left_.UpdateGUI();

        avatar_right_.SetAvatar(
            "아이언맨",
            100.0f,
            0,
            string.Format( Record_Message, 0, 0, 0 ) );
        avatar_right_.UpdateGUI();
	}
}
