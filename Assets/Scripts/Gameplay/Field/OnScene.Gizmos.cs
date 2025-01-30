#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        [SerializeField] bool Draw;
        void OnDrawGizmos()
        {
            if (!Draw) return;
            DrawEdges();
            DrawBubbles();
            SetupBarriers();
        }
        
        void DrawEdges()
        {
            Gizmos.color = Color.yellow;
            Vector2 Max = (Vector2)transform.position + FieldSize * 0.5f;
            Vector2 Min = (Vector2)transform.position - FieldSize * 0.5f;
            Vector2 Corner = new Vector2(-FieldSize.x, FieldSize.y)*0.5f;
            Vector2 MinusCorner = (Vector2)transform.position - Corner;
            Corner = (Vector2)transform.position + Corner;
            Gizmos.DrawLine(Corner, Min);
            Gizmos.DrawLine(MinusCorner, Min);
            Gizmos.DrawLine(Corner, Max);
            Gizmos.DrawLine(MinusCorner, Max);
        }
        
        void DrawBubbles()
        {
            const int DrawnLines = 6;
            RefreshFieldStats();
            Vector2 LinePoint = StartPoint;
            Vector2 ShiftDown =  Vector2.down * BubbleSize * Mathf.Sin(60*Mathf.Deg2Rad);
            /*
            LinePoint += (FieldSize.x - (BubblesCountPerLine * BubbleSize)) * 0.5f * Vector2.right;
            LinePoint += ShiftDown * 0.5f;
            */
            
            Gizmos.color = Color.green;
            for (int Line = 0; Line < DrawnLines; Line ++)
            {
                Vector2 BubblePoint = LinePoint;
                if (Line % 2 == 1)
                {
                    BubblePoint += Vector2.right * BubbleSize * 0.5f;
                }
                for (int Bubble=0; Bubble < BubblesCountPerLine; Bubble++)
                {
                    Gizmos.DrawSphere(BubblePoint, BubbleSize*0.5f);
                    BubblePoint += Vector2.right * BubbleSize;
                }
                LinePoint += ShiftDown;
            }
        }
    }
}
#endif