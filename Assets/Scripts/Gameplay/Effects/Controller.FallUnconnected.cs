using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public partial class Controller : MonoBehaviour
    {
        int FallAnimationsCount;
        
        public void AnimateFallUnconnectedBubbles(List<Bubble> Bubbles)
        {
            if (Bubbles == null || Bubbles.Count == 0)
            {
                return;
            }
            foreach(var Bubble in Bubbles)
            {
                FallAnimationsCount++;
                Bubble.OnSceneAnimationEnds?.Invoke();
                Bubble.MyTransform.SetParent(MovingParent);
                Bubble.MyRigid.isKinematic = false;
                Bubble.MyRigid.AddForce(Vector2.right * FallenForceScale * Random.Range(-1f, 1f), ForceMode2D.Impulse);
                Bubble.OnScene.layer = BackgroundLayer;
            }
            StartCoroutine(SeekBubblesFall(Bubbles));
        }
        
        IEnumerator SeekBubblesFall(List<Bubble> bubbles)
        {
            float SeekTime = 0;
            var Bubbles = new List<Bubble>(bubbles.Count);
            Bubbles.AddRange(bubbles);
            while (Bubbles.Count > 0)
            {
                for (int i=0; i< Bubbles.Count; i++)
                {
                    if (Bubbles[i].MyTransform.position.y < -10)
                    {
                        Bubbles[i].MyRigid.isKinematic = true;
                        Bubbles[i].OnScene.layer = DefaultLayer;
                        BubblesPool.Hide(Bubbles[i]);
                        Bubbles.RemoveAt(i);
                        FallAnimationsCount--;
                        i--;
                        continue;
                    }
                    if (SeekTime > 1 && Mathf.Approximately(Bubbles[i].MyRigid.velocity.y, 0))
                    {
                        var Bubble =  Bubbles[i];
                        Bubble.MyRigid.isKinematic = true;
                        Bubble.OnScene.layer = DefaultLayer;
                        StartCoroutine(SoftHideBubble(Bubble, () => FallAnimationsCount--));
                        Bubbles.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
                yield return Wait;
                SeekTime += Time.deltaTime;
            }
            TryResetMovingParent();
        }
    }
}
