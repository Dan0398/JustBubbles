using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public partial class Interstitial : MonoBehaviour
    {
        public class Fade
        {
            const float NeedShowAtTime = 2;
            const byte MaxSteps = 20;
            Button Clickable;
            MaskableGraphic onScene;
            Interstitial parent;
            WaitForFixedUpdate wait;
            Coroutine animationRoutine;
            bool shown;
            byte animationStep;

            public Fade(MaskableGraphic OnScene, Interstitial Parent)
            {
                onScene = OnScene;
                Clickable = onScene.GetComponent<Button>();
                parent = Parent;
                wait = new();
            }
            
            public void TryShowFadeByTime(float time)
            {
                if (time > NeedShowAtTime) return;
                if (shown) return;
                if (animationRoutine != null) parent.StopCoroutine(animationRoutine);
                animationRoutine = parent.StartCoroutine(ShowFade());
                
                IEnumerator ShowFade()
                {
                    shown = true;
                    onScene.enabled = true;
                    Clickable.enabled = true;
                    parent.PauseParent();
                    while(animationStep < MaxSteps)
                    {
                        animationStep++;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, animationStep / (float) MaxSteps);
                        onScene.color = new Color(0, 0, 0, .7f * Lerp);
                        yield return wait;
                    }
                }
            }
            
            public void Hide(System.Action value = null)
            {
                if (!shown)
                {
                    value?.Invoke();
                    return;
                }
                if (animationRoutine != null) parent.StopCoroutine(animationRoutine);
                animationRoutine = parent.StartCoroutine(HideFade());
                
                IEnumerator HideFade()
                {
                    shown = false;
                    Clickable.enabled = false;
                    while(animationStep > 0)
                    {
                        animationStep--;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, animationStep / (float) MaxSteps);
                        onScene.color = new Color(0, 0, 0, .7f * Lerp);
                        yield return wait;
                    }
                    onScene.enabled = false;
                }
            }
        }
    }
}