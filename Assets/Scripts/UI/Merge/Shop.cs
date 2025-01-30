using System.Collections;
using Gameplay.Merge;
using UnityEngine;
using TMPro;

namespace UI.Merge
{   
    public class Shop : BaseAnimatedWindow
    {
        [Header("Shop components")]
        [SerializeField] TMP_Text MoneyText;
        [SerializeField] RectTransform BombBuyButton, ShakerBuyButton;
        [SerializeField] int BombCost, ShakerCost;
        Gameplay.GameType.Merge gameMode;
        Services.Audio.Sounds.Service sounds;
        System.Action OnUnbind;
        
        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(AnimateShow());
        } 
        
        IEnumerator AnimateShow()
        {
            gameMode.ProcessPause();
            SetTurnOffStatus(false);
            StartCoroutine(AnimateFade(.5f));
            StartCoroutine(AnimateHeaderColor(.2f));
            yield return AnimateHeader(.7f);
            yield return new WaitForSecondsRealtime(.2f);
            yield return AnimateUnwrapWindow(.8f);
            SetTurnOffStatus(true);
        }
        
        public void Hide() => Hide(false, null);
        
        public void Hide(bool fast, System.Action AfterEnd) => StartCoroutine(AnimateHide(fast, AfterEnd));
        
        IEnumerator AnimateHide(bool fast, System.Action AfterEnd = null)
        {
            float HeaderTime = fast? .4f: 0.8f;
            
            SetTurnOffStatus(false);
            yield return AnimateUnwrapWindow(HeaderTime, true);
            if (!fast) yield return new WaitForSecondsRealtime(.2f);
            gameMode.ProcessUnpause();
            AfterEnd?.Invoke();
            
            StartCoroutine(AnimateHeaderColor(HeaderTime, true));
            StartCoroutine(AnimateFade(HeaderTime, true));
            yield return AnimateHeader(HeaderTime, true);
            
            gameObject.SetActive(false);
        }

        internal void Bind(SaveModel selectedSaveSlot, Gameplay.GameType.Merge merge)
        {
            gameMode = merge;
            OnUnbind?.Invoke();
            OnUnbind = null;
            
            System.Action RefreshMoney = () => MoneyText.text = selectedSaveSlot.Money.Value.ToString() + '$';
            RefreshMoney.Invoke();
            selectedSaveSlot.Money.Changed += RefreshMoney;
            OnUnbind += () => selectedSaveSlot.Money.Changed -= RefreshMoney;
        }
        
        public void TryBuyBomb()
        {
            var SuccessLogic = gameMode.IsBuyBombSuccess(BombCost);
            if (SuccessLogic == null)
            {
                StartCoroutine(AnimateFailureButton(BombBuyButton));
                sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                sounds.Play(Services.Audio.Sounds.SoundType.InstrumentFail);
            }
            else
            {
                Hide(true, SuccessLogic);
            }
        }

        IEnumerator AnimateFailureButton(RectTransform target)
        {
            var Wait = new WaitForFixedUpdate();
            var offserMin = target.offsetMin;
            var offsetMax = BombBuyButton.offsetMax;
            for(int i = 0; i <= 50; i++)
            {
                float Lerp = Mathf.Sin(i/50f * 2880f * Mathf.Deg2Rad);
                var Outstand = Vector2.right * Lerp * 10;
                target.offsetMin = offserMin + Outstand;
                target.offsetMax = offsetMax + Outstand;
                yield return Wait; 
            }
        }

        public void TryBuyShaker()
        {
            var SuccessLogic = gameMode.IsBuyShakerSuccess(ShakerCost);
            if (SuccessLogic == null)
            {
                StartCoroutine(AnimateFailureButton(ShakerBuyButton));
                sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                sounds.Play(Services.Audio.Sounds.SoundType.InstrumentFail);
            }
            else
            {
                Hide(true, SuccessLogic);
            }
        }
    }
}