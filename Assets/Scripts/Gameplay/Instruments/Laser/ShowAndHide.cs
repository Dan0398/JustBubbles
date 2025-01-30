using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class Laser : BaseInstrument
    {
        System.Action<float> IdleSoundChange;
        public override void ShowAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            if (traj == null) traj = new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, trajectoryCollisionsCount);
            if (underAttack == null) underAttack = new List<DamagedBubble>(5);
            gameObject.SetActive(true);
            TargetBubble = null;
            underAttack.Clear();
            IdleSoundChange = Sounds.PlayAndGiveVolumeChange(Services.Audio.Sounds.SoundType.LaserIdle);
            //idleSound.Play();
            ReactOnFieldMove();
            Energy = MaxEnergy;
            
            StartCoroutine(AnimateShow(Duration, OnAnimationEnds));
            
            void OnAnimationEnds() => instrumentShown = true;
        }
        
        IEnumerator AnimateShow(float Duration, System.Action OnEnd, bool isInverted = false)
        {
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float Lerp = 0;
            var renderer = laserOnScene.GetComponent<SpriteRenderer>();
            for (int Step = 0; Step <= Steps; Step++)
            {
                Lerp = 1 - Mathf.Sin(Step/(float)Steps * 90 * Mathf.Deg2Rad);
                if(isInverted) Lerp = 1 - Lerp;
                IdleSoundChange.Invoke(1 - Lerp);
                //idleSound.ChangeVolume(1 - Lerp);
                renderer.color = Color.white - Color.black * Lerp;
                laserOnScene.localPosition = OutOfViewPos * Lerp;
                yield return Wait;
            }
            OnEnd?.Invoke();
        }
        
        public override void HideAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            instrumentShown = false;
            StartCoroutine(AnimateShow(Duration, AfterHide, true));
            
            void AfterHide()
            {
                gameObject.SetActive(false);
                OnEnd?.Invoke();
            }
        }
    }
}
