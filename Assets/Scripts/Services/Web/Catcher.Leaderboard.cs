#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        static Services.Leaderboards.Line[] LBLines;
        
        public static async UniTask<Services.Leaderboards.Line[]> RequestLeaderboard(string LeaderBoardTechName)
        {
            LBLines = null;
            Application.ExternalCall("GetLeaderboardsByName", LeaderBoardTechName);
            while(LBLines == null) await Utilities.Wait();
            return LBLines;
        }
        
        public void ApplyLeaderboard(string LBLinesInJSON)
        {
            LBLines = Newtonsoft.Json.JsonConvert.DeserializeObject<Services.Leaderboards.Line[]>(LBLinesInJSON);
        }
        
        public static void SetNewScoreInLeaderboards(Services.Leaderboards.ScoreWrite NewScore)
        {
            var ScoreString = Newtonsoft.Json.JsonConvert.SerializeObject(NewScore);
            Application.ExternalCall("SetNewScoreInLeaderboards", ScoreString);
        }
    }
}
#endif