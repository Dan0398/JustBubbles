using System.Collections;
using UnityEngine;

namespace UI
{
    public partial class Interstitial : MonoBehaviour
    {
        public class Plank
        {
            private const float NeedShowAtTime = 10;
            private const int MaxSteps = 50;

            private byte _animationStep;
            private Coroutine _animationRoutine;
            private RectTransform _onScene;
            private Interstitial _parent;
            private WaitForFixedUpdate _wait;
            private bool _shown;

            public Plank(RectTransform plankOnScene, Interstitial parent)
            {
                _onScene = plankOnScene;
                this._parent = parent;
                _wait = new();
            }
            
            public void TryShowPlankByTime(float time)
            {
                if (time > NeedShowAtTime) return;
                if (_shown) return;
                if (_animationRoutine != null) _parent.StopCoroutine(_animationRoutine);
                _animationRoutine = _parent.StartCoroutine(ShowPlank());
                
                IEnumerator ShowPlank()
                {
                    _shown = true;
                    _onScene.gameObject.SetActive(true);
                    while(_animationStep < MaxSteps)
                    {
                        _animationStep++;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, _animationStep / (float) MaxSteps);
                        UpdateRectByStep(Lerp);
                        yield return _wait;
                    }
                }
            }
            
            public void Hide(System.Action value = null)
            {
                if(!_shown)
                {
                    value?.Invoke();
                    return;
                }
                if (_animationRoutine != null) _parent.StopCoroutine(_animationRoutine);
                _animationRoutine = _parent.StartCoroutine(HidePlank());
            
                IEnumerator HidePlank()
                {
                    _shown = false;
                    while(_animationStep > 0)
                    {
                        _animationStep--;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, _animationStep / (float) MaxSteps);
                        UpdateRectByStep(Lerp);
                        yield return _wait;
                    }
                    _onScene.gameObject.SetActive(false);
                }
            }
            
            private void UpdateRectByStep(float Lerp)
            {
                _onScene.anchorMin = new Vector2(.5f, Mathf.Lerp(1, 0.93f, Lerp));
                _onScene.anchorMax = new Vector2(.5f, Mathf.Lerp(1.07f, 1, Lerp));
                _onScene.offsetMin = new Vector2(-360, 0);
                _onScene.offsetMax = new Vector2(360, 0);
            }
        }
    }
}