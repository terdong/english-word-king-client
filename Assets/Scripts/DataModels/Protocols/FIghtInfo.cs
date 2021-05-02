using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamGehem.DataModels.Protocols
{
    [ProtoContract]
    public class FightInfo
    {
        [ProtoMember(2)]
        public int [] Hp { get; private set; }
        [ProtoMember(3)]
        public int [] Score { get; private set; }

        public static FightInfo CreateFightInfo(params int [][] a_param)
        {
            return new FightInfo(a_param);
        }

        public FightInfo() { }
        private FightInfo(int [][] a_param)
        {
            Hp = new int[a_param.Length];
            Score = new int[a_param.Length];
            for (int i = 0; i < a_param.Length; ++i)
            {
                Hp[i] = a_param[i][0];
                Score[i] = a_param[i][1];
            }
        }

        public override string ToString()
        {
            return string.Format("L_Hp={0}, L_Score={1}, R_Hp={2}, R_Score={3}", Hp[0], Score[0], Hp[1], Score[1]);
        }
    }
}
