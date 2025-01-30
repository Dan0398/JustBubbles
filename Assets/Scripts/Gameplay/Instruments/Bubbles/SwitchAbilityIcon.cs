using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public class SwitchAbilityIcon : MonoBehaviour
    {
        [SerializeField] GameObject PCHelpIcon;
        bool isUserUseSwitchAbility;
        Coroutine animationRoutine;
        WaitForFixedUpdate Wait = new();
        
        public void ReactOnEnvironment(bool IsPC)
        {
            PCHelpIcon.SetActive(IsPC);
        }
        
        public void TryShowHelpAnimated(float Duration)
        {
            if (isUserUseSwitchAbility) return;
            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animationRoutine = StartCoroutine(AnimateIcons(Duration));
        }
        
        IEnumerator AnimateIcons(float Duration, bool isShow = true)
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
                yield return Wait;
            }
        }
        
        public void ReceiveUserSwitched(float AnimationDuration = 1f)
        {
            if (isUserUseSwitchAbility) return;
            HideNonSwitched(AnimationDuration);
            isUserUseSwitchAbility = true;
        }
        
        public void HideNonSwitched(float AnimationDuration)
        {
            if (isUserUseSwitchAbility) return;
            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animationRoutine = StartCoroutine(AnimateIcons(AnimationDuration, false));
        }
    }
}