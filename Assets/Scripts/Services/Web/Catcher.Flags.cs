#pragma warning disable CS0618 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        static System.Action<int> OnInterstitialDelay;
        
        public static void RequestInterstitialDelay(System.Action<int> onResult)
        {
            OnInterstitialDelay = onResult;
            Application.ExternalCall("GiveInterstitialDelayFlag");
        }
        
        public void TranslateInterstitialDelay(string ComplexInJSON)
        {
            if (int.TryParse(ComplexInJSON, out int result))
            {
                OnInterstitialDelay?.Invoke(result);
                OnInterstitialDelay = null;
            }
        }
    }
}