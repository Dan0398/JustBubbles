using UnityEngine;

namespace Services.Audio.Sounds
{
    public class OnEnableSound: BaseRequest
    {
        [SerializeField] SoundType PlayType;
        [SerializeField] bool StopOnDisable;
        
        void OnEnable()
        {
            TryPlaySound(PlayType);
        }
        
        void OnDisable()
        {
            if (!StopOnDisable) return;
            TryStopSound(PlayType);
        }
    }
}