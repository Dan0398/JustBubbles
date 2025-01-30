#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        static bool LoginRequested, LoginSuccess;
        
        public static async UniTask<bool> IsRequestLogInSuccess()
        {
            LoginSuccess = false;
            //Services.DI.Single<Services.Pause>().CallHardPause();
            Application.ExternalCall("TryLogin");
            while (LoginRequested) await UniTask.Delay(100);
            return LoginSuccess;
        }
    }
}
#endif