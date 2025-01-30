using BrakelessGames.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Endless
{
    [RequireComponent(typeof(Animator))]
    public class EndlessCanvas : MonoBehaviour
    {
        const int MinimalViewTimeAds = 10;
        [SerializeField] UI.Interstitial Interstitial;
        [SerializeField] RequestByAds AdsRequest;
        [SerializeField] Image RewardCircleFill;
        [Header("Bonus")]
        [SerializeField] Image BonusView;
        [SerializeField] TextTMPLocalized BonusName, BonusUserReact;
        System.Action OnDispose;
        Gameplay.GameType.Endless parent;
        System.Func<bool> AfterRewardFill;
        Coroutine RewardRoutine, RequestRewardRoutine;
        float RewardNewAmount;
        WaitForFixedUpdate Wait = new();
        private float ReservedAmount;

        public void CallPause() => parent.CallSettings();
        
        public void ShowRewardFill(float NewAmount, System.Func<bool> AfterEnd, float ReservedAmount = 0)
        {
            AfterRewardFill = AfterEnd;
            RewardNewAmount = NewAmount;
            this.ReservedAmount = ReservedAmount;
            if (RewardRoutine != null) StopCoroutine(RewardRoutine);
            GetComponent<Animator>().SetTrigger("ShowReward");
        }
        
        public void RegisterRewardShown()
        {
            RewardRoutine = StartCoroutine(AnimateRewardFill(RewardCircleFill.fillAmount, RewardNewAmount, AfterFill));
            
            IEnumerator AnimateRewardFill(float oldAmount, float newAmount, System.Action OnEnd)
            {
                const float Steps = 20;
                for (int i = 1; i <= Steps; i++)
                {
                    RewardCircleFill.fillAmount = Mathf.Lerp(oldAmount, newAmount, Mathf.Sin(90 * i/(float)Steps * Mathf.Deg2Rad));
                    yield return Wait;
                }
                yield return new WaitForSecondsRealtime(0.5f);
                OnEnd.Invoke();
            }
            
            void AfterFill()
            {
                if (AfterRewardFill == null)
                {
                    HideGift();
                    return;
                }
                var isFull = AfterRewardFill.Invoke();
                BonusUserReact.SetNewKey(isFull? "Max" : "Received");
                AfterRewardFill = null;
                RewardRoutine = StartCoroutine(AnimateRewardFill(0, ReservedAmount, HideGift));
            }
            
            void HideGift() => GetComponent<Animator>().SetTrigger("HideReward");
        }
        
        public void Show(Gameplay.GameType.Endless Parent)
        {
            parent = Parent;
            gameObject.SetActive(true);
            Interstitial.Show(Parent);
        }

        internal void Hide()
        {
            GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void RegisterAfterHide()
        {
            gameObject.SetActive(false);
        }

        internal void ShowClaimedReward(Content.Instrument.Config.InstrumentView shown)
        {
            if (shown == null) return;
            BonusView.sprite = shown.Sprite;
            BonusName.SetNewKey(shown.NameLangKey);
            GetComponent<Animator>().SetTrigger("ShowBonus");
        }
        
        public void ReactOnFailUseInstrument(Content.Instrument.Config.InstrumentView Data, System.Action OnActivate)
        {
            if (RequestRewardRoutine != null) StopCoroutine(RequestRewardRoutine);
            RequestRewardRoutine = StartCoroutine(StartRequestRoutine());
            
            IEnumerator StartRequestRoutine()
            {
                yield return new WaitForSecondsRealtime(0.6f);
                AdsRequest.TurnOn(Data, OnActivate);
            }
        }
        public void HideAdsRequest() => AdsRequest.StartTurnOff();
        
        public void RegisterAfterHideAdsRequest() => AdsRequest.TurnOff();

        internal void Dispose()
        {
            OnDispose?.Invoke();
            OnDispose = null;
            Interstitial.Hide();
        }
    }
}