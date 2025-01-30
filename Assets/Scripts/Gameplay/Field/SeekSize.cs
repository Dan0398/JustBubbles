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
        bool IsFieldSizeDynamic;
        float MaxAspectRatio;
        int CheckStep = 30;
        float oldAspectRatio;
        Coroutine FieldAspectAnim;

        internal void ResetAspect() => oldAspectRatio = -1;
        
        public void TryCheckAspectChange()
        {
            if (!IsFieldSizeDynamic) return;
            CheckStep ++;
            if (CheckStep >= 30) CheckStep = 0;
            if (CheckStep == 0)
            {
                CheckAspectChange();
            }
        }
        
        public void CheckAspectChange()
        {
            var Aspect = Mathf.Clamp(Screen.width / (float) (Screen.height * (1 - UpperRelativePlace)), MinAspectRatio, MaxAspectRatio);
            if (Aspect == oldAspectRatio) return;
            oldAspectRatio = Aspect;
            SetAspect(Aspect);
        }
        
        public void SetAspect(float AspectRatio)
        {
            var OldSize = FieldSize;
            var oldBubblesCount = BubblesCountPerLine;
            //AspectRatio = Mathf.Clamp(AspectRatio, MinAspectRatio, MaxAspectRatio);
            BubblesCountPerLine = GimmeBubbleCount((FieldSize.y * AspectRatio));
            FieldSize = new Vector2(BubbleSize * BubblesCountPerLine, FieldSize.y);
            inGameCanvas.ReactOnFieldResize(FieldSize.x/FieldSize.y);
            RefreshFieldStats();
            SetupBarriers();
            if (FieldAspectAnim != null)
            {
                StopCoroutine(FieldAspectAnim);
            }
            FieldAspectAnim = StartCoroutine(AnimateAspectChange(OldSize));
            SetBubblesToEmptyPlaces(oldBubblesCount);
        }
        
        IEnumerator AnimateAspectChange(Vector2 OldSize)
        {
            Lines ??= new List<LineOfBubbles>();
            var linesUnderShift = new List<LineOfBubbles>();
            linesUnderShift.AddRange(Lines);
            int Count = Lines.Count;
            
            float[] OldPos = new float[Count];
            float[] NewPos = new float[Count];
            for (int k = 0; k < Count; k++)
            {
                OldPos[k] = linesUnderShift[k].OnScene.position.x;
                NewPos[k] = StartPoint.x + (linesUnderShift[k].Shifted? ShiftWidth:0);
            }
            float Lerp = 0;
            for (int i = 0; i <= 30; i++)
            {
                Lerp = i/30f;
                for (int k=0; k< Count; k++)
                {
                    linesUnderShift[k].OnScene.position = new Vector3(Mathf.Lerp(OldPos[k], NewPos[k], Lerp),  linesUnderShift[k].OnScene.position.y, 0);
                }
                SetupBarriers(Vector2.Lerp(OldSize, FieldSize, Lerp));
                yield return Wait;
            }
        }
        
        void SetBubblesToEmptyPlaces(int OldSize)
        {
            for (int i=0; i< Lines.Count; i++)
            {
                var Line = Lines[i];
                FillLineByBubbles(ref Line);
            }
        }
        
        int GimmeBubbleCount() => GimmeBubbleCount(FieldSize.x);
        
        int GimmeBubbleCount(float XSize) => Mathf.FloorToInt((XSize - ShiftWidth) / (float)BubbleSize);
        
        void RefreshFieldStats()
        {
            LineHeight = BubbleSize * Mathf.Sin(60 * Mathf.Deg2Rad);
            ShiftWidth = BubbleSize * 0.5f;
            BubblesCountPerLine = GimmeBubbleCount();
            RefreshStartPoint();
        }
        
        void RefreshStartPoint()
        {
            float UsefulWidth = BubblesCountPerLine * BubbleSize + ShiftWidth;
            StartPoint = (Vector2)transform.position + new Vector2(-UsefulWidth * 0.5f, FieldSize.y * 0.5f);
            StartPoint += new Vector2(ShiftWidth, - ShiftWidth);
            StartPoint += FieldSize.y * UpperRelativePlace * Vector2.down;
        }
        
        void SetupBarriers() => SetupBarriers(new Vector2(BubblesCountPerLine * BubbleSize + ShiftWidth, FieldSize.y));
        
        void SetupBarriers(Vector2 fieldSize)
        {
            barriers.SetupBarriers(fieldSize, fieldSize.y * UpperRelativePlace);
            if (EndLine != null)
            {
                EndLine.localPosition = new Vector3(0, fieldSize.y * 0.5f - FieldUsableSpace, 0.5f);
                EndLine.localScale = new Vector3(fieldSize.x + Barriers.SizeWidth * 2, 0.025f);
            }
            if (Background != null)
            {
                Background.sizeDelta = new Vector2(FieldSize.y * (Screen.width / (float)Screen.height), FieldSize.y);
            }
        }
    }
}