using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class SniperShot : BaseInstrument
    {
        public override void ReactOnClickDown()
        {
            _userPressed = true;
            RefreshLineStatus();
            if (!User.UsingTouch) ProcessShot();
        }

        public override void ReactOnClickUp()
        {
            _userPressed = false;
            RefreshLineStatus();
            if (User.UsingTouch) ProcessShot();
        }
        
        private void RefreshLineStatus()
        {
            _aimLine.SetActive(InstrumentShown && !_shotProcessed && _userPressed == User.UsingTouch);
        }
        
        private void ProcessShot()
        {
            if (_shotProcessed) return;
            if (!InstrumentShown) return;
            if (!User.IsClickedInGameField()) return;
            
            if (!_rifleParent) _rifleParent = _rifleOnScene.parent;
            
            Sounds.Play(Services.Audio.Sounds.SoundType.SniperShoot);
            var Result = Field.ProcessSniperShot(_rifleParent.position, MouseClampedDirection, 30);
            if (Result == null) return;
            StartCoroutine(AnimateBulletFly());
            
            IEnumerator AnimateBulletFly()
            {
                _bulletOnScene.gameObject.SetActive(true);
                _bulletOnScene.position = _rifleParent.position;
                _bulletOnScene.rotation = _rifleOnScene.rotation;
                Vector3 BulletStep = MouseClampedDirection * _bulletSpeed;
                _shotProcessed = true;
                RefreshLineStatus();
                float DistanceSqrt = 0;
                while(Result.RemoveBubbleByDistance(DistanceSqrt) > 0)
                {
                    _bulletOnScene.position += BulletStep;
                    DistanceSqrt += _bulletSpeed;
                    yield return Wait;
                }
                _bulletOnScene.gameObject.SetActive(false);
                Result.OnRequireCleanField.Invoke();
                _shotProcessed = false;
                RefreshLineStatus();
                AfterUse?.Invoke();
            }
        }
    }
}