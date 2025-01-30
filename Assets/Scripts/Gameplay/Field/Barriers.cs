using UnityEngine;

namespace Gameplay.Field
{
    [System.Serializable]
    public class Barriers
    {
        public const float SizeWidth = 0.3f;
        [SerializeField] BoxCollider2D LeftBarrier, RightBarrier, TopBarrier;
        [SerializeField] RectTransform BackgroundRect;
        
        public User.CollisionType TryResponseCollision(Collider2D col)
        {
            if (LeftBarrier != null && col == LeftBarrier)
            {
                return User.CollisionType.LeftBarrier;
            }
            if (RightBarrier != null && col == RightBarrier)
            {
                return User.CollisionType.RightBarrier;
            }
            if (TopBarrier != null && col == TopBarrier)
            {
                return User.CollisionType.IntoBunch;
                //return User.CollisionType.TopBarrier;
            }
            return User.CollisionType.IntoBunch;
        }
        
        public void SetupBarriers(Vector2 FieldSize, float UpperOutstand)
        {
            var Size = new Vector2(SizeWidth, FieldSize.y + SizeWidth);
            var XOffset = (FieldSize.x + SizeWidth) * 0.5f;
            if (LeftBarrier != null)
            {
                LeftBarrier.transform.localPosition = new Vector3(-XOffset, (SizeWidth) * 0.5f, 0.1f);
                LeftBarrier.transform.localScale = Size;
            }
            if (RightBarrier != null)
            {
                RightBarrier.transform.localPosition = new Vector3(XOffset, (SizeWidth) * 0.5f, 0.1f);
                RightBarrier.transform.localScale = Size;
            }
            if (TopBarrier != null)
            {
                TopBarrier.transform.localPosition = new Vector3(0, (FieldSize.y + SizeWidth- UpperOutstand) * 0.5f , 0.1f);
                TopBarrier.transform.localScale = new Vector3(FieldSize.x, SizeWidth + UpperOutstand);
            }
            if (BackgroundRect != null)
            {
                BackgroundRect.sizeDelta = FieldSize;
            }
        }
    }
}