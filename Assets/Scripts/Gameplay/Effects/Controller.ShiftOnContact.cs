using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public partial class Controller : MonoBehaviour
    {
        public class ShiftPackage
        {
            const float WorldAffect = 0.3f;
            bool Reversed;
            public Bubble bubble {get; private set;}
            Vector3 OldPosInLocal, EndPlaceInLocal, ShiftedPlaceInLocal;
            Transform BubbleTransform;
            
            
            ShiftPackage(Bubble bubble, float Multiplier, Vector3 Dir, float ZOutstand)
            {
                Reversed = false;
                this.bubble = bubble;
                BubbleTransform = bubble.MyTransform;
                ShiftedPlaceInLocal = BubbleTransform.localPosition + Dir * Multiplier * WorldAffect - Vector3.forward * 0.1f * ZOutstand;
                EndPlaceInLocal = bubble.LocalPosInLine;
            }
            
            public ShiftPackage(Bubble bubble, Vector3 Dir, float Multiplier, float ZOutstand): this(bubble, Multiplier, Dir, ZOutstand)
            {
                OldPosInLocal = BubbleTransform.parent.InverseTransformPoint(BubbleTransform.position);
            }
            
            public ShiftPackage(Bubble bubble, Vector3 Dir, float Multiplier, Vector3 WorldPos, float ZOutstand): this(bubble, Multiplier, Dir, ZOutstand)
            {
                OldPosInLocal = BubbleTransform.parent.InverseTransformPoint(WorldPos);
                ShiftedPlaceInLocal = EndPlaceInLocal + Dir * Multiplier * WorldAffect - Vector3.forward * 0.1f * ZOutstand;
            }
            
            public void ApplyShift(float Scale)
            {
                if (!Reversed)
                {
                    BubbleTransform.localPosition = Vector3.Lerp(OldPosInLocal, ShiftedPlaceInLocal, Scale);
                }
                else 
                {
                    BubbleTransform.localPosition = Vector3.Lerp(EndPlaceInLocal, ShiftedPlaceInLocal, Scale);
                }
            }
            
            public void Reverse() => Reversed = true;
        }
    
        public void ShiftBubblesAfterContact(Bubble Central, List<Bubble> ConnectedBubbles, Vector3 ConnectionDirection, Vector3 WorldPos, float MaxDistance)
        {
            Sounds.PlayBubbleSet();
            List<ShiftPackage> Shifted = new(5)
            {
                new(Central, ConnectionDirection, .4f, WorldPos, 1)
            };
            
            for(int i = 0; i < ConnectedBubbles.Count; i ++)
            {
                if (Central == ConnectedBubbles[i]) continue;
                var Dir = ConnectedBubbles[i].MyTransform.position - Central.MyTransform.position;
                float Dot = Vector3.Dot(Dir, ConnectionDirection);
                if (Dot <= 0) continue;
                var Dist = Dir.magnitude;
                var FixedDot = EasingFunction.EaseOutCubic(0, 1, Dot);
                Dist = 1 - Mathf.Clamp01(Dist / MaxDistance);
                var Result = FixedDot * Dist;
                if (Mathf.Approximately(Result, 0)) continue;
                Shifted.Add(new ShiftPackage(ConnectedBubbles[i], Dir.normalized, Result, Dist));
            }
            if (Shifted.Count < 2) return;
            StartCoroutine(AnimateShiftAfterContact(Shifted));
        }
        
        IEnumerator AnimateShiftAfterContact(List<ShiftPackage> Shifted)
        {
            const int FramesOfAnimation = 20;
            const int FramesHalf = 10;
            
            float Lerp = 0;
            for (int k = 0; k < Shifted.Count; k++)
            {
                var Curr = Shifted[k];
                Curr.bubble.OnSceneAnimationEnds?.Invoke();
                Curr.bubble.OnSceneAnimationEnds = () => 
                {
                    Curr.ApplyShift(0);
                    Shifted.Remove(Curr);
                    Curr.bubble.OnSceneAnimationEnds = null;
                };
            }
            
            for (int i = 0; i <= FramesOfAnimation; i++)
            {
                Lerp = Mathf.Sin( i / (float) FramesOfAnimation * 180 * Mathf.Deg2Rad);
                for (int k = 0; k < Shifted.Count; k++)
            {
                    Shifted[k].ApplyShift(Lerp);
                    if (i == FramesHalf) Shifted[k].Reverse();
                }
                yield return Wait;
            }
            
            for (int k = Shifted.Count-1; k >= 0; k--)
            {
                Shifted[k].bubble.OnSceneAnimationEnds?.Invoke();
            }
        }
    }
}
