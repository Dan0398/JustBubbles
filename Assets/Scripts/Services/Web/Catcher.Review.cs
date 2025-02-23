#pragma warning disable CS0618 
#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        private static bool _isReviewMade, _isReviewSuccess;
        
        public static async UniTask<bool> IsReviewSuccess()
        {
            _isReviewMade = false;
            _isReviewSuccess = false;
            Application.ExternalCall("RequestReview");
            while (!_isReviewMade) await Utilities.Wait();
            return _isReviewSuccess;
        }
        
        public void ReviewSuccess()
        {
            _isReviewSuccess = true;
            _isReviewMade = true;
        }
        
        public void ReviewFail(string Message)
        {
            _isReviewSuccess = false;
            _isReviewMade = true;
        }
    }
}
#endif