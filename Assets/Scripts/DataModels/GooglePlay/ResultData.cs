using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataModels
{
	class ResultData
	{
        public int Correct_Answer_Number { get; set; }
        public int Wrong_Answer_Number { get; set; }
        public int Total_Turn { get; set; }
        public String Fight_Time { get; set; }
        public int Combo_Number { get; set; }
	}

    class RewardData
    {
        public int Fight_Bonus { get; set; }
        public int Victory_Bonus { get; set; }
        public int Score_Bonus { get; set; }
        public int Combo_Bonus { get; set; }
        public int Total_Bonus
        {
            get
            {
                return Fight_Bonus + Victory_Bonus + Score_Bonus + Combo_Bonus;
            }
        }
    }
}
