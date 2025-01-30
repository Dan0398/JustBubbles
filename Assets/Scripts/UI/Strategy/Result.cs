namespace UI.Strategy
{
    public class Result
    {
        public int SessionResult, BestResult;
        public bool IsNewHighScore, IsSuccess;
        public string LeaderBoardName, HintLangKey;
        public System.Action OnEnd, OnRetry;
    }
}