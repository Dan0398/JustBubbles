#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
    using TaskBool = Cysharp.Threading.Tasks.UniTask<bool>;
#else
    using Task = System.Threading.Tasks.Task;
    using TaskBool = System.Threading.Tasks.Task<bool>;
#endif
using UnityEngine;

namespace Services.Advertisements
{
    class IdleAds : AdShell
    {
        public override int BannerHeight => 0;
        
        public override void Init()
        {
            return;
        }
        
        public override void ShowBanner()
        {
            Debug.Log("RTB Banner Shown");
        }
        
        public override void HideBanner()
        {
            Debug.Log("RTB Banner Hidden");
        }
        
        public override async Task ShowInterstitial()
        {
            Debug.Log("Interstitial Shown");
            await Utilities.Wait(3000);
            return;
        }
    
        public override async TaskBool IsRewardedAdSuccess()
        {
            Debug.Log("Rewarded Ad Shown");
            await Utilities.Wait(3000);
            return true;
        }
        
        public override void Dispose()
        {
            return;
        }
    }
}