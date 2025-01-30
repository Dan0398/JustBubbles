using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public void ShowViews()
        {
            if (ViewsAnimation != null)
            {
                StopCoroutine(ViewsAnimation);
            }
            ViewsAnimation = StartCoroutine(AnimateViews(true));
        }
        
        public void HideViews()
        {
            if (ViewsAnimation != null)
            {
                StopCoroutine(ViewsAnimation);
            }
            ViewsAnimation = StartCoroutine(AnimateViews(false));
        }
        
        IEnumerator AnimateViews(bool IsShown)
        {
            float Lerp = 0;
            Color color = Color.clear;
            for (int i=1; i<=60; i++)
            {
                Lerp = Mathf.Sin(i/60f * 90 * Mathf.Deg2Rad);
                if (!IsShown) Lerp = 1 - Lerp;
                foreach(var renderer in FieldRenderers)
                {
                    color = renderer.color;
                    color.a = Lerp;
                    renderer.color = color;
                }
                yield return Wait;
            }
        }
    }
}