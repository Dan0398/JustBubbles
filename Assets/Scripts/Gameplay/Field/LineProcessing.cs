using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public void FullCleanupAnimated(float Duration, System.Action OnEnd = null)
        {
            if (Lines.Count == 0)
            {
                OnEnd?.Invoke();
                return;
            }
            EffectsTransformMoveFrozen = true;
            StartCoroutine(AnimateLines(false, Duration, CleanLinesAndInvokeEnd));
            
            void CleanLinesAndInvokeEnd()
            {
                for (int i = Lines.Count - 1; i >= 0; i --)
                {
                    var Result = Lines[i].CleanLineAndGetCount(Pool);
                    ColorStats.DecountByBubble(Result);
                    Lines.RemoveAt(i);
                }
                LastLineShifted = true;
                OnEnd?.Invoke();
                EffectsTransformMoveFrozen = false;
            }
        }
        
        public void AppendOneLine()
        {
            //var Line = 
            CreateAndPlaceLineAtTop(true);
            //FillLineByBubbles(ref Line);
        }
        
        public void AppendLinesAndAnimate(int LinesCount, float AnimDuration, System.Action OnEnd = null)
        {
            for (int i = 0; i < LinesCount; i ++)
            {
                //AppendOneLine();
                //var Line = 
                CreateAndPlaceLineAtTop(true);
                //FillLineByBubbles(ref Line);
            }
            if (LineDownAnimation != null)
            {
                StopCoroutine(LineDownAnimation);
            }
            LineDownAnimation = StartCoroutine(AnimateLines(true, AnimDuration, OnEnd));
        }
        
        IEnumerator AnimateLines(bool AnimateDown, float Duration, System.Action OnEnd)
        {
            float Delta = Lines[0].OnScene.position.y - StartPoint.y;
            if (!AnimateDown) Delta -= Lines.Count * LineHeight;
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            Delta /= Steps;
            for (int i = 0; i < Steps; i ++)
            {
                ShiftLinesDown(Delta);
                yield return Wait;
            }
            OnEnd?.Invoke();
            LineDownAnimation = null;
        }
        
        public void ShiftLinesDown(float Scale)
        {
            var Shift = Vector3.down * Scale;
            for (int i = 0; i< Lines.Count; i++)
            {
                Lines[i].OnScene.position += Shift;
            }
            if (EffectsTransform != null && !EffectsTransformMoveFrozen)
            {
                EffectsTransform.position += Shift;
            }
            OnFieldRefreshed?.Invoke();
        }
        
        //LineOfBubbles 
        void CreateAndPlaceLineAtTop(bool RequireFillLine = false)
        {
            Vector3 Pos = StartPoint;
            if (Lines.Count > 0 && Lines[0] != null)
            {
                Pos = new Vector3(StartPoint.x, Lines[0].OnScene.transform.position.y);
                //Pos += Vector3.up * (Lines[0].OnScene.transform.position.y - Pos.y);
            }
            if (LastLineShifted) Pos += Vector3.right * ShiftWidth;
            Pos += Vector3.up * LineHeight;
            
            var Result = new LineOfBubbles(transform, LastLineShifted);
            LastLineShifted = !LastLineShifted;
            Lines.Insert(0, Result);
            Result.OnScene.position = Pos;
            if (RequireFillLine)
            {
                FillLineByBubbles(ref Result);
            }
            //return Result;
        }
        
        void FillLineByBubbles(ref LineOfBubbles Line)
        {
            var AddCountRequired = BubblesCountPerLine - Line.MaxCapacity;
            if (AddCountRequired > 0)
            {
                Bubble[] Append = new Bubble[AddCountRequired];
                for (int k=0; k< AddCountRequired; k++)
                {
                    Append[k] = GiveAndPrepareBubble();
                }
                Line.ExtendOverMax(ref Append);
                ColorStats.IncrementByBubble(Append);
            }
            else
            {
                var Trimmed = Line.GetTrimmedAfterTryTrimCapacity(BubblesCountPerLine);
                if (Trimmed.Extended)
                {
                    ColorStats.IncrementByBubble(Trimmed.Added);
                }
                else 
                {
                    ColorStats.DecountByBubble(Trimmed.Added);
                }
            }
        }
    }
}
