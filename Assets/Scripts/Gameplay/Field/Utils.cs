using System;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public int BubblesCountOnScene => ColorStats.FullCount.Value;
        
        public bool IsPlaceOnTopEmpty => Lines[0].OnScene.position.y <  StartPoint.y;//(StartPoint.y - FieldSize.y * UpperRelativePlace);
        
        public int LineCount => Lines.Count;
        
        public void TryFilterColor(ref Bubble bubble)
        {
            if (ColorStats.AvailableColors.Contains(bubble.MyColor)) return;
            if (ColorStats.AvailableColors.Count == 0) return;
            bubble.RandomizeColor(ColorStats.AvailableColors);
        }
        
        public Bubble GiveAndPrepareBubble()
        {
            var BubbleObj = Pool.GiveItem();
            BubbleObj.SetSize(BubbleSize);
            BubbleObj.ActivateCollisions();
            BubbleObj.MyRigid.isKinematic = true;
            BubbleObj.OnScene.layer = 0;
            BubbleObj.RandomizeColor(ColorStats.AvailableColors, Difficulty);
            return BubbleObj;
        }
        
        public bool IsPositionInsideField(Vector2 mouseWorldPos)
        {
            Vector2 Min = new Vector2(transform.position.x - FieldSize.x*0.5f, transform.position.y + FieldSize.y * 0.5f - FieldUsableSpace);
            Vector2 Max = (Vector2)transform.position + FieldSize * 0.5f;
            return  mouseWorldPos.x >= Min.x && 
                    mouseWorldPos.y >= Min.y && 
                    mouseWorldPos.x <= Max.x && 
                    mouseWorldPos.y <= Max.y;
        }
        
        public User.CollisionType TryResponseCollision(Collider2D col) => barriers.TryResponseCollision(col);
        
        public float GetDistanceToFieldEdge()
        {
            if (Lines.Count == 0) return 1;
            var Result = Lines[Lines.Count-1].OnScene.position.y - StartPoint.y - FieldSize.y * UpperRelativePlace + FieldUsableSpace - BubbleSize;
            //Result /= LineHeight;
            return Result;
        }
        
        public float GetRelativeDistanceToFieldEdge() => GetDistanceToFieldEdge()/ (FieldUsableSpace - FieldSize.y * UpperRelativePlace);
        
        public bool IsLowerLineUnderFieldEdge() => GetDistanceToFieldEdge() < 0;
        
        public void SetColorConfig(int Count, bool RequireFilter)
        {
            if (!Started) Start();
            ColorStats.RequireFilter = RequireFilter;
            ColorStats.AvailableColors = ColorPicker.GiveColorsByCount(Count);
        }
    }
}