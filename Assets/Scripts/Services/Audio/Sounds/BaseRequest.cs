using UnityEngine;

namespace Services.Audio.Sounds
{
    public class BaseRequest : MonoBehaviour
    {
        protected Service parentalService;
        bool serviceRequested;
        
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
        
        void TryGetService()
        {
            if (serviceRequested) return;
            parentalService = Services.DI.Single<Service>();
            serviceRequested = true;
        }
    }
}