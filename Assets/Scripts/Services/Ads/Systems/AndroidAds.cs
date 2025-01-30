#if UNITY_ANDROID
namespace Services.Advertisements
{
    public class AndroidAds: MobileAds
    {
        protected override string AppID                 => "5284443";
        protected override string InterstitialPlaceName => "Interstitial_Android";
        protected override string RewardedPlaceName     => "Rewarded_Android";
        protected override string BannerPlaceName       => "Banner_Android";
    }
}
#endif