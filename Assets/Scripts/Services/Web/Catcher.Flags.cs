#pragma warning disable CS0618 
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        private static System.Action<int> _onInterstitialDelay;
        
        public static void RequestInterstitialDelay(System.Action<int> onResult)
        {
            _onInterstitialDelay = onResult;
            Application.ExternalCall("GiveInterstitialDelayFlag");
        }
        
        public void TranslateInterstitialDelay(string ComplexInJSON)
        {
            if (int.TryParse(ComplexInJSON, out int result))
            {
                _onInterstitialDelay?.Invoke(result);
                _onInterstitialDelay = null;
            }
        }
    }
}