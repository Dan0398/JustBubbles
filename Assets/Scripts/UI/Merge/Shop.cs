using System.Collections;
using Gameplay.Merge;
using UnityEngine;
using TMPro;

namespace UI.Merge
{   
    public class Shop : BaseAnimatedWindow
    {
        [Header("Shop components")]
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private RectTransform _bombBuyButton;
        [SerializeField] private RectTransform _shakerBuyButton;
        [SerializeField] private int _bombCost;
        [SerializeField] private int _shakerCost;
        private Gameplay.GameType.Merge _gameMode;
        private Services.Audio.Sounds.Service _sounds;
        private System.Action _onUnbind;
        
        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(AnimateShow());
        } 
        
        private IEnumerator AnimateShow()
        {
            _gameMode.ProcessPause();
            SetTurnOffStatus(false);
            StartCoroutine(AnimateFade(.5f));
            StartCoroutine(AnimateHeaderColor(.2f));
            yield return AnimateHeader(.7f);
            yield return new WaitForSecondsRealtime(.2f);
            yield return AnimateUnwrapWindow(.8f);
            SetTurnOffStatus(true);
        }
        
        public void Hide()
        {
            Hide(false, null);
        }
        
        public void Hide(bool fast, System.Action AfterEnd)
        {
            StartCoroutine(AnimateHide(fast, AfterEnd));
        }
        
        private IEnumerator AnimateHide(bool fast, System.Action AfterEnd = null)
        {
            float HeaderTime = fast? .4f: 0.8f;
            
            SetTurnOffStatus(false);
            yield return AnimateUnwrapWindow(HeaderTime, true);
            if (!fast) yield return new WaitForSecondsRealtime(.2f);
            _gameMode.ProcessUnpause();
            AfterEnd?.Invoke();
            
            StartCoroutine(AnimateHeaderColor(HeaderTime, true));
            StartCoroutine(AnimateFade(HeaderTime, true));
            yield return AnimateHeader(HeaderTime, true);
            
            gameObject.SetActive(false);
        }

        public void Bind(SaveModel selectedSaveSlot, Gameplay.GameType.Merge merge)
        {
            _gameMode = merge;
            _onUnbind?.Invoke();
            _onUnbind = null;
            
            System.Action RefreshMoney = () => _moneyText.text = selectedSaveSlot.Money.Value.ToString() + '$';
            RefreshMoney.Invoke();
            selectedSaveSlot.Money.Changed += RefreshMoney;
            _onUnbind += () => selectedSaveSlot.Money.Changed -= RefreshMoney;
        }
        
        public void TryBuyBomb()
        {
            var SuccessLogic = _gameMode.IsBuyBombSuccess(_bombCost);
            if (SuccessLogic == null)
            {
                StartCoroutine(AnimateFailureButton(_bombBuyButton));
                _sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                _sounds.Play(Services.Audio.Sounds.SoundType.InstrumentFail);
            }
            else
            {
                Hide(true, SuccessLogic);
            }
        }

        private IEnumerator AnimateFailureButton(RectTransform target)
        {
            var Wait = new WaitForFixedUpdate();
            var offserMin = target.offsetMin;
            var offsetMax = _bombBuyButton.offsetMax;
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
            var SuccessLogic = _gameMode.IsBuyShakerSuccess(_shakerCost);
            if (SuccessLogic == null)
            {
                StartCoroutine(AnimateFailureButton(_shakerBuyButton));
                _sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                _sounds.Play(Services.Audio.Sounds.SoundType.InstrumentFail);
            }
            else
            {
                Hide(true, SuccessLogic);
            }
        }
    }
}