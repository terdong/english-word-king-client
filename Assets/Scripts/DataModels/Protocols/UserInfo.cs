using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace TeamGehem.DataModels.Protocols
{
    [ProtoContract]
    public class UserInfo
    {
        [ProtoMember(2)]
        public int Guest_Id { get; private set; }
        [ProtoMember(3)]
        public int Room_Index { get; set; }
        [ProtoMember(4)]
        public string First_Session_Id { get; private set; }
        [ProtoMember(5)]
        public string Guest_Name { get; private set; }
        [ProtoMember(6)]
        public string Game_Url_Path { get; private set; }

        public static UserInfo CreateUserInfo(int guest_id, string guest_name, string first_session_id, string game_url_path)
        {
            return new UserInfo(guest_id, guest_name, first_session_id, game_url_path);
        }

        public UserInfo() { }
        private UserInfo(int guest_id, string guest_name, string first_session_id, string game_url_path)
        {
            Guest_Id = guest_id;
            Room_Index = -1;
            First_Session_Id = first_session_id;
            Guest_Name = guest_name;
            Game_Url_Path = game_url_path;
        }
        public override string ToString()
        {
            return string.Format("Guest_Id={0}, Guest_Name={1}, First_Session_Id={2}, Room_Index={3}, Game_Url_Path={4}", Guest_Id, Guest_Name, First_Session_Id, Room_Index, Game_Url_Path);
        }
    }
}
