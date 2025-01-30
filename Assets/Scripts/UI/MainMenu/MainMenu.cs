using System.Collections;
using UnityEngine;

namespace UI.Menu
{
    public partial class MainMenu : MonoBehaviour, Services.IService
    {
        public System.Action OnCallPause;
        [SerializeField] RectTransform MovedWindow;
        [SerializeField] CanvasGroup Fader;
        [SerializeField] Gameplay.Controller Gameplay;
        [SerializeField] Animator EndlessAnims, TimeTrialAnims, StrategyAnims, MergeAnims, SettingsAnims;
        Animator[] ButtonAnims;
        bool selectionAvailable = false;
        
        //Появляется
        public void Show()
        {
            selectionAvailable = false;
            gameObject.SetActive(true);
            StartCoroutine(AnimateWindow(false, () => selectionAvailable = true));
        }
        
        public void CallPause() => OnCallPause.Invoke();
        
        public void GoToEndlessMode()
        {
            StartCoroutine(ProcessSelect(EndlessAnims, () => Gameplay.StartEndless()));
        }
        
        public void GoToTimeTrialMode()
        {
            StartCoroutine(ProcessSelect(TimeTrialAnims, () => Gameplay.StartTimeTrial()));
        }
        
        public void GoToStrategyMode()
        {
            StartCoroutine(ProcessSelect(StrategyAnims, () => Gameplay.StartStrategy()));
        }
        
        public void GoToMergeMode()
        {
            StartCoroutine(ProcessSelect(MergeAnims, () => Gameplay.StartMerge()));
        }
        
        IEnumerator ProcessSelect(Animator Ignored, System.Action CustomAction)
        {
            if (!selectionAvailable) yield break;
            selectionAvailable = false;
            if (ButtonAnims == null)
            {
                ButtonAnims = new Animator[]{EndlessAnims, TimeTrialAnims, StrategyAnims, MergeAnims, SettingsAnims};
            }
            foreach(var Anim in ButtonAnims)
            {
                if (Anim == Ignored) continue;
                Anim.SetTrigger("Hide");
            }
            yield return new WaitForSeconds(0.5f);
            
            Ignored.SetTrigger("Hide");
            yield return AnimateWindow(true);
            CustomAction?.Invoke();
            gameObject.SetActive(false);
        }
        
        IEnumerator AnimateWindow(bool IsHide = false, System.Action OnEnd = null)
        {
            const float UpperY = 0.55f;
            const float LowerY = 0.05f;
            const float MoveDelta = 0.1f;
            var wait = new WaitForFixedUpdate();
            for (int i = 0; i <= 25; i++)
            {
                float Lerp = Mathf.Sin(i/25f * 90 * Mathf.Deg2Rad);
                if (IsHide) Lerp = 1 - Lerp;
                MovedWindow.anchorMin = new Vector2(0, LowerY - MoveDelta * (1 - Lerp));
                MovedWindow.anchorMax = new Vector2(1, UpperY - MoveDelta * (1 - Lerp));
                Fader.alpha = Lerp;
                yield return wait;
            }
            OnEnd?.Invoke();
        }
        
        /*
        void SwitchAnimatorsStatuses(bool isMainActive, Animator ForIgnore = null)
        {
            CanvasAnims.enabled = isMainActive;
            foreach(var Anim in ButtonAnims)
            {
                if (Anim == ForIgnore) continue;
                Anim.enabled = !isMainActive;
            }
        }
        */
        
    }
}