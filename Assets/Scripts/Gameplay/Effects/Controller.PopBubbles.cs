using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public partial class Controller : MonoBehaviour
    {
        int PopAnimationsCount;
        
        public void PopBubbles(List<Bubble> Bubbles)
        {
            if (Bubbles == null || Bubbles.Count == 0)
            {
                return;
            }
            foreach(var Bubble in Bubbles)
            {
                PopAnimationsCount++;
                Bubble.OnSceneAnimationEnds?.Invoke();
                Bubble.MyTransform.SetParent(MovingParent);
                Bubble.DeactivateCollisions();
                Bubble.MyRigid.isKinematic = true;
            }
            StartCoroutine(AnimatePopping(Bubbles));
        }

        IEnumerator AnimatePopping(List<Bubble> Bubbles)
        {
            if (BubblePopTransform == null)
            {
                BubblePopTransform = BubblePopParticle.transform;
            }
            var Main = BubblePopParticle.main;
            Main.startColor = ColorPicker.GetColorByEnum(Bubbles[0].MyColor);
            for (int i = 0; i < Bubbles.Count; i ++)
            {
                PlayPopEffectAt(Bubbles[i].MyTransform.position + Vector3.back * 0.1f);
                yield return Wait;
                yield return Wait;
                StartCoroutine(SoftHideBubble(Bubbles[i], RemoveAndCheck));
            }
            
            void RemoveAndCheck()
            {
                PopAnimationsCount--;
                TryResetMovingParent();
            }
        }
        
        public void PlayPopEffectAt(Vector3 pos)
        {
            BubblePopTransform.position = pos;
            BubblePopParticle.Emit(1);
            Sounds.PlayBubblePop();
        }
        
        IEnumerator SoftHideBubble(Bubble Target, System.Action OnEnd = null)
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
                yield return Wait;
            }
            BubblesPool.Hide(Target);
            Shadow.color = Color.black;
            OnEnd?.Invoke();
        }
        
        public void HideBubbles(IEnumerable<Bubble> bubbles)
        {
            foreach(var bubble in bubbles)
            {
                bubble.OnSceneAnimationEnds?.Invoke();
            }
            BubblesPool.Hide(bubbles);
        }
    }
}