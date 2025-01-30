#if (UNITY_ANDROID || UNITY_IOS)
using System.Threading.Tasks;
using UnityEngine.Advertisements;

namespace Services.Advertisements
{
    public abstract class MobileAds : AdShell, IUnityAdsInitializationListener, IUnityAdsShowListener
    {
        bool BannerRequired     = false;
        bool fullSizeShown      = false;
        bool fullSizeEndSuccess = true;
        
        protected abstract string AppID                 { get; }
        protected abstract string InterstitialPlaceName { get; }
        protected abstract string RewardedPlaceName     { get; }
        protected abstract string BannerPlaceName       { get; }
        
        public override int BannerHeight => 0;

        public override void Dispose() { }

        public override void Init()
        {
            bool TestMode = false;
        #if UNITY_EDITOR
            TestMode = true;
        #endif
            Advertisement.Initialize(AppID, TestMode, this);
        }

        public void OnInitializationComplete() 
        {
            ShowBanner();
            Advertisement.Load(InterstitialPlaceName);
            Advertisement.Load(RewardedPlaceName);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            if (error == UnityAdsInitializationError.AD_BLOCKER_DETECTED) return;
            Init();
        }

        public override void ShowBanner()
        {
            Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
            BannerRequired = true;
            BannerLoadOptions AfterLoad = null;
            System.Action StartLoadBanner = () => Advertisement.Banner.Load(BannerPlaceName, AfterLoad);
            AfterLoad = new BannerLoadOptions()
            {
                loadCallback = () => 
                { 
                    if (!BannerRequired) return;
                    Advertisement.Banner.Show(BannerPlaceName);
                },
                errorCallback = (s) => StartLoadBanner.Invoke()
            };
            StartLoadBanner.Invoke();
        }
        
        public override void HideBanner()
        {
            BannerRequired = false;
            Advertisement.Banner.Hide();
        }

        public override async Task ShowInterstitial()
        {
            fullSizeShown = true;
            Advertisement.Show(InterstitialPlaceName, this);
            while (fullSizeShown) if (await Utilities.IsWaitEndsFailure()) return;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            fullSizeShown = false;
            fullSizeEndSuccess = false;
        }

        public void OnUnityAdsShowStart(string placementId) { }

        public void OnUnityAdsShowClick(string placementId) { }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            fullSizeShown = false;
            fullSizeEndSuccess = true;
        }
        
        public override async Task<bool> IsRewardedAdSuccess()
        {
            fullSizeShown = true;
            Advertisement.Show(RewardedPlaceName, this);
            while (fullSizeShown) if (await Utilities.IsWaitEndsFailure()) return false;
            return fullSizeEndSuccess;
        }
    }
}
#endif