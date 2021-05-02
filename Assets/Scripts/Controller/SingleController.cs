using UnityEngine;
using System.Collections;

public class SingleController : MonoBehaviour {

    private DataService data_service_;

	// Use this for initialization
	void Start () {
        data_service_ = new DataService("db_eng_words.db");

        //data_service_.DropTables();
        
        //data_service_.CreateWord("word", "단어");
        //Word word = data_service_.GetWord("word");
        //Debug.Log(word + ", " + data_service_.GetWordInfo(word.id));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
