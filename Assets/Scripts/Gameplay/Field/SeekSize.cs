using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
#if UNITY_WEBGL
        public static float MinAspectRatio => 0.6f;
#else 
        public static float MinAspectRatio => 9/16f;
#endif
        private bool _isFieldSizeDynamic;
        private float _maxAspectRatio;
        private int _checkStep = 30;
        private float _oldAspectRatio;
        private Coroutine _fieldAspectAnim;

        public void ResetAspect()
        {
            _oldAspectRatio = -1;
        }
        
        public void TryCheckAspectChange()
        {
            if (!_isFieldSizeDynamic) return;
            _checkStep ++;
            if (_checkStep >= 30) _checkStep = 0;
            if (_checkStep == 0)
            {
                CheckAspectChange();
            }
        }
        
        public void CheckAspectChange()
        {
            var Aspect = Mathf.Clamp(Screen.width / (float) (Screen.height * (1 - UpperRelativePlace)), MinAspectRatio, _maxAspectRatio);
            if (Aspect == _oldAspectRatio) return;
            _oldAspectRatio = Aspect;
            SetAspect(Aspect);
        }
        
        public void SetAspect(float AspectRatio)
        {
            var OldSize = _fieldSize;
            var oldBubblesCount = BubblesCountPerLine;
            BubblesCountPerLine = GimmeBubbleCount((_fieldSize.y * AspectRatio));
            _fieldSize = new Vector2(BubbleSize * BubblesCountPerLine, _fieldSize.y);
            _inGameCanvas.ReactOnFieldResize(_fieldSize.x/_fieldSize.y);
            RefreshFieldStats();
            SetupBarriers();
            if (_fieldAspectAnim != null)
            {
                StopCoroutine(_fieldAspectAnim);
            }
            _fieldAspectAnim = StartCoroutine(AnimateAspectChange(OldSize));
            SetBubblesToEmptyPlaces(oldBubblesCount);
        }
        
        private IEnumerator AnimateAspectChange(Vector2 OldSize)
        {
            _lines ??= new List<LineOfBubbles>();
            var linesUnderShift = new List<LineOfBubbles>();
            linesUnderShift.AddRange(_lines);
            int Count = _lines.Count;
            
            float[] OldPos = new float[Count];
            float[] NewPos = new float[Count];
            for (int k = 0; k < Count; k++)
            {
                OldPos[k] = linesUnderShift[k].OnScene.position.x;
                NewPos[k] = _startPoint.x + (linesUnderShift[k].Shifted? _shiftWidth:0);
            }
            float Lerp = 0;
            for (int i = 0; i <= 30; i++)
            {
                Lerp = i/30f;
                for (int k=0; k< Count; k++)
                {
                    linesUnderShift[k].OnScene.position = new Vector3(Mathf.Lerp(OldPos[k], NewPos[k], Lerp),  linesUnderShift[k].OnScene.position.y, 0);
                }
                SetupBarriers(Vector2.Lerp(OldSize, _fieldSize, Lerp));
                yield return _wait;
            }
        }
        
        private void SetBubblesToEmptyPlaces(int OldSize)
        {
            for (int i=0; i< _lines.Count; i++)
            {
                var Line = _lines[i];
                FillLineByBubbles(ref Line);
            }
        }
        
        private int GimmeBubbleCount()
        {
            return GimmeBubbleCount(_fieldSize.x);
        }
        
        private int GimmeBubbleCount(float XSize)
        {
            return Mathf.FloorToInt((XSize - _shiftWidth) / (float)BubbleSize);
        }
        
        private void RefreshFieldStats()
        {
            _lineHeight = BubbleSize * Mathf.Sin(60 * Mathf.Deg2Rad);
            _shiftWidth = BubbleSize * 0.5f;
            BubblesCountPerLine = GimmeBubbleCount();
            RefreshStartPoint();
        }
        
        private void RefreshStartPoint()
        {
            float UsefulWidth = BubblesCountPerLine * BubbleSize + _shiftWidth;
            _startPoint = (Vector2)transform.position + new Vector2(-UsefulWidth * 0.5f, _fieldSize.y * 0.5f);
            _startPoint += new Vector2(_shiftWidth, - _shiftWidth);
            _startPoint += _fieldSize.y * UpperRelativePlace * Vector2.down;
        }
        
        private void SetupBarriers() => SetupBarriers(new Vector2(BubblesCountPerLine * BubbleSize + _shiftWidth, _fieldSize.y));
        
        private void SetupBarriers(Vector2 fieldSize)
        {
            _barriers.SetupBarriers(fieldSize, fieldSize.y * UpperRelativePlace);
            if (_endLine != null)
            {
                _endLine.localPosition = new Vector3(0, fieldSize.y * 0.5f - _fieldUsableSpace, 0.5f);
                _endLine.localScale = new Vector3(fieldSize.x + Barriers.SizeWidth * 2, 0.025f);
            }
            if (_background != null)
            {
                _background.sizeDelta = new Vector2(_fieldSize.y * (Screen.width / (float)Screen.height), _fieldSize.y);
            }
        }
    }
}