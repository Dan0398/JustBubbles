#if UNITY_WEBGL
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        public static System.Action<bool> IsInterstitialSuccess;
        public static System.Action<bool> IsRewardedSuccess;
        
        public void RewardedAdSuccess()
        {
            IsRewardedSuccess?.Invoke(true);
        }
        
        public void RewardedAdFailed()
        {
            IsRewardedSuccess?.Invoke(false);
        }
        
        public void InterstitialSuccess()
        {
            IsInterstitialSuccess?.Invoke(true);
        }
        
        public void InterstitialFailed()
        {
            IsInterstitialSuccess?.Invoke(false);
        }
    }
}
#endif