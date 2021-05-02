using UnityEngine;
using System.Collections;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayManager : MonoBehaviour {
    public static GooglePlayManager Instance { get; private set; }

    private void RetrieveUserEmail()
    {
//        using (AndroidJavaClass jc_plus = new AndroidJavaClass("com.google.android.gms.plus.Plus"))
//        {
//            using (AndroidJavaObject jo_plusAccountApi = jc_plus.GetStatic<AndroidJavaObject>("AccountApi"))
//            {
//                mUserEmail = jo_plusAccountApi.Call<string>("getAccountName", Instance. mGHManager.GetApiClient());
//               Logger.d("Player email: " + mUserEmail);
//            }
//        }
    }

	void Awake()
    {
 //       PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
    // enables saving game progress.        
 //       .EnableSavedGames() 

        // registers a callback to handle game invitations received while the game is not running.
        //.WithInvitationDelegate(<callback method>) 

        // registers a callback for turn based match notifications received while the game is not running.
       // .WithMatchDelegate(<callback method>) 

  //      .Build();

  //      PlayGamesPlatform.InitializeInstance(config);

        // recommended for debugging:
   //     PlayGamesPlatform.DebugLogEnabled = true;

        // Activate the Google Play Games platform
    //    PlayGamesPlatform.Activate();

        //((PlayGamesPlatform)Social.Active).SetDefaultLeaderboardForUI(GameIds.LeaderboardId);
    }

    void OnDestroy()
    {
     //   ((PlayGamesPlatform)Social.Active).SignOut();

        if (Instance != null) { Instance = null; }
    }
}
