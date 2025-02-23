using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public void FullCleanupAnimated(float Duration, System.Action OnEnd = null)
        {
            if (_lines.Count == 0)
            {
                OnEnd?.Invoke();
                return;
            }
            _effectsTransformMoveFrozen = true;
            StartCoroutine(AnimateLines(false, Duration, CleanLinesAndInvokeEnd));
            
            void CleanLinesAndInvokeEnd()
            {
                for (int i = _lines.Count - 1; i >= 0; i --)
                {
                    var Result = _lines[i].CleanLineAndGetCount(_pool);
                    ColorStats.DecountByBubble(Result);
                    _lines.RemoveAt(i);
                }
                _lastLineShifted = true;
                OnEnd?.Invoke();
                _effectsTransformMoveFrozen = false;
            }
        }
        
        public void AppendOneLine()
        {
            CreateAndPlaceLineAtTop(true);
        }
        
        public void AppendLinesAndAnimate(int LinesCount, float AnimDuration, System.Action OnEnd = null)
        {
            for (int i = 0; i < LinesCount; i ++)
            {
                CreateAndPlaceLineAtTop(true);
            }
            if (_lineDownAnimation != null)
            {
                StopCoroutine(_lineDownAnimation);
            }
            _lineDownAnimation = StartCoroutine(AnimateLines(true, AnimDuration, OnEnd));
        }
        
        private IEnumerator AnimateLines(bool AnimateDown, float Duration, System.Action OnEnd)
        {
            float Delta = _lines[0].OnScene.position.y - _startPoint.y;
            if (!AnimateDown) Delta -= _lines.Count * _lineHeight;
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            Delta /= Steps;
            for (int i = 0; i < Steps; i ++)
            {
                ShiftLinesDown(Delta);
                yield return _wait;
            }
            OnEnd?.Invoke();
            _lineDownAnimation = null;
        }
        
        public void ShiftLinesDown(float Scale)
        {
            var Shift = Vector3.down * Scale;
            for (int i = 0; i< _lines.Count; i++)
            {
                _lines[i].OnScene.position += Shift;
            }
            if (_effectsTransform != null && !_effectsTransformMoveFrozen)
            {
                _effectsTransform.position += Shift;
            }
            OnFieldRefreshed?.Invoke();
        }
        
        private void CreateAndPlaceLineAtTop(bool RequireFillLine = false)
        {
            Vector3 Pos = _startPoint;
            if (_lines.Count > 0 && _lines[0] != null)
            {
                Pos = new Vector3(_startPoint.x, _lines[0].OnScene.transform.position.y);
            }
            if (_lastLineShifted) Pos += Vector3.right * _shiftWidth;
            Pos += Vector3.up * _lineHeight;
            
            var Result = new LineOfBubbles(transform, _lastLineShifted);
            _lastLineShifted = !_lastLineShifted;
            _lines.Insert(0, Result);
            Result.OnScene.position = Pos;
            if (RequireFillLine)
            {
                FillLineByBubbles(ref Result);
            }
        }
        
        private void FillLineByBubbles(ref LineOfBubbles Line)
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