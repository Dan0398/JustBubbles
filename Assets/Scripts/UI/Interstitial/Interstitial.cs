using Ads = Services.Advertisements;
using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace UI
{
    public partial class Interstitial : MonoBehaviour
    {
        [SerializeField] RectTransform AdsPlank;
        [SerializeField] MaskableGraphic FadeOnScene;
        [SerializeField] TextTMPLocalized AdsLabel;
        [SerializeField] string AdsTimerLocalized;
        Fade fade;
        Plank plank;
        [SerializeField] int oldShownTimeOnScreen;
        Gameplay.GameType.BaseType parent;
        [SerializeField] float timer;
        Ads.Controller Ads;
        Ads.Timed Timed;
        int resetTimer;
        
        bool works => resetTimer > 0;
        
        public void Show(Gameplay.GameType.BaseType endless)
        {
            parent = endless;
            gameObject.SetActive(true);
            timer = -1;
            resetTimer = -1;
            StartCoroutine(TryActivate());
        }
        
        IEnumerator TryActivate()
        {
            for(int i = 0; i < 10; i++)
            {
                if (Timed != null && Timed.RequestDone && Timed.Ready)
                {
                    resetTimer = Timed.CountBetweenShows;
                    timer = resetTimer;
                    yield break;
                }
                yield return new WaitForSeconds(3);
            }
        }
        
        void Start()
        {
            Ads = Services.DI.Single<Ads.Controller>();
            Timed = Ads.Timed;
            plank = new Plank(AdsPlank, this);
            fade = new Fade(FadeOnScene, this);
        }
        
        void FixedUpdate()
        {
            if (!works) return;
            if (parent == null) return;
            if (parent.InSettings) return;
            if (Timed.InitialTimingDone) return;
            if (timer < 0) return;
            timer -= Time.fixedUnscaledDeltaTime;
            plank.TryShowPlankByTime(timer);
            fade.TryShowFadeByTime(timer);
            TryCountdownTimer();
            if (timer <= 0) ForceShowAds();
        }
        
        void TryCountdownTimer()
        {
            if (timer > 10) return;
            if (Mathf.FloorToInt(timer) == oldShownTimeOnScreen) return;
            oldShownTimeOnScreen = Mathf.CeilToInt(timer);
            AdsLabel.SetNewKeyFormatted(AdsTimerLocalized, new string[]{ oldShownTimeOnScreen.ToString() } );
        }
        
        public async void ForceShowAds()
        {
            timer = -1;
            parent.ProcessPause();
            await Ads.ShowInterstitial();
            parent.ProcessUnpause();
            fade.Hide();
            plank.Hide();
            timer = resetTimer;
        }
        
        public void PauseParent() => parent?.ProcessPause();
        
        public async void Hide()
        {
            if (parent == null) return;
            parent = null;
            
            bool PlankReady = false;
            bool FadeReady = false;
            plank?.Hide(() => PlankReady = true);
            fade?.Hide(() => FadeReady = true);
            while (!PlankReady || !FadeReady) await Utilities.Wait();
            if (parent != null) return;
            gameObject.SetActive(false);
        }
    }
}