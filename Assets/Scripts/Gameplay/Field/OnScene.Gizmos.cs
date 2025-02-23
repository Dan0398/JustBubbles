#if UNITY_EDITOR
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        [SerializeField] private bool _draw;
        
        private void OnDrawGizmos()
        {
            if (!_draw) return;
            DrawEdges();
            DrawBubbles();
            SetupBarriers();
        }
        
        private void DrawEdges()
        {
            Gizmos.color = Color.yellow;
            Vector2 Max = (Vector2)transform.position + _fieldSize * 0.5f;
            Vector2 Min = (Vector2)transform.position - _fieldSize * 0.5f;
            Vector2 Corner = new Vector2(-_fieldSize.x, _fieldSize.y)*0.5f;
            Vector2 MinusCorner = (Vector2)transform.position - Corner;
            Corner = (Vector2)transform.position + Corner;
            Gizmos.DrawLine(Corner, Min);
            Gizmos.DrawLine(MinusCorner, Min);
            Gizmos.DrawLine(Corner, Max);
            Gizmos.DrawLine(MinusCorner, Max);
        }
        
        private void DrawBubbles()
        {
            const int DrawnLines = 6;
            RefreshFieldStats();
            Vector2 LinePoint = _startPoint;
            Vector2 ShiftDown =  BubbleSize * Mathf.Sin(60*Mathf.Deg2Rad) * Vector2.down;
            
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