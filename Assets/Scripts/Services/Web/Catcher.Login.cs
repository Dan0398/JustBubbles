#pragma warning disable CS0618 
#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        private static bool _loginRequested, _loginSuccess;
        
        public static async UniTask<bool> IsRequestLogInSuccess()
        {
            _loginSuccess = false;
            Application.ExternalCall("TryLogin");
            while (_loginRequested) await UniTask.Delay(100);
            return _loginSuccess;
        }
    }
}
#endif