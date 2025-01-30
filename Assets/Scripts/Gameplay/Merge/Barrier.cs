using System.Collections;
using UnityEngine;

namespace Gameplay.Merge
{
    [System.Serializable]
    public class Barrier
    {
        const float ColliderWidth = 0.3f;
        [SerializeField] Vector2 RootPoint;
        [SerializeField] float Height;
        [SerializeField] Transform Left, Right, Bottom, Top;
        [SerializeField] SpriteRenderer[] Recolor;
        [SerializeField] Color MainColor;
        Vector2 actualSize;
        float halfWidth;
        public float Width => actualSize.x;
        WaitForFixedUpdate Wait;
        
        public IEnumerator ShowAndResizeAnimated(SizeType type, End gameOverPlace, float Duration = 1f)
        {
            Wait ??= new();
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            actualSize = SizeFromType(type);
            halfWidth = actualSize.x * 0.5f;
            var ZeroSize = new Vector2(0, Height);
            var PureColor = new Color(MainColor.r, MainColor.g, MainColor.b, 0);
            for (int i = 0; i <= Steps; i++)
            {
                var Lerp = EasingFunction.EaseInSine(0,1, i/(float) Steps);
                var ShownSize = Vector2.Lerp(ZeroSize, actualSize, Lerp);
                if (Bottom != null)
                {
                    Bottom.position = RootPoint + Vector2.down * ColliderWidth * 0.5f;
                    Bottom.localScale = new Vector3(ShownSize.x + ColliderWidth * 2, ColliderWidth);
                    Bottom.gameObject.SetActive(true);
                }
                if (Top != null)
                {
                    Top.position = RootPoint + Vector2.up * (actualSize.y - ColliderWidth * 0.5f);
                    Top.localScale = new Vector3(ShownSize.x + ColliderWidth * 2, ColliderWidth);
                    Top.gameObject.SetActive(true);
                }
                if (Left != null)
                {
                    Left.position = new Vector3(RootPoint.x - (ShownSize.x + ColliderWidth) * 0.5f, RootPoint.y + ShownSize.y * 0.5f);
                    Left.localScale = new Vector3(ColliderWidth, ShownSize.y);
                    Left.gameObject.SetActive(true);
                }
                if (Right != null)
                {
                    Right.position = new Vector3(RootPoint.x + (ShownSize.x + ColliderWidth) * 0.5f, RootPoint.y + ShownSize.y * 0.5f);
                    Right.localScale = new Vector3(ColliderWidth, ShownSize.y);
                    Right.gameObject.SetActive(true);
                }
                foreach(var recolor in Recolor)
                {
                    recolor.color = Color.Lerp(PureColor, MainColor, Lerp);
                }
                gameOverPlace.RefreshView(ShownSize.x, Lerp);
                yield return Wait;
            }
        }
        
        public enum SizeType : byte
        {
            Slim = 0,
            Quad = 1,
            Wide = 2,
        }
        
        Vector2 SizeFromType(SizeType type)
        {
            if (type == SizeType.Slim)
            {
                return new Vector2(5, Height);
            }
            else if (type == SizeType.Quad)
            {
                return new Vector2(Height, Height);
            }
            else if (type == SizeType.Wide)
            {
                return new Vector2(15, Height);
            }
            throw new System.Exception("Barrier size not found");
        }

        internal bool IsPosInside(Vector2 input)
        {
            return  input.x > RootPoint.x - halfWidth &&
                    input.x < RootPoint.x + halfWidth &&
                    input.y > RootPoint.y             &&
                    input.x < RootPoint.y + actualSize.y;
        }

        internal void Hide()
        {
            Bottom?.gameObject.SetActive(false);
            Left?.gameObject.SetActive(false);
            Right?.gameObject.SetActive(false);
            Top?.gameObject.SetActive(false);
        }
        
        public IEnumerator Shake()
        {
            Wait ??= new();
            float Width = Bottom.localScale.x;
            Vector3[] LocalDefault = new Vector3[] { Left.position, Right.position, Bottom.position, Top.position};
            Rigidbody2D[] rigids = new Rigidbody2D[] { Left.GetComponent<Rigidbody2D>(), Right.GetComponent<Rigidbody2D>(), Bottom.GetComponent<Rigidbody2D>(), Top.GetComponent<Rigidbody2D>() };
            foreach(var rigid in rigids) rigid.bodyType = RigidbodyType2D.Kinematic;
            for (int i = 0; i <= 60; i++)
            {
                float Lerp = Mathf.Sin(i/60f * 1440 * Mathf.Deg2Rad);
                for (int k = 0; k < rigids.Length; k++)
                {
                    rigids[k].MovePosition(LocalDefault[k] + Lerp * Width * 0.25f * Vector3.right);
                }
                yield return Wait;
            }
            foreach(var rigid in rigids) rigid.bodyType = RigidbodyType2D.Static;
        }
    }
}