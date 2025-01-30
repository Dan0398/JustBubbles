using System.Collections;
using UnityEngine;

namespace UI
{
    public partial class Interstitial : MonoBehaviour
    {
        public class Plank
        {
            const float NeedShowAtTime = 10;
            const int MaxSteps = 50;

            byte animationStep;
            Coroutine animationRoutine;
            RectTransform onScene;
            Interstitial parent;
            WaitForFixedUpdate wait;
            bool shown;

            public Plank(RectTransform plankOnScene, Interstitial parent)
            {
                onScene = plankOnScene;
                this.parent = parent;
                wait = new();
            }
            
            public void TryShowPlankByTime(float time)
            {
                if (time > NeedShowAtTime) return;
                if (shown) return;
                if (animationRoutine != null) parent.StopCoroutine(animationRoutine);
                animationRoutine = parent.StartCoroutine(ShowPlank());
                
                IEnumerator ShowPlank()
                {
                    shown = true;
                    onScene.gameObject.SetActive(true);
                    while(animationStep < MaxSteps)
                    {
                        animationStep++;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, animationStep / (float) MaxSteps);
                        UpdateRectByStep(Lerp);
                        yield return wait;
                    }
                }
            }
            
            public void Hide(System.Action value = null)
            {
                if(!shown)
                {
                    value?.Invoke();
                    return;
                }
                if (animationRoutine != null) parent.StopCoroutine(animationRoutine);
                animationRoutine = parent.StartCoroutine(HidePlank());
            
                IEnumerator HidePlank()
                {
                    shown = false;
                    while(animationStep > 0)
                    {
                        animationStep--;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, animationStep / (float) MaxSteps);
                        UpdateRectByStep(Lerp);
                        yield return wait;
                    }
                    onScene.gameObject.SetActive(false);
                }
            }
            
            void UpdateRectByStep(float Lerp)
            {
                onScene.anchorMin = new Vector2(.5f, Mathf.Lerp(1, 0.93f, Lerp));
                onScene.anchorMax = new Vector2(.5f, Mathf.Lerp(1.07f, 1, Lerp));
                onScene.offsetMin = new Vector2(-360, 0);
                onScene.offsetMax = new Vector2(360, 0);
            }
        }
    }
}