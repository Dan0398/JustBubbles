using System.Collections;
using UnityEngine;

namespace UI.Menu
{
    public partial class MainMenu : MonoBehaviour, Services.IService
    {
        public System.Action OnCallPause;
        [SerializeField] private RectTransform _movedWindow;
        [SerializeField] private CanvasGroup _fader;
        [SerializeField] private Gameplay.Controller _gameplay;
        [SerializeField] private Animator _endlessAnims;
        [SerializeField] private Animator _timeTrialAnims;
        [SerializeField] private Animator _strategyAnims;
        [SerializeField] private Animator _mergeAnims;
        [SerializeField] private Animator _settingsAnims;
        private Animator[] _buttonAnims;
        private bool _selectionAvailable;
        
        public void Show()
        {
            _selectionAvailable = false;
            gameObject.SetActive(true);
            StartCoroutine(AnimateWindow(false, () => _selectionAvailable = true));
        }
        
        public void CallPause()
        {
            OnCallPause.Invoke();
        }
        
        public void GoToEndlessMode()
        {
            StartCoroutine(ProcessSelect(_endlessAnims, () => _gameplay.StartEndless()));
        }
        
        public void GoToTimeTrialMode()
        {
            StartCoroutine(ProcessSelect(_timeTrialAnims, () => _gameplay.StartTimeTrial()));
        }
        
        public void GoToStrategyMode()
        {
            StartCoroutine(ProcessSelect(_strategyAnims, () => _gameplay.StartStrategy()));
        }
        
        public void GoToMergeMode()
        {
            StartCoroutine(ProcessSelect(_mergeAnims, () => _gameplay.StartMerge()));
        }
        
        private IEnumerator ProcessSelect(Animator Ignored, System.Action CustomAction)
        {
            if (!_selectionAvailable) yield break;
            _selectionAvailable = false;
            _buttonAnims ??= new Animator[]{_endlessAnims, _timeTrialAnims, _strategyAnims, _mergeAnims, _settingsAnims};
            foreach(var Anim in _buttonAnims)
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
        
        private IEnumerator AnimateWindow(bool IsHide = false, System.Action OnEnd = null)
        {
            const float UpperY = 0.55f;
            const float LowerY = 0.05f;
            const float MoveDelta = 0.1f;
            var wait = new WaitForFixedUpdate();
            for (int i = 0; i <= 25; i++)
            {
                float Lerp = Mathf.Sin(i/25f * 90 * Mathf.Deg2Rad);
                if (IsHide) Lerp = 1 - Lerp;
                _movedWindow.anchorMin = new Vector2(0, LowerY - MoveDelta * (1 - Lerp));
                _movedWindow.anchorMax = new Vector2(1, UpperY - MoveDelta * (1 - Lerp));
                _fader.alpha = Lerp;
                yield return wait;
            }
            OnEnd?.Invoke();
        }
    }
}