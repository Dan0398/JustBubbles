#pragma warning disable CS0618 
using UnityEngine;
using Task = Cysharp.Threading.Tasks.UniTask;
using TaskBool = Cysharp.Threading.Tasks.UniTask<bool>;
using System.Runtime.InteropServices;

namespace Services.Advertisements
{
#if UNITY_WEBGL
    class WebGLAds: AdShell
    {
        public override int BannerHeight => 0;
        
        public override void Init()
        {
            Services.Web.Catcher.IsInterstitialSuccess += ApplyInterstitial;
            Services.Web.Catcher.IsRewardedSuccess += ApplyRewarded;
            Debug.Log("WebGL Ads inited in UnityEngine");
        }
        
        public override void ShowBanner()
        {
            Application.ExternalCall("Banner shown");
        }
        
        public override void HideBanner()
        {
            Application.ExternalCall("Banner hidden");
        }
        
        void ApplyInterstitial(bool isSuccess)
        {
            InterstitialStatus = isSuccess? AdStatus.Success : AdStatus.Failed;
        }
        
        void ApplyRewarded(bool isSuccess)
        {
            RewardedStatus = isSuccess? AdStatus.Success : AdStatus.Failed;
        }
        
        public override async Task ShowInterstitial()
        {
            InterstitialStatus = AdStatus.OnProcess;
            Application.ExternalCall("ShowInterstitial");
            while (InterstitialStatus == AdStatus.OnProcess)
            {
                if (await Utilities.IsWaitEndsFailure()) return;
            }
        }
        
        public override async TaskBool IsRewardedAdSuccess()
        {
            RewardedStatus = AdStatus.OnProcess;
            Application.ExternalCall("ShowRewardedAd");
            while (RewardedStatus == AdStatus.OnProcess) 
            {
                if (await Utilities.IsWaitEndsFailure()) return false;
            }
            return RewardedStatus == AdStatus.Success;
        }
        
        public override void Dispose()
        {
            return;
        }
    }
#endif
}