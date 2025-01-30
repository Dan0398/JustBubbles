using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class SniperShot : BaseInstrument
    {
        public override void ReactOnClickDown()
        {
            userPressed = true;
            RefreshLineStatus();
            if (!User.UsingTouch) ProcessShot();
        }

        public override void ReactOnClickUp()
        {
            userPressed = false;
            RefreshLineStatus();
            if (User.UsingTouch) ProcessShot();
        }
        
        void RefreshLineStatus()
        {
            AimLine.SetActive(instrumentShown && !ShotProcessed && userPressed == User.UsingTouch);
        }
        
        void ProcessShot()
        {
            if (ShotProcessed) return;
            if (!instrumentShown) return;
            if (!User.IsClickedInGameField()) return;
            
            if (!rifleParent) rifleParent = rifleOnScene.parent;
            
            Sounds.Play(Services.Audio.Sounds.SoundType.SniperShoot);
            var Result = Field.ProcessSniperShot(rifleParent.position, mouseClampedDirection, 30);
            if (Result == null) return;
            StartCoroutine(AnimateBulletFly());
            
            IEnumerator AnimateBulletFly()
            {
                BulletOnScene.gameObject.SetActive(true);
                BulletOnScene.position = rifleParent.position;
                BulletOnScene.rotation = rifleOnScene.rotation;
                Vector3 BulletStep = mouseClampedDirection * BulletSpeed;
                ShotProcessed = true;
                RefreshLineStatus();
                float DistanceSqrt = 0;
                while(Result.RemoveBubbleByDistance(DistanceSqrt) > 0)
                {
                    BulletOnScene.position += BulletStep;
                    DistanceSqrt += BulletSpeed;
                    yield return Wait;
                }
                BulletOnScene.gameObject.SetActive(false);
                Result.OnRequireCleanField.Invoke();
                ShotProcessed = false;
                RefreshLineStatus();
                AfterUse?.Invoke();
            }
        }
    }
}
