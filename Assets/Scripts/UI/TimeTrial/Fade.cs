#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace UI.Survival
{
    [System.Serializable]
    public class Fade: MonoBehaviour
    {
        private const float FinalAlphaScale = 0.7f;
        private const int AnimationSteps = 30;
        [SerializeField] private Image _fadeView;
        private int _step = 0;
        
        public void Show()
        {
            gameObject.SetActive(true);
            AnimateFade(true);
        }
        
        public void Hide()
        {
            AnimateFade(false);
        }
        
        private async void AnimateFade(bool isShowFade)
        {
            float Lerp = 0;
            if (isShowFade) _fadeView.enabled = true;
            while(true)
            {
                Lerp = Mathf.Sin(90 * (_step / (float) AnimationSteps) * Mathf.Deg2Rad);
                _fadeView.color = Color.black * FinalAlphaScale * Lerp;
                if (isShowFade)
                {
                    if (_step == AnimationSteps) break;
                    _step ++;
                }
                else 
                {
                    if (_step == 0) break;
                    _step--;
                }
                await Task.Yield();
            }
            if (!isShowFade) _fadeView.enabled = false;
        }
    }
}