using BrakelessGames.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Endless
{
    [RequireComponent(typeof(Animator))]
    public class EndlessCanvas : MonoBehaviour
    {
        [SerializeField] private UI.Interstitial _interstitial;
        [SerializeField] private RequestByAds _adsRequest;
        [SerializeField] private Image _rewardCircleFill;
        [Header("Bonus")]
        [SerializeField] private Image _bonusView;
        [SerializeField] private TextTMPLocalized _bonusName;
        [SerializeField] private TextTMPLocalized _bonusUserReact;
        private System.Action _onDispose;
        private Gameplay.GameType.Endless _parent;
        private System.Func<bool> _afterRewardFill;
        private Coroutine _rewardRoutine, _requestRewardRoutine;
        private float _rewardNewAmount;
        private WaitForFixedUpdate _wait = new();
        private float _reservedAmount;

        public void CallPause() => _parent.CallSettings();
        
        public void ShowRewardFill(float NewAmount, System.Func<bool> AfterEnd, float ReservedAmount = 0)
        {
            _afterRewardFill = AfterEnd;
            _rewardNewAmount = NewAmount;
            _reservedAmount = ReservedAmount;
            if (_rewardRoutine != null) StopCoroutine(_rewardRoutine);
            GetComponent<Animator>().SetTrigger("ShowReward");
        }
        
        public void RegisterRewardShown()
        {
            _rewardRoutine = StartCoroutine(AnimateRewardFill(_rewardCircleFill.fillAmount, _rewardNewAmount, AfterFill));
            
            IEnumerator AnimateRewardFill(float oldAmount, float newAmount, System.Action OnEnd)
            {
                const float Steps = 20;
                for (int i = 1; i <= Steps; i++)
                {
                    _rewardCircleFill.fillAmount = Mathf.Lerp(oldAmount, newAmount, Mathf.Sin(90 * i/(float)Steps * Mathf.Deg2Rad));
                    yield return _wait;
                }
                yield return new WaitForSecondsRealtime(0.5f);
                OnEnd.Invoke();
            }
            
            void AfterFill()
            {
                if (_afterRewardFill == null)
                {
                    HideGift();
                    return;
                }
                var isFull = _afterRewardFill.Invoke();
                _bonusUserReact.SetNewKey(isFull? "Max" : "Received");
                _afterRewardFill = null;
                _rewardRoutine = StartCoroutine(AnimateRewardFill(0, _reservedAmount, HideGift));
            }
            
            void HideGift() => GetComponent<Animator>().SetTrigger("HideReward");
        }
        
        public void Show(Gameplay.GameType.Endless Parent)
        {
            _parent = Parent;
            gameObject.SetActive(true);
            _interstitial.Show(Parent);
        }

        public void Hide()
        {
            GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void RegisterAfterHide()
        {
            gameObject.SetActive(false);
        }

        public void ShowClaimedReward(Content.Instrument.Config.InstrumentView shown)
        {
            if (shown == null) return;
            _bonusView.sprite = shown.Sprite;
            _bonusName.SetNewKey(shown.NameLangKey);
            GetComponent<Animator>().SetTrigger("ShowBonus");
        }
        
        public void ReactOnFailUseInstrument(Content.Instrument.Config.InstrumentView Data, System.Action OnActivate)
        {
            if (_requestRewardRoutine != null) StopCoroutine(_requestRewardRoutine);
            _requestRewardRoutine = StartCoroutine(StartRequestRoutine());
            
            IEnumerator StartRequestRoutine()
            {
                yield return new WaitForSecondsRealtime(0.6f);
                _adsRequest.TurnOn(Data, OnActivate);
            }
        }
        
        public void HideAdsRequest()
        {
            _adsRequest.StartTurnOff();
        }
        
        public void RegisterAfterHideAdsRequest()
        {
            _adsRequest.TurnOff();
        }

        public void Dispose()
        {
            _onDispose?.Invoke();
            _onDispose = null;
            _interstitial.Hide();
        }
    }
}