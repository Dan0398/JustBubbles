using UnityEngine;

namespace Services.Audio.Sounds
{
    public class OnEnableSound: BaseRequest
    {
        [SerializeField] private SoundType _playType;
        [SerializeField] private bool _stopOnDisable;
        
        private void OnEnable()
        {
            TryPlaySound(_playType);
        }
        
        private void OnDisable()
        {
            if (!_stopOnDisable) return;
            TryStopSound(_playType);
        }
    }
}