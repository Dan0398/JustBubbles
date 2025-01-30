using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bomb
{
    public partial class Bomb : BaseInstrument
    {
        public override void ReactOnClickDown()
        {
            wannaShoot = true;
            if (!User.UsingTouch) ShootBomb();
        }

        public override void ReactOnClickUp()
        {
            wannaShoot = false;
            if (User.UsingTouch) ShootBomb();
        }
        
        private void ShootBomb()
        {
            if (inFly) return;
            if (!instrumentShown) return;
            if (!User.IsClickedInGameField()) return;
            if (BombOnScene == null) return;
            
            var FlyTrajectory = new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, trajectoryCollisionsCount);
            Sounds.Stop(Services.Audio.Sounds.SoundType.BombIdle);
            Sounds.Play(Services.Audio.Sounds.SoundType.BombFly);
            //Effects.PlayBombFlySound();
            StartCoroutine(AnimateBombFly(BombOnScene.transform, ProcessEndBombWay));
            inFly = true;
        
            IEnumerator AnimateBombFly(Transform Target, System.Action OnEnd)
            {
                FlyTrajectory.PrepareFirst(User.transform.position, mouseClampedDirection);
                Target.position = FlyTrajectory.PosOnWay;
                FlyTrajectory.StepLengthOnWay = BombFlySpeed;
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
                BombOnScene.SetActive(false);
                Effects.PlayExplosionEffects(FlyTrajectory.PosOnWay);
                Field.ProcessExplosion(FlyTrajectory.PosOnWay, ExplodeRadius);
                AfterUse?.Invoke();
                inFly = false;
            }
        }
    }
}