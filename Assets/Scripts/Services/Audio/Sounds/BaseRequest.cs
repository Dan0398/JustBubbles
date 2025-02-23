using UnityEngine;

namespace Services.Audio.Sounds
{
    public class BaseRequest : MonoBehaviour
    {
        protected Service parentalService;
        private bool _serviceRequested;
        
        public void TryPlaySound(SoundType type)
        {
            TryGetService();
            if (parentalService == null) return;
            parentalService.Play(type);
        }
        
        public void TryStopSound(SoundType type)
        {
            TryGetService();
            if (parentalService == null) return;
            parentalService.Stop(type);
        }
        
        private void TryGetService()
        {
            if (_serviceRequested) return;
            parentalService = Services.DI.Single<Service>();
            _serviceRequested = true;
        }
    }
}