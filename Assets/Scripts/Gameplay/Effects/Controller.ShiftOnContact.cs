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
            public Bubble Bubble { get; private set;}
            private bool _reversed;
            private Vector3 _oldPosInLocal, _endPlaceInLocal, _shiftedPlaceInLocal;
            private Transform _bubbleTransform;
            
            private ShiftPackage(Bubble bubble, float multiplier, Vector3 direction, float zOutstand)
            {
                Bubble = bubble;
                _reversed = false;
                _bubbleTransform = bubble.MyTransform;
                _shiftedPlaceInLocal = _bubbleTransform.localPosition + multiplier * WorldAffect * direction - 0.1f * zOutstand * Vector3.forward;
                _endPlaceInLocal = bubble.LocalPosInLine;
            }
            
            public ShiftPackage(Bubble bubble, Vector3 Dir, float Multiplier, float ZOutstand): this(bubble, Multiplier, Dir, ZOutstand)
            {
                _oldPosInLocal = _bubbleTransform.parent.InverseTransformPoint(_bubbleTransform.position);
            }
            
            public ShiftPackage(Bubble bubble, Vector3 Dir, float Multiplier, Vector3 WorldPos, float ZOutstand): this(bubble, Multiplier, Dir, ZOutstand)
            {
                _oldPosInLocal = _bubbleTransform.parent.InverseTransformPoint(WorldPos);
                _shiftedPlaceInLocal = _endPlaceInLocal + Dir * Multiplier * WorldAffect - Vector3.forward * 0.1f * ZOutstand;
            }
            
            public void ApplyShift(float Scale)
            {
                if (!_reversed)
                {
                    _bubbleTransform.localPosition = Vector3.Lerp(_oldPosInLocal, _shiftedPlaceInLocal, Scale);
                }
                else 
                {
                    _bubbleTransform.localPosition = Vector3.Lerp(_endPlaceInLocal, _shiftedPlaceInLocal, Scale);
                }
            }
            
            public void Reverse() => _reversed = true;
        }
    
        public void ShiftBubblesAfterContact(Bubble Central, List<Bubble> ConnectedBubbles, Vector3 ConnectionDirection, Vector3 WorldPos, float MaxDistance)
        {
            _sounds.PlayBubbleSet();
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
        
        private IEnumerator AnimateShiftAfterContact(List<ShiftPackage> Shifted)
        {
            const int FramesOfAnimation = 20;
            const int FramesHalf = 10;
            
            float Lerp = 0;
            for (int k = 0; k < Shifted.Count; k++)
            {
                var Curr = Shifted[k];
                Curr.Bubble.OnSceneAnimationEnds?.Invoke();
                Curr.Bubble.OnSceneAnimationEnds = () => 
                {
                    Curr.ApplyShift(0);
                    Shifted.Remove(Curr);
                    Curr.Bubble.OnSceneAnimationEnds = null;
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
                yield return _wait;
            }
            
            for (int k = Shifted.Count-1; k >= 0; k--)
            {
                Shifted[k].Bubble.OnSceneAnimationEnds?.Invoke();
            }
        }
    }
}
