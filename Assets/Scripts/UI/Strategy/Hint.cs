using BrakelessGames.Localization;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Strategy
{
    public class Hint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler
    {
        const int MaxSteps = 40;
        [SerializeField] TextTMPLocalized Text;
        [SerializeField] GameObject Window;
        [SerializeField] Image Mask;
        [SerializeField] int step;
        bool maskShown, pointerInside, isPCLogic;
        Button Interactable;
        WaitForFixedUpdate Wait;
        Coroutine MaskAnimation;

#region  PC_Callbacks
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isPCLogic) return;
            IncrementStep(MaxSteps);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isPCLogic) return;
            pointerInside = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isPCLogic) return;
            Stop();
        }
#endregion
        
#region  TouchCallbacks
        public void OnDeselect(BaseEventData eventData)
        {
            if (isPCLogic) return;
            Stop();
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            if (isPCLogic) return;
            IncrementStep(MaxSteps);
        }
#endregion

        void Start()
        {
            Interactable = GetComponent<Button>();
            var Env = Services.DI.Single<Services.Environment>();
            System.Action Refresh = () => 
            {
                isPCLogic = !Env.IsUsingTouch.Value;
                Interactable.navigation = new Navigation()
                {
                    mode = isPCLogic? Navigation.Mode.None : Navigation.Mode.Automatic
                };
            };
            Refresh.Invoke();
            Env.IsUsingTouch.Changed += Refresh;
        }

        void FixedUpdate()
        {
            if (!pointerInside) return;
            IncrementStep(1);
        }
        
        void IncrementStep(int Amount)
        {
            if (maskShown) return;
            step += Amount;
            if (step >= MaxSteps)
            {
                maskShown = true;
                MaskAnimation = StartCoroutine(FillMask());
            }
        }
        
        void Stop()
        {
            pointerInside = false;
            step = 0;
            maskShown = false;
            Mask.fillAmount = 0;
            if (MaskAnimation != null)
            {
                StopCoroutine(MaskAnimation);
            }
            
        }
        
        public void Hide()
        {
            Window.SetActive(false);
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
            Text.SetNewKey(InfoLangKey);
            Window.SetActive(true);
            gameObject.SetActive(true);
        }
        
        IEnumerator FillMask()
        {
            if (Wait == null) Wait = new WaitForFixedUpdate();
            for (int i = 0; i <= 25; i++)
            {
                Mask.fillAmount = Mathf.Sin(i/25f * 90f * Mathf.Deg2Rad);
                yield return Wait;
            }
        }
    }
}