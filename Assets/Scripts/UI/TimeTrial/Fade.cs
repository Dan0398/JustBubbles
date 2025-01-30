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
        const float FinalAlphaScale = 0.7f;
        const int AnimationSteps = 30;
        [SerializeField] Image FadeView;
        WaitForEndOfFrame Wait = new WaitForEndOfFrame();
        int Step = 0;
        
        public void Show()
        {
            gameObject.SetActive(true);
            AnimateFade(true);
        }
        
        public void Hide()
        {
            AnimateFade(false);
        }
        
        async void AnimateFade(bool isShowFade)
        {
            float Lerp = 0;
            if (isShowFade) FadeView.enabled = true;
            while(true)
            {
                Lerp = Mathf.Sin(90 * (Step / (float) AnimationSteps) * Mathf.Deg2Rad);
                FadeView.color = Color.black * FinalAlphaScale * Lerp;
                if (isShowFade)
                {
                    if (Step == AnimationSteps) break;
                    Step ++;
                }
                else 
                {
                    if (Step == 0) break;
                    Step--;
                }
                await Task.Yield();
            }
            if (!isShowFade) FadeView.enabled = false;
        }
    }
}