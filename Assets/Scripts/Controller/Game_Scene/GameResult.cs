using UnityEngine;
using System.Collections;

public class GameResult : MonoBehaviour {

    enum ResultLabel
    {
        Winner = 0,
        Loser,
        CorrectAnswerNumber,
        WrongAnswerNumber,
        TotalTurn,
        Time,
        Combo,
        FightBonus,
        VictoryBonus,
        ScoreBonus,
        ComboBonus,
        TotalBonus,
        Max_
    }

    public UILabel[] labels;

	// Use this for initialization
	void Start () {
        foreach(UILabel label in labels)
        {
            label.text = "";
        }
	}

    /// <summary>
    /// TODO : 결과 화면 데이터 입력 메소드.
    /// </summary>
    public void SetContents()
    {

    }
}
