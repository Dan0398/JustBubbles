using System.Collections;
using UnityEngine;

namespace Gameplay.Merge
{
    [System.Serializable]
    public class Barrier
    {
        private const float ColliderWidth = 0.3f;
        
        [SerializeField] private Vector2 _rootPoint;
        [SerializeField] private float _height;
        [SerializeField] private Transform _left;
        [SerializeField] private Transform _right;
        [SerializeField] private Transform _bottom;
        [SerializeField] private Transform _top;
        [SerializeField] private SpriteRenderer[] _recolor;
        [SerializeField] private Color _mainColor;
        private Vector2 _actualSize;
        private float _halfWidth;
        private WaitForFixedUpdate _wait;
        
        public float Width => _actualSize.x;
        
        public IEnumerator ShowAndResizeAnimated(SizeType type, End gameOverPlace, float Duration = 1f)
        {
            _wait ??= new();
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            _actualSize = SizeFromType(type);
            _halfWidth = _actualSize.x * 0.5f;
            var ZeroSize = new Vector2(0, _height);
            var PureColor = new Color(_mainColor.r, _mainColor.g, _mainColor.b, 0);
            for (int i = 0; i <= Steps; i++)
            {
                var Lerp = EasingFunction.EaseInSine(0,1, i/(float) Steps);
                var ShownSize = Vector2.Lerp(ZeroSize, _actualSize, Lerp);
                if (_bottom != null)
                {
                    _bottom.position = _rootPoint + Vector2.down * ColliderWidth * 0.5f;
                    _bottom.localScale = new Vector3(ShownSize.x + ColliderWidth * 2, ColliderWidth);
                    _bottom.gameObject.SetActive(true);
                }
                if (_top != null)
                {
                    _top.position = _rootPoint + Vector2.up * (_actualSize.y - ColliderWidth * 0.5f);
                    _top.localScale = new Vector3(ShownSize.x + ColliderWidth * 2, ColliderWidth);
                    _top.gameObject.SetActive(true);
                }
                if (_left != null)
                {
                    _left.position = new Vector3(_rootPoint.x - (ShownSize.x + ColliderWidth) * 0.5f, _rootPoint.y + ShownSize.y * 0.5f);
                    _left.localScale = new Vector3(ColliderWidth, ShownSize.y);
                    _left.gameObject.SetActive(true);
                }
                if (_right != null)
                {
                    _right.position = new Vector3(_rootPoint.x + (ShownSize.x + ColliderWidth) * 0.5f, _rootPoint.y + ShownSize.y * 0.5f);
                    _right.localScale = new Vector3(ColliderWidth, ShownSize.y);
                    _right.gameObject.SetActive(true);
                }
                foreach(var recolor in _recolor)
                {
                    recolor.color = Color.Lerp(PureColor, _mainColor, Lerp);
                }
                gameOverPlace.RefreshView(ShownSize.x, Lerp);
                yield return _wait;
            }
        }
        
        private Vector2 SizeFromType(SizeType type)
        {
            if (type == SizeType.Slim)
            {
                return new Vector2(5, _height);
            }
            else if (type == SizeType.Quad)
            {
                return new Vector2(_height, _height);
            }
            else if (type == SizeType.Wide)
            {
                return new Vector2(15, _height);
            }
            throw new System.Exception("Barrier size not found");
        }

        public bool IsPosInside(Vector2 input)
        {
            return  input.x > _rootPoint.x - _halfWidth &&
                    input.x < _rootPoint.x + _halfWidth &&
                    input.y > _rootPoint.y             &&
                    input.x < _rootPoint.y + _actualSize.y;
        }

        public void Hide()
        {
            _bottom?.gameObject.SetActive(false);
            _left?.gameObject.SetActive(false);
            _right?.gameObject.SetActive(false);
            _top?.gameObject.SetActive(false);
        }
        
        public IEnumerator Shake()
        {
            _wait ??= new();
            float Width = _bottom.localScale.x;
            Vector3[] LocalDefault = new Vector3[] { _left.position, _right.position, _bottom.position, _top.position};
            Rigidbody2D[] rigids = new Rigidbody2D[] { _left.GetComponent<Rigidbody2D>(), _right.GetComponent<Rigidbody2D>(), _bottom.GetComponent<Rigidbody2D>(), _top.GetComponent<Rigidbody2D>() };
            foreach(var rigid in rigids) rigid.bodyType = RigidbodyType2D.Kinematic;
            for (int i = 0; i <= 60; i++)
            {
                float Lerp = Mathf.Sin(i/60f * 1440 * Mathf.Deg2Rad);
                for (int k = 0; k < rigids.Length; k++)
                {
                    rigids[k].MovePosition(LocalDefault[k] + Lerp * Width * 0.25f * Vector3.right);
                }
                yield return _wait;
            }
            foreach(var rigid in rigids) rigid.bodyType = RigidbodyType2D.Static;
        }
    }
}