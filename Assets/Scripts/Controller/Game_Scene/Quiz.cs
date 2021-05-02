using UnityEngine;
using System.Collections;
using System;
using TeamGehem.DataModels.Protocols;
using TeamGehem;

public class Quiz : MonoBehaviour {
    private static readonly string Quiz_Content_Sentence = "[Q. {0}] What does \"{1}\" mean?";
    private static readonly string Fight_Sentence = "제{0}턴 - {1} 하실 차례입니다.";
    private static readonly string Answer_Sentence = "{0}. {1}";
    private static readonly string Fight_Auto_Sentence = "자동 {0}";
    private static readonly int Max_Quiz_Count = 5;

    int right_answer_index_;
    int quiz_count_;

    UILabel[] label_answer_;
    UILabel label_quiz_;
    UILabel label_count_;

    SocketPackage sp;

    public void OnClickButton(GameObject go)
    {
        //SoundManager.PlayEffect("gui_click");
        int selected_number = System.Convert.ToInt32(go.name.Remove(0, 15));

        SetDisableSelectedButton( go.transform, true );

        CheckRightAnswer(selected_number);
    }
    public void setContents(QuizInfo quiz_info)
    {
        if (quiz_info.Quiz_Current_Turn > -1)
        {
            label_quiz_.text = string.Format(Quiz_Content_Sentence, quiz_info.Quiz_Current_Turn, quiz_info.Quiz_Question);
            for (int i = 0; i < quiz_info.Quiz_Example.Length; ++i)
            {
                label_answer_[i].text = string.Format(Answer_Sentence, i + 1, quiz_info.Quiz_Example[i]);
                Debug.Log(string.Format("label_answer_[{0}].text = {1}", i ,label_answer_[i].text));
            }

            for ( int i = 0; i < 4; ++i )
            {
                SetDisableSelectedButton( transform.FindChild( "Sprite_Button_0" + i ), false );
            }
        }

        label_count_.text = quiz_info.Quiz_Count_Time.ToString();
    }

    public IEnumerator SetContents(QuizInfo quiz_info)
    {
        yield return new WaitForEndOfFrame();
//        yield return new WaitForSeconds(0.1f);
        if (quiz_info.Quiz_Current_Turn > -1)
        {
            label_quiz_.text = string.Format(Quiz_Content_Sentence, quiz_info.Quiz_Current_Turn, quiz_info.Quiz_Question);
            for (int i = 0; i < quiz_info.Quiz_Example.Length; ++i)
            {
                label_answer_[i].text = string.Format(Answer_Sentence, i + 1, quiz_info.Quiz_Example[i]);
                Debug.Log(string.Format("label_answer_[{0}].text = {1}", i, label_answer_[i].text));
            }

            for (int i = 0; i < 4; ++i)
            {
                SetDisableSelectedButton(transform.FindChild("Sprite_Button_0" + i), false);
            }
        }

        label_count_.text = quiz_info.Quiz_Count_Time.ToString();
    }

    public void InitializeQuizGUI()
    {

    }

    public void InitializeFightGUI()
    {

    }

    void Awake()
    {
        label_answer_ = new UILabel[4];
        for(int i=0; i< label_answer_.Length; ++i)
        {
            label_answer_[i] = transform.FindChild("Sprite_Button_0" + i + "/Label").GetComponent<UILabel>();
        }

        label_quiz_ = transform.FindChild("Label_Quiz").GetComponent<UILabel>();
        label_count_ = transform.FindChild("Label_Count").GetComponent<UILabel>();

        NetworkManager.Instance.GetSocketPackage(UrlPath.game, out sp);
    }

    void Start()
    {
        Reset();
    }

    void Reset()
    {
        right_answer_index_ = 0;
        quiz_count_ = Max_Quiz_Count;
        for (int i = 0; i < label_answer_.Length; ++i)
        {
            label_answer_[i].text = string.Empty;
        }

        label_quiz_.text = string.Empty;
        label_count_.text = quiz_count_.ToString();
    }

    IEnumerator StartQuizCount()
    {
        for (int i = 0; i < 5; ++i)
        {
            label_count_.text = (--quiz_count_).ToString();
            yield return new WaitForSeconds(2.0f);
        }
        //yield return null;
    }

    void CheckRightAnswer(int selected_number)
    {
        sp.Send(EwkProtoFactory.CreateEwkProtocol<int>(ProtocolEnum.RightAnswer, selected_number).GetBytes);

        //if(IsRightAnswer(selected_number))
        //{
        //    GameController.ChangeGameMode(GameController.GameMode.Fight);
        //}else
        //{

        //}
    }

    bool IsRightAnswer(int selected_number)
    {
        return right_answer_index_ == selected_number;
    }

    public void SetDisableSelectedButton( Transform trans, bool isDisable )
    {
        Debug.Log( "be called SetDisableSelectedButton" );
        if ( trans != null ) { trans.GetComponent<UIButton>().isEnabled = !isDisable; }
        trans.FindChild("Label").GetComponent<UILabel>().color = !isDisable ? Color.white : Color.gray;
    }
}
