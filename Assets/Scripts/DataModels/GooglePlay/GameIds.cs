using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DataModels
{
    public class GameIds
    {
        // Achievements IDs (as given by Developer Console)
        public class Achievements
        {
            public const string NotADisaster = "PLACEHOLDER"; // <GPGSID>
            public const string PointBlank = "PLACEHOLDER"; // <GPGSID>
            public const string FullCombo = "PLACEHOLDER"; // <GPGSID>
            public const string ClearAllLevels = "PLACEHOLDER"; // <GPGSID>
            public const string PerfectAccuracy = "PLACEHOLDER"; // <GPGSID>

            public static string[] ForRank = {
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER" // <GPGSID>
        };
            public static int[] RankRequired = { 3, 6, 10 };

            public static string[] ForTotalStars = {
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER" // <GPGSID>
        };
            public static int[] TotalStarsRequired = { 12, 24, 36 };

            // incrementals:
            public static string[] IncGameplaySeconds = {
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER" // <GPGSID>
        };
            public static string[] IncGameplayRounds = {
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER", // <GPGSID>
            "PLACEHOLDER" // <GPGSID>
        };
        }

        // Leaderboard ID (as given by Developer Console)
        public static string LeaderboardId = "PLACEHOLDER"; // <GPGSID>
    }
}
