using Ads = Services.Advertisements;
using BrakelessGames.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public partial class Interstitial : MonoBehaviour
    {
        [SerializeField] private RectTransform _adsPlank;
        [SerializeField] private MaskableGraphic _fadeOnScene;
        [SerializeField] private TextTMPLocalized _adsLabel;
        [SerializeField] private string _adsTimerLocalized;
        [SerializeField] private int _oldShownTimeOnScreen;
        [SerializeField] private float _timer;
        private Fade _fade;
        private Plank _plank;
        private Gameplay.GameType.BaseType _parent;
        private Ads.Controller _ads;
        private Ads.Timed _timed;
        private int _resetTimer;
        
        private bool Works => _resetTimer > 0;
        
        public void Show(Gameplay.GameType.BaseType endless)
        {
            _parent = endless;
            gameObject.SetActive(true);
            _timer = -1;
            _resetTimer = -1;
            StartCoroutine(TryActivate());
        }
        
        private IEnumerator TryActivate()
        {
            for(int i = 0; i < 10; i++)
            {
                if (_timed != null && _timed.RequestDone && _timed.Ready)
                {
                    _resetTimer = _timed.CountBetweenShows;
                    _timer = _resetTimer;
                    yield break;
                }
                yield return new WaitForSeconds(3);
            }
        }
        
        private void Start()
        {
            _ads = Services.DI.Single<Ads.Controller>();
            _timed = _ads.Timed;
            _plank = new Plank(_adsPlank, this);
            _fade = new Fade(_fadeOnScene, this);
        }
        
        private void FixedUpdate()
        {
            if (!Works) return;
            if (_parent == null) return;
            if (_parent.InSettings) return;
            if (_timed.InitialTimingDone) return;
            if (_timer < 0) return;
            _timer -= Time.fixedUnscaledDeltaTime;
            _plank.TryShowPlankByTime(_timer);
            _fade.TryShowFadeByTime(_timer);
            TryCountdownTimer();
            if (_timer <= 0) ForceShowAds();
        }
        
        private void TryCountdownTimer()
        {
            if (_timer > 10) return;
            if (Mathf.FloorToInt(_timer) == _oldShownTimeOnScreen) return;
            _oldShownTimeOnScreen = Mathf.CeilToInt(_timer);
            _adsLabel.SetNewKeyFormatted(_adsTimerLocalized, new string[]{ _oldShownTimeOnScreen.ToString() } );
        }
        
        public async void ForceShowAds()
        {
            _timer = -1;
            _parent.ProcessPause();
            await _ads.ShowInterstitial();
            _parent.ProcessUnpause();
            _fade.Hide();
            _plank.Hide();
            _timer = _resetTimer;
        }
        
        public void PauseParent()
        {
            _parent?.ProcessPause();
        }
        
        public async void Hide()
        {
            if (_parent == null) return;
            _parent = null;
            
            bool PlankReady = false;
            bool FadeReady = false;
            _plank?.Hide(() => PlankReady = true);
            _fade?.Hide(() => FadeReady = true);
            while (!PlankReady || !FadeReady) await Utilities.Wait();
            if (_parent != null) return;
            gameObject.SetActive(false);
        }
    }
}