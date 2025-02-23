using UnityEngine;

namespace Gameplay.GameType
{
    [System.Serializable]
    public class SurvivalStage
    {
        [SerializeField] private readonly float _duration;
        [SerializeField] private readonly int _sceneColors;
        [SerializeField] private readonly int _rewardByComboCount;
        [SerializeField] private readonly float _speedStart;
        [SerializeField] private readonly float _speedEnd;
        [SerializeField] private float _timeFromStart;
        
        public int SceneColors => _sceneColors;
        
        public int RewardByComboCount => _rewardByComboCount;
        
        public float TimeOfStageRelative => Mathf.Clamp01(_timeFromStart/_duration);
        
        public float Speed => Mathf.Lerp(_speedStart, _speedEnd, TimeOfStageRelative);

        public bool RequireChangeStage => _timeFromStart >= _duration;

        public float TimeOutOfDuration => Mathf.Clamp(_timeFromStart - _duration, 0, float.MaxValue);
                
        public SurvivalStage(int SceneColors, float Duration, int RewardByComboCount, float SpeedStart, float SpeedEnd)
        {
            _sceneColors = SceneColors;
            _duration = Duration;
            _rewardByComboCount = RewardByComboCount;
            _speedStart = SpeedStart * Time.fixedDeltaTime;
            _speedEnd = SpeedEnd * Time.fixedDeltaTime;
        }
        
        public void RegisterTime(float timeScale)
        {
            _timeFromStart += timeScale;
        }
    }
}