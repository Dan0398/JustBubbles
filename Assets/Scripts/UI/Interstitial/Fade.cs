using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public partial class Interstitial : MonoBehaviour
    {
        public class Fade
        {
            private const float NeedShowAtTime = 2;
            private const byte MaxSteps = 20;
            private Button _clickable;
            private MaskableGraphic _onScene;
            private Interstitial _parent;
            private WaitForFixedUpdate _wait;
            private Coroutine _animationRoutine;
            private bool _shown;
            private byte _animationStep;

            public Fade(MaskableGraphic OnScene, Interstitial Parent)
            {
                _onScene = OnScene;
                _clickable = _onScene.GetComponent<Button>();
                _parent = Parent;
                _wait = new();
            }
            
            public void TryShowFadeByTime(float time)
            {
                if (time > NeedShowAtTime) return;
                if (_shown) return;
                if (_animationRoutine != null) _parent.StopCoroutine(_animationRoutine);
                _animationRoutine = _parent.StartCoroutine(ShowFade());
                
                IEnumerator ShowFade()
                {
                    _shown = true;
                    _onScene.enabled = true;
                    _clickable.enabled = true;
                    _parent.PauseParent();
                    while(_animationStep < MaxSteps)
                    {
                        _animationStep++;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, _animationStep / (float) MaxSteps);
                        _onScene.color = new Color(0, 0, 0, .7f * Lerp);
                        yield return _wait;
                    }
                }
            }
            
            public void Hide(System.Action value = null)
            {
                if (!_shown)
                {
                    value?.Invoke();
                    return;
                }
                if (_animationRoutine != null) _parent.StopCoroutine(_animationRoutine);
                _animationRoutine = _parent.StartCoroutine(HideFade());
                
                IEnumerator HideFade()
                {
                    _shown = false;
                    _clickable.enabled = false;
                    while(_animationStep > 0)
                    {
                        _animationStep--;
                        float Lerp = EasingFunction.EaseInOutSine(0, 1, _animationStep / (float) MaxSteps);
                        _onScene.color = new Color(0, 0, 0, .7f * Lerp);
                        yield return _wait;
                    }
                    _onScene.enabled = false;
                }
            }
        }
    }
}