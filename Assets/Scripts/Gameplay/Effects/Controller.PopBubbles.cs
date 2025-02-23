using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public partial class Controller : MonoBehaviour
    {
        private int _popAnimationsCount;
        
        public void PopBubbles(List<Bubble> Bubbles)
        {
            if (Bubbles == null || Bubbles.Count == 0)
            {
                return;
            }
            foreach(var Bubble in Bubbles)
            {
                _popAnimationsCount++;
                Bubble.OnSceneAnimationEnds?.Invoke();
                Bubble.MyTransform.SetParent(_movingParent);
                Bubble.DeactivateCollisions();
                Bubble.MyRigid.isKinematic = true;
            }
            StartCoroutine(AnimatePopping(Bubbles));
        }

        private IEnumerator AnimatePopping(List<Bubble> Bubbles)
        {
            if (_bubblePopTransform == null)
            {
                _bubblePopTransform = _bubblePopParticle.transform;
            }
            var Main = _bubblePopParticle.main;
            Main.startColor = ColorPicker.GetColorByEnum(Bubbles[0].MyColor);
            for (int i = 0; i < Bubbles.Count; i ++)
            {
                PlayPopEffectAt(Bubbles[i].MyTransform.position + Vector3.back * 0.1f);
                yield return _wait;
                yield return _wait;
                StartCoroutine(SoftHideBubble(Bubbles[i], RemoveAndCheck));
            }
            
            void RemoveAndCheck()
            {
                _popAnimationsCount--;
                TryResetMovingParent();
            }
        }
        
        public void PlayPopEffectAt(Vector3 pos)
        {
            _bubblePopTransform.position = pos;
            _bubblePopParticle.Emit(1);
            _sounds.PlayBubblePop();
        }
        
        private IEnumerator SoftHideBubble(Bubble Target, System.Action OnEnd = null)
        {
            const int Steps = 20;
            
            var Renderer = Target.OnScene.GetComponent<SpriteRenderer>();
            var Shadow = Target.MyTransform.GetChild(0).GetComponent<SpriteRenderer>();
            var OldColor = ColorPicker.GetColorByEnum(Target.MyColor);
            var NewColor = new Color(OldColor.r,OldColor.g,OldColor.b,0);
            for (int Step = 1; Step < Steps; Step ++)
            {
                Renderer.color = Color.Lerp(OldColor, NewColor, Step/(float)Steps);
                Shadow.color = new Color(0,0,0,1 - Mathf.Clamp01( (Step*2f)/(float)Steps));
                yield return _wait;
            }
            _bubblesPool.Hide(Target);
            Shadow.color = Color.black;
            OnEnd?.Invoke();
        }
        
        public void HideBubbles(IEnumerable<Bubble> bubbles)
        {
            foreach(var bubble in bubbles)
            {
                bubble.OnSceneAnimationEnds?.Invoke();
            }
            _bubblesPool.Hide(bubbles);
        }
    }
}