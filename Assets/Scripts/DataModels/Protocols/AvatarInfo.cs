using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamGehem.DataModels.Protocols
{
    [ProtoContract]
    public class AvatarInfo
    {
        [ProtoMember(1)]
        public int [] Left_Data { get; private set; }
        [ProtoMember(2)]
        public int [] Right_Data { get; private set; }

        public static AvatarInfo CreateAvatarInfo(int[] left_data, int[] right_data)
        {
            return new AvatarInfo( left_data, right_data );
        }

        public AvatarInfo() { }
        private AvatarInfo( int[] left_data, int[] right_data )
        {
            Left_Data = left_data;
            Right_Data = right_data;
        }
    }
}
