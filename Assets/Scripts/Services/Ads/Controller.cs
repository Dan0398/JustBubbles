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
        private EnvFreezer _freezer;
        private AdShell _adSystem;
        
        public Controller()
        {
            _freezer = new EnvFreezer();
            Timed = new();
            CreateAdSystem();
        }
        
        public void ShowBanner()
        {
            _adSystem.ShowBanner();
        }
        
        public void HideBanner()
        {
            _adSystem.HideBanner();
        }
        
        public async Task ShowInterstitial() 
        {
            _freezer.RememberAndFreezeEnvironment();
            await _adSystem.ShowInterstitial();
            _freezer.RestoreEnvironment();
        }
        
        public async TaskBool IsRewardAdSuccess() 
        { 
            _freezer.RememberAndFreezeEnvironment();
            var Result = await _adSystem.IsRewardedAdSuccess();
            _freezer.RestoreEnvironment();
            return Result;
        }
    }
}