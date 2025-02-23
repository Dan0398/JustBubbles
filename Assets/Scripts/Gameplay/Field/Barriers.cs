using UnityEngine;

namespace Gameplay.Field
{
    [System.Serializable]
    public class Barriers
    {
        public const float SizeWidth = 0.3f;
        [SerializeField] BoxCollider2D _leftBarrier;
        [SerializeField] BoxCollider2D _rightBarrier;
        [SerializeField] BoxCollider2D _topBarrier;
        [SerializeField] RectTransform _backgroundRect;
        
        public User.CollisionType TryResponseCollision(Collider2D col)
        {
            if (_leftBarrier != null && col == _leftBarrier)
            {
                return User.CollisionType.LeftBarrier;
            }
            if (_rightBarrier != null && col == _rightBarrier)
            {
                return User.CollisionType.RightBarrier;
            }
            if (_topBarrier != null && col == _topBarrier)
            {
                return User.CollisionType.IntoBunch;
            }
            return User.CollisionType.IntoBunch;
        }
        
        public void SetupBarriers(Vector2 FieldSize, float UpperOutstand)
        {
            var Size = new Vector2(SizeWidth, FieldSize.y + SizeWidth);
            var XOffset = (FieldSize.x + SizeWidth) * 0.5f;
            if (_leftBarrier != null)
            {
                _leftBarrier.transform.localPosition = new Vector3(-XOffset, SizeWidth * 0.5f, 0.1f);
                _leftBarrier.transform.localScale = Size;
            }
            if (_rightBarrier != null)
            {
                _rightBarrier.transform.localPosition = new Vector3(XOffset, SizeWidth * 0.5f, 0.1f);
                _rightBarrier.transform.localScale = Size;
            }
            if (_topBarrier != null)
            {
                _topBarrier.transform.localPosition = new Vector3(0, (FieldSize.y + SizeWidth- UpperOutstand) * 0.5f , 0.1f);
                _topBarrier.transform.localScale = new Vector3(FieldSize.x, SizeWidth + UpperOutstand);
            }
            if (_backgroundRect != null)
            {
                _backgroundRect.sizeDelta = FieldSize;
            }
        }
    }
}