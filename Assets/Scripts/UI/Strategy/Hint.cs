using BrakelessGames.Localization;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Strategy
{
    public class Hint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler
    {
        private const int MaxSteps = 40;
        [SerializeField] private TextTMPLocalized _text;
        [SerializeField] private GameObject _window;
        [SerializeField] private Image _mask;
        [SerializeField] private int _step;
        private bool _maskShown, _pointerInside, _isPCLogic;
        private Button _interactable;
        private WaitForFixedUpdate _wait;
        private Coroutine _maskAnimation;

#region  PC_Callbacks
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isPCLogic) return;
            IncrementStep(MaxSteps);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isPCLogic) return;
            _pointerInside = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isPCLogic) return;
            Stop();
        }
#endregion
        
#region  TouchCallbacks
        public void OnDeselect(BaseEventData eventData)
        {
            if (_isPCLogic) return;
            Stop();
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            if (_isPCLogic) return;
            IncrementStep(MaxSteps);
        }
#endregion

        private void Start()
        {
            _interactable = GetComponent<Button>();
            var Env = Services.DI.Single<Services.Environment>();
            System.Action Refresh = () => 
            {
                _isPCLogic = !Env.IsUsingTouch.Value;
                _interactable.navigation = new Navigation()
                {
                    mode = _isPCLogic? Navigation.Mode.None : Navigation.Mode.Automatic
                };
            };
            Refresh.Invoke();
            Env.IsUsingTouch.Changed += Refresh;
        }

        private void FixedUpdate()
        {
            if (!_pointerInside) return;
            IncrementStep(1);
        }
        
        private void IncrementStep(int Amount)
        {
            if (_maskShown) return;
            _step += Amount;
            if (_step >= MaxSteps)
            {
                _maskShown = true;
                _maskAnimation = StartCoroutine(FillMask());
            }
        }
        
        private void Stop()
        {
            _pointerInside = false;
            _step = 0;
            _maskShown = false;
            _mask.fillAmount = 0;
            if (_maskAnimation != null)
            {
                StopCoroutine(_maskAnimation);
            }
            
        }
        
        public void Hide()
        {
            _window.SetActive(false);
            gameObject.SetActive(false);
        }

        public void Show(string InfoLangKey)
        {
            if (string.IsNullOrEmpty(InfoLangKey))
            {
                Hide();
                return;
            }
            Stop();
            _text.SetNewKey(InfoLangKey);
            _window.SetActive(true);
            gameObject.SetActive(true);
        }
        
        private IEnumerator FillMask()
        {
            _wait ??= new WaitForFixedUpdate();
            for (int i = 0; i <= 25; i++)
            {
                _mask.fillAmount = Mathf.Sin(i/25f * 90f * Mathf.Deg2Rad);
                yield return _wait;
            }
        }
    }
}