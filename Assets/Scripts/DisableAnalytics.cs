#if !UNITY_EDITOR
using UnityEngine;

public class DisableAnalytics
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void BeforeSplashScreen()
    {
        UnityEngine.Analytics.PerformanceReporting.enabled = false;
    }
}
#endif
