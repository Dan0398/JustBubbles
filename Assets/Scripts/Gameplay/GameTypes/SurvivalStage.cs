using UnityEngine;

namespace Gameplay.GameType
{
    [System.Serializable]
    public class SurvivalStage
    {
        [SerializeField] readonly float duration;
        [SerializeField] readonly int sceneColors, rewardByComboCount;
        [SerializeField] readonly float speedStart, speedEnd;
        [SerializeField] float timeFromStart;
        
        public int SceneColors => sceneColors;
        
        public int RewardByComboCount => rewardByComboCount;
        
        public float TimeOfStageRelative => Mathf.Clamp01(timeFromStart/duration);
        
        public float Speed => Mathf.Lerp(speedStart, speedEnd, TimeOfStageRelative);

        public bool RequireChangeStage => timeFromStart >= duration;

        public float TimeOutOfDuration => Mathf.Clamp(timeFromStart - duration, 0, float.MaxValue);
                
        public SurvivalStage(int SceneColors, float Duration, int RewardByComboCount, float SpeedStart, float SpeedEnd)
        {
            sceneColors = SceneColors;
            duration = Duration;
            rewardByComboCount = RewardByComboCount;
            speedStart = SpeedStart * Time.fixedDeltaTime;
            speedEnd = SpeedEnd * Time.fixedDeltaTime;
        }
        
        public void RegisterTime(float timeScale)
        {
            timeFromStart += timeScale;
        }
    }
}