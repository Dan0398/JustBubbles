using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class SniperShot : BaseInstrument
    {
        public override void ShowAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            gameObject.SetActive(true);
            
            if (_turnOffRoutine != null) StopCoroutine(_turnOffRoutine);
            if (_showRoutine != null) StopCoroutine(_showRoutine);
            
            Sounds.Play(Services.Audio.Sounds.SoundType.SniperTake);
            _showRoutine = StartCoroutine(AnimateShow(Duration, OnAnimationEnds));
            
            void OnAnimationEnds()
            {
                InstrumentShown = true;
                RefreshLineStatus();
            }
        }
        
        private IEnumerator AnimateShow(float Duration, System.Action OnEnd, bool isInverted = false)
        {
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float Lerp = 0;
            var rifleSprite = _rifleOnScene.GetComponent<SpriteRenderer>();
            for (int Step = 0; Step <= Steps; Step++)
            {
                Lerp = Mathf.Sin(Step/(float)Steps * 90 * Mathf.Deg2Rad);
                if(isInverted) Lerp = 1 - Lerp;
                
                _rifleOnScene.localPosition = OutOfViewPos * (1-Lerp);
                rifleSprite.color = Color.white - Color.black * (1-Lerp);
                
                yield return Wait;
            }
            OnEnd?.Invoke();
        }

        public override void HideAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            InstrumentShown = false;
            RefreshLineStatus();
            if (_showRoutine != null) StopCoroutine(_showRoutine);
            _showRoutine = StartCoroutine(AnimateShow(Duration, AfterHide, true));
            
            void AfterHide()
            {
                gameObject.SetActive(false);
                OnEnd?.Invoke();
            }
        }

        public override void ProcessAimVector(Vector2 Vector)
        {
            base.ProcessAimVector(Vector);
            if (!User.MouseInsideField) return;
            RotateRifle();
        }
        
        private void RotateRifle()
        {
            if (!_rifleParent) _rifleParent = _rifleOnScene.parent;
            if (!_rifleSprite) _rifleSprite = _rifleOnScene.GetComponent<SpriteRenderer>();
            _rifleOnScene.rotation = Quaternion.LookRotation(_rifleOnScene.forward, new Vector3(MouseClampedDirection.y, -MouseClampedDirection.x));
            _rifleSprite.flipY = MouseClampedDirection.x >= 0;
        }
    }
}