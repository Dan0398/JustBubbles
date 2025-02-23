using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public partial class Controller : MonoBehaviour
    {
        private int _fallAnimationsCount;
        
        public void AnimateFallUnconnectedBubbles(List<Bubble> Bubbles)
        {
            if (Bubbles == null || Bubbles.Count == 0)
            {
                return;
            }
            foreach(var Bubble in Bubbles)
            {
                _fallAnimationsCount++;
                Bubble.OnSceneAnimationEnds?.Invoke();
                Bubble.MyTransform.SetParent(_movingParent);
                Bubble.MyRigid.isKinematic = false;
                Bubble.MyRigid.AddForce(Vector2.right * _fallenForceScale * Random.Range(-1f, 1f), ForceMode2D.Impulse);
                Bubble.OnScene.layer = _backgroundLayer;
            }
            StartCoroutine(SeekBubblesFall(Bubbles));
        }
        
        private IEnumerator SeekBubblesFall(List<Bubble> bubbles)
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
                        Bubbles[i].OnScene.layer = _defaultLayer;
                        _bubblesPool.Hide(Bubbles[i]);
                        Bubbles.RemoveAt(i);
                        _fallAnimationsCount--;
                        i--;
                        continue;
                    }
                    if (SeekTime > 1 && Mathf.Approximately(Bubbles[i].MyRigid.velocity.y, 0))
                    {
                        var Bubble =  Bubbles[i];
                        Bubble.MyRigid.isKinematic = true;
                        Bubble.OnScene.layer = _defaultLayer;
                        StartCoroutine(SoftHideBubble(Bubble, () => _fallAnimationsCount--));
                        Bubbles.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
                yield return _wait;
                SeekTime += Time.deltaTime;
            }
            TryResetMovingParent();
        }
    }
}