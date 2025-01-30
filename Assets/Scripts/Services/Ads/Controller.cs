using Utils.Observables;
#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
    using TaskBool = Cysharp.Threading.Tasks.UniTask<bool>;
#else
    using Task = System.Threading.Tasks.Task;
    using TaskBool = System.Threading.Tasks.Task<bool>;
#endif

namespace Services.Advertisements 
{
    public partial class Controller : Services.IService
    {   
        public ObsInt BannerActualShift { get; private set; } = 0;
        public readonly Timed Timed;
        EnvFreezer Freezer;
        AdShell AdSystem;
        
        public Controller()
        {
            Freezer = new EnvFreezer();
            Timed = new();
            CreateAdSystem();
        }
        
        public void ShowBanner() => AdSystem.ShowBanner();
        public void HideBanner() => AdSystem.HideBanner();
        
        public async Task ShowInterstitial() 
        {
            Freezer.RememberAndFreezeEnvironment();
            await AdSystem.ShowInterstitial();
            Freezer.RestoreEnvironment();
        }
        
        public async TaskBool IsRewardAdSuccess() 
        { 
            Freezer.RememberAndFreezeEnvironment();
            var Result = await AdSystem.IsRewardedAdSuccess();
            Freezer.RestoreEnvironment();
            return Result;
        }
    }
}