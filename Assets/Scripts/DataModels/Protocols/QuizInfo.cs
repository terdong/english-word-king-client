using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace TeamGehem.DataModels.Protocols
{
    [ProtoContract]
    public class QuizInfo
    {
        [ProtoMember( 2 )]
        public string Quiz_Question { get; private set; }
        [ProtoMember( 3 )]
        public string[] Quiz_Example { get; private set; }
        [ProtoMember( 4 )]
        public int Quiz_Count_Time { get; private set; }
        [ProtoMember( 5 )]
        public int Quiz_Current_Turn {get; private set; }

        public static QuizInfo CreateUserInfo( string quiz_question, string[] quiz_string, int quiz_count, int current_turn )
        {
            return new QuizInfo( quiz_question, quiz_string, quiz_count, current_turn);
        }

        public static QuizInfo CreateUserInfo(int quiz_count)
        {
            return new QuizInfo(string.Empty, null, quiz_count, -1);
        }

        public QuizInfo() { }
        public QuizInfo(string quiz_question, string[] quiz_string, int quiz_count, int current_turn)
        {
            Quiz_Question = quiz_question;
            Quiz_Example = quiz_string;
            Quiz_Count_Time = quiz_count;
            Quiz_Current_Turn = current_turn;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Quiz_Count_Time = {0}, Quiz_Current_Turn = {1}, ", Quiz_Count_Time, Quiz_Current_Turn);
            if (!string.IsNullOrEmpty(Quiz_Question))
            {
                sb.AppendFormat("Quiz_Question = {0}, ", Quiz_Question);
            }
            if (Quiz_Example != null)
            {
                sb.Append("Quiz_Example = {");
                for (int i = 0; i < Quiz_Example.Length; ++i)
                {
                    sb.AppendFormat("{0},", Quiz_Example[i]);
                }
                sb.Append( "}" );
            }

            return sb.ToString();
        }
    }
}
