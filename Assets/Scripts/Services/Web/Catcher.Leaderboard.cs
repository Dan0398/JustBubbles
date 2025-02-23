#pragma warning disable CS0618 
#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        private static Leaderboards.Line[] _lbLines;
        
        public static async UniTask<Leaderboards.Line[]> RequestLeaderboard(string LeaderBoardTechName)
        {
            _lbLines = null;
            Application.ExternalCall("GetLeaderboardsByName", LeaderBoardTechName);
            while(_lbLines == null) await Utilities.Wait();
            return _lbLines;
        }
        
        public void ApplyLeaderboard(string LBLinesInJSON)
        {
            _lbLines = Newtonsoft.Json.JsonConvert.DeserializeObject<Services.Leaderboards.Line[]>(LBLinesInJSON);
        }
        
        public static void SetNewScoreInLeaderboards(Services.Leaderboards.ScoreWrite NewScore)
        {
            var ScoreString = Newtonsoft.Json.JsonConvert.SerializeObject(NewScore);
            Application.ExternalCall("SetNewScoreInLeaderboards", ScoreString);
        }
    }
}
#endif