using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataModels
{
	class UserData
	{
        public UserData()
        {
            Guest_Id_ = null;
            User_Name_ = null;
        }
        public string Guest_Id_ { get; set; }
        public string User_Name_ { get; set; }
	}
}
