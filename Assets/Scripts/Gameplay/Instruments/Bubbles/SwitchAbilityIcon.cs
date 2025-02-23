using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public class SwitchAbilityIcon : MonoBehaviour
    {
        [SerializeField] private GameObject _pcHelpIcon;
        private bool _isUserUseSwitchAbility;
        private Coroutine _animationRoutine;
        private WaitForFixedUpdate _wait = new();
        
        public void ReactOnEnvironment(bool IsPC)
        {
            _pcHelpIcon.SetActive(IsPC);
        }
        
        public void TryShowHelpAnimated(float Duration)
        {
            if (_isUserUseSwitchAbility) return;
            if (_animationRoutine != null) StopCoroutine(_animationRoutine);
            _animationRoutine = StartCoroutine(AnimateIcons(Duration));
        }
        
        private IEnumerator AnimateIcons(float Duration, bool isShow = true)
        {
            float Steps = Duration / Time.fixedDeltaTime;
            var HelpRenderers = GetComponentsInChildren<SpriteRenderer>();
            for (int Step = 0; Step <= Steps; Step++)
            {
                float Lerp = Step / Steps;
                if (!isShow) Lerp = 1 - Lerp;
                foreach(var Renderer in HelpRenderers)
                {
                    Renderer.color = Color.white - Color.black * (1 - Lerp);
                }
                yield return _wait;
            }
        }
        
        public void ReceiveUserSwitched(float AnimationDuration = 1f)
        {
            if (_isUserUseSwitchAbility) return;
            HideNonSwitched(AnimationDuration);
            _isUserUseSwitchAbility = true;
        }
        
        public void HideNonSwitched(float AnimationDuration)
        {
            if (_isUserUseSwitchAbility) return;
            if (_animationRoutine != null) StopCoroutine(_animationRoutine);
            _animationRoutine = StartCoroutine(AnimateIcons(AnimationDuration, false));
        }
    }
}