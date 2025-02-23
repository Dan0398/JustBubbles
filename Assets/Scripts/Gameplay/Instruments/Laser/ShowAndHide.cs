using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class Laser : BaseInstrument
    {
        private System.Action<float> _idleSoundChange;
        
        public override void ShowAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            _trajectory ??= new User.Trajectory(CollisionRadius, Field.TryResponseCollision, 1, TrajectoryCollisionsCount);
            _underAttack ??= new List<DamagedBubble>(5);
            gameObject.SetActive(true);
            _targetBubble = null;
            _underAttack.Clear();
            _idleSoundChange = Sounds.PlayAndGiveVolumeChange(Services.Audio.Sounds.SoundType.LaserIdle);
            ReactOnFieldMove();
            _energy = _maxEnergy;
            
            StartCoroutine(AnimateShow(Duration, OnAnimationEnds));
            
            void OnAnimationEnds() => InstrumentShown = true;
        }
        
        private IEnumerator AnimateShow(float Duration, System.Action OnEnd, bool isInverted = false)
        {
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float Lerp = 0;
            var renderer = _laserOnScene.GetComponent<SpriteRenderer>();
            for (int Step = 0; Step <= Steps; Step++)
            {
                Lerp = 1 - Mathf.Sin(Step/(float)Steps * 90 * Mathf.Deg2Rad);
                if(isInverted) Lerp = 1 - Lerp;
                _idleSoundChange.Invoke(1 - Lerp);
                renderer.color = Color.white - Color.black * Lerp;
                _laserOnScene.localPosition = OutOfViewPos * Lerp;
                yield return Wait;
            }
            OnEnd?.Invoke();
        }
        
        public override void HideAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            InstrumentShown = false;
            StartCoroutine(AnimateShow(Duration, AfterHide, true));
            
            void AfterHide()
            {
                gameObject.SetActive(false);
                OnEnd?.Invoke();
            }
        }
    }
}