#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
    using TaskBool = Cysharp.Threading.Tasks.UniTask<bool>;
#else
    using Task = System.Threading.Tasks.Task;
    using TaskBool = System.Threading.Tasks.Task<bool>;
#endif

namespace Services.Advertisements
{
    public abstract class AdShell
    {
        protected AdStatus InterstitialStatus = AdStatus.Failed;
        protected AdStatus RewardedStatus = AdStatus.Failed;
        
        public abstract int BannerHeight {get;}
        
        public abstract void Init();
        
        public abstract void ShowBanner();
        
        public abstract void HideBanner();
        
        public abstract Task ShowInterstitial();
        
        public abstract TaskBool IsRewardedAdSuccess();
        
        public abstract void Dispose();
    }
}