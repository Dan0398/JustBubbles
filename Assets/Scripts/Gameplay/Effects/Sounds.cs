using UnityEngine;
using Services.Audio.Sounds;

namespace Gameplay.Effects
{
    [System.Serializable]
    public class Sounds
    {
        private Service _soundService;
        private bool _serviceRequested;
        private float _popPitch = 1;
        
        public void PlayBubblePop()
        {
            _popPitch = Mathf.Clamp(_popPitch + Random.Range(-0.05f, 0.05f), 0.9f,1.3f);
            RequestService();
            if (_soundService == null) return;
            _soundService.Play(SoundType.BubblePop, _popPitch);
        }
        
        public void PlayBubbleSet()
        {
            RequestService();
            if (_soundService == null) return;
            _soundService.Play(SoundType.BubbleSet);
        }
        
        private void RequestService()
        {
            if (!_serviceRequested)
            {
                _soundService = Services.DI.Single<Service>();
                _serviceRequested = true;
            }
        }
    }
}