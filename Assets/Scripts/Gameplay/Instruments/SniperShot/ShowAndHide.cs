using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments
{
    public partial class SniperShot : BaseInstrument
    {
        public override void ShowAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            gameObject.SetActive(true);
            
            if (turnOffRoutine != null) StopCoroutine(turnOffRoutine);
            if (showRoutine != null) StopCoroutine(showRoutine);
            //turnOffRequired = false;
            
            Sounds.Play(Services.Audio.Sounds.SoundType.SniperTake);
            showRoutine = StartCoroutine(AnimateShow(Duration, OnAnimationEnds));
            
            void OnAnimationEnds()
            {
                instrumentShown = true;
                RefreshLineStatus();
            }
        }
        
        IEnumerator AnimateShow(float Duration, System.Action OnEnd, bool isInverted = false)
        {
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float Lerp = 0;
            var rifleSprite = rifleOnScene.GetComponent<SpriteRenderer>();
            for (int Step = 0; Step <= Steps; Step++)
            {
                Lerp = Mathf.Sin(Step/(float)Steps * 90 * Mathf.Deg2Rad);
                if(isInverted) Lerp = 1 - Lerp;
                
                rifleOnScene.localPosition = OutOfViewPos * (1-Lerp);
                rifleSprite.color = Color.white - Color.black * (1-Lerp);
                
                yield return Wait;
            }
            OnEnd?.Invoke();
        }

        public override void HideAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            //turnOffRequired = true;
            instrumentShown = false;
            RefreshLineStatus();
            if (showRoutine != null) StopCoroutine(showRoutine);
            showRoutine = StartCoroutine(AnimateShow(Duration, AfterHide, true));
            
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
        
        void RotateRifle()
        {
            if (!rifleParent) rifleParent = rifleOnScene.parent;
            if (!rifleSprite) rifleSprite = rifleOnScene.GetComponent<SpriteRenderer>();
            rifleOnScene.rotation = Quaternion.LookRotation(rifleOnScene.forward, new Vector3(mouseClampedDirection.y, -mouseClampedDirection.x));
            rifleSprite.flipY = mouseClampedDirection.x >= 0;
        }
    }
}