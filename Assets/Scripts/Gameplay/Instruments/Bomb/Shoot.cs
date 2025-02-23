using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bomb
{
    public partial class Bomb : BaseInstrument
    {
        public override void ReactOnClickDown()
        {
            _wannaShoot = true;
            if (!User.UsingTouch) ShootBomb();
        }

        public override void ReactOnClickUp()
        {
            _wannaShoot = false;
            if (User.UsingTouch) ShootBomb();
        }
        
        private void ShootBomb()
        {
            if (_inFly) return;
            if (!InstrumentShown) return;
            if (!User.IsClickedInGameField()) return;
            if (_bombOnScene == null) return;
            
            var FlyTrajectory = new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, TrajectoryCollisionsCount);
            Sounds.Stop(Services.Audio.Sounds.SoundType.BombIdle);
            Sounds.Play(Services.Audio.Sounds.SoundType.BombFly);
            StartCoroutine(AnimateBombFly(_bombOnScene.transform, ProcessEndBombWay));
            _inFly = true;
        
            IEnumerator AnimateBombFly(Transform Target, System.Action OnEnd)
            {
                FlyTrajectory.PrepareFirst(User.transform.position, MouseClampedDirection);
                Target.position = FlyTrajectory.PosOnWay;
                FlyTrajectory.StepLengthOnWay = _bombFlySpeed;
                while (!FlyTrajectory.Completed)
                {
                    FlyTrajectory.TryStepAndCheckCollisions();
                    Target.position = FlyTrajectory.PosOnWay;
                    yield return Wait;
                }
                OnEnd?.Invoke();
            }
            
            void ProcessEndBombWay()
            {
                Sounds.Stop(Services.Audio.Sounds.SoundType.BombFly);
                Sounds.Play(Services.Audio.Sounds.SoundType.BombExplode);
                _bombOnScene.SetActive(false);
                _effects.PlayExplosionEffects(FlyTrajectory.PosOnWay);
                Field.ProcessExplosion(FlyTrajectory.PosOnWay, _explodeRadius);
                AfterUse?.Invoke();
                _inFly = false;
            }
        }
    }
}