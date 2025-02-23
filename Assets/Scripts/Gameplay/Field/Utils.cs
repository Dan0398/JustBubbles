using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public int BubblesCountOnScene => ColorStats.FullCount.Value;
        
        public bool IsPlaceOnTopEmpty => _lines[0].OnScene.position.y <  _startPoint.y;
        
        public int LineCount => _lines.Count;
        
        public void TryFilterColor(ref Bubble bubble)
        {
            if (ColorStats.AvailableColors.Contains(bubble.MyColor)) return;
            if (ColorStats.AvailableColors.Count == 0) return;
            bubble.RandomizeColor(ColorStats.AvailableColors);
        }
        
        public Bubble GiveAndPrepareBubble()
        {
            var BubbleObj = _pool.GiveItem();
            BubbleObj.SetSize(BubbleSize);
            BubbleObj.ActivateCollisions();
            BubbleObj.MyRigid.isKinematic = true;
            BubbleObj.OnScene.layer = 0;
            BubbleObj.RandomizeColor(ColorStats.AvailableColors, Difficulty);
            return BubbleObj;
        }
        
        public bool IsPositionInsideField(Vector2 mouseWorldPos)
        {
            Vector2 Min = new(transform.position.x - _fieldSize.x*0.5f, transform.position.y + _fieldSize.y * 0.5f - _fieldUsableSpace);
            Vector2 Max = (Vector2)transform.position + _fieldSize * 0.5f;
            return  mouseWorldPos.x >= Min.x && 
                    mouseWorldPos.y >= Min.y && 
                    mouseWorldPos.x <= Max.x && 
                    mouseWorldPos.y <= Max.y;
        }
        
        public User.CollisionType TryResponseCollision(Collider2D col) => _barriers.TryResponseCollision(col);
        
        public float GetDistanceToFieldEdge()
        {
            if (_lines.Count == 0) return 1;
            var Result = _lines[_lines.Count-1].OnScene.position.y - _startPoint.y - _fieldSize.y * UpperRelativePlace + _fieldUsableSpace - BubbleSize;
            return Result;
        }
        
        public float GetRelativeDistanceToFieldEdge() => GetDistanceToFieldEdge()/ (_fieldUsableSpace - _fieldSize.y * UpperRelativePlace);
        
        public bool IsLowerLineUnderFieldEdge() => GetDistanceToFieldEdge() < 0;
        
        public void SetColorConfig(int Count, bool RequireFilter)
        {
            if (!_started) Start();
            ColorStats.RequireFilter = RequireFilter;
            ColorStats.AvailableColors = ColorPicker.GiveColorsByCount(Count);
        }
    }
}