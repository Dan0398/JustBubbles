namespace Services.Leaderboards 
{
    public class ScoreWrite
    {
        public string LeaderboardName;
        public int Score;
        
        public ScoreWrite(string leaderboardName, int score)
        {
            LeaderboardName = leaderboardName;
            Score = score;
        }        
    }
}