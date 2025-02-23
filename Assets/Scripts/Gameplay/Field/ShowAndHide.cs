using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public void ShowViews()
        {
            if (_viewsAnimation != null)
            {
                StopCoroutine(_viewsAnimation);
            }
            _viewsAnimation = StartCoroutine(AnimateViews(true));
        }
        
        public void HideViews()
        {
            if (_viewsAnimation != null)
            {
                StopCoroutine(_viewsAnimation);
            }
            _viewsAnimation = StartCoroutine(AnimateViews(false));
        }
        
        private IEnumerator AnimateViews(bool IsShown)
        {
            float Lerp = 0;
            Color color = Color.clear;
            for (int i=1; i<=60; i++)
            {
                Lerp = Mathf.Sin(i/60f * 90 * Mathf.Deg2Rad);
                if (!IsShown) Lerp = 1 - Lerp;
                foreach(var renderer in _fieldRenderers)
                {
                    color = renderer.color;
                    color.a = Lerp;
                    renderer.color = color;
                }
                yield return _wait;
            }
        }
    }
}