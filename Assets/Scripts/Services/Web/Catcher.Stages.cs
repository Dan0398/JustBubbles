#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        public static void SendLoadingEnds()
        {
            Application.ExternalCall("SubmitLoadingEnd");
        }
        
        public static void SendGameplayStart()
        {
            var Env = Services.DI.Single<Services.Environment>();
            if (!Env.RequireSendGameplayStatus.Value) return;
            Application.ExternalCall("SubmitGameplayStart");
        }
        
        public static void SendGameplayEnd()
        {
            var Env = Services.DI.Single<Services.Environment>();
            if (!Env.RequireSendGameplayStatus.Value) return;
            Application.ExternalCall("SubmitGameplayEnd");
        }
    }
}
#endif