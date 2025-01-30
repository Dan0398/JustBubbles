using UnityEngine;
using Services.Audio.Sounds;

namespace Gameplay.Effects
{
    [System.Serializable]
    public class Sounds
    {
        bool serviceRequested;
        Service SoundService;
        float popPitch = 1;
        
        public void PlayBubblePop()
        {
            //BubblePop.time = 0;
            popPitch = Mathf.Clamp(popPitch + Random.Range(-0.05f, 0.05f), 0.9f,1.3f);
            RequestService();
            if (SoundService == null) return;
            SoundService.Play(SoundType.BubblePop, popPitch);
        }
        
        public void PlayBubbleSet()
        {
            RequestService();
            if (SoundService == null) return;
            SoundService.Play(SoundType.BubbleSet);
        }
        
        void RequestService()
        {
            if (!serviceRequested)
            {
                SoundService = Services.DI.Single<Service>();
                serviceRequested = true;
            }
        }
    }
}