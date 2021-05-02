using UnityEngine;

/// <summary>
/// Very simple example of how to use a TextList with a UIInput for chat.
/// </summary>

[RequireComponent(typeof(UIInput))]
public class GehemChatInput : MonoBehaviour
{
    public ReadyRoom parent_;
	public UITextList textList;
	public bool fillWithDummyData = false;

	UIInput mInput;

	/// <summary>
	/// Submit notification is sent by UIInput when 'enter' is pressed or iOS/Android keyboard finalizes input.
	/// </summary>

	public void OnSubmit ()
	{
		if (textList != null)
		{
			// It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
			string text = NGUIText.StripSymbols(mInput.value);

			if (!string.IsNullOrEmpty(text))
			{
                // TODO : 서버로 채팅 데이터 전송.[Terdong : 2014-08-04]
				//textList.Add(text);

                parent_.RequestTalkMessage( text );
				mInput.value = "";
				mInput.isSelected = false;
			}
		}
	}

    public void AddChatMessageToTextList( string message )
    {
        textList.Add( message );
    }

    void Start()
    {
        mInput = GetComponent<UIInput>();
        mInput.label.maxLineCount = 1;

        //textList.textLabel.color = Color.black;
        textList.Add("[36454f]");

        if ( fillWithDummyData && textList != null )
        {
            for ( int i = 0; i < 30; ++i )
            {
                textList.Add( ( ( i % 2 == 0 ) ? "[FFFFFF]" : "[AAAAAA]" ) +
                    "This is an example paragraph for the text list, testing line " + i + "[-]" );
            }
        }
    }
}
