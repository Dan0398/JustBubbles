using Utils.Observables;

namespace Data
{
    public class User: IAbstractData
    {
        public int SelectedBubbleID, SelectedBackgroundID;
        public ObsInt ColorCountInEndless;
        public ObsInt SurvivalBestScore;
        public int StrategyBestScore;
        
        [UnityEngine.Scripting.Preserve]
        public User()
        {
            SelectedBubbleID = 0;
            SelectedBackgroundID = 0;
            ColorCountInEndless = 4;
            SurvivalBestScore = 0;
            StrategyBestScore = 0;
        }

        public void SetValuesAsFromStart()
        {
            SelectedBubbleID = 0;
            SelectedBackgroundID = 0;
            ColorCountInEndless = 4;;
            SurvivalBestScore = 0;
            StrategyBestScore = 0;
        }
    }
}