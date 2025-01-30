#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        static bool isReviewMade, isReviewSuccess;
        
        public static async UniTask<bool> IsReviewSuccess()
        {
            isReviewMade = false;
            isReviewSuccess = false;
            Application.ExternalCall("RequestReview");
            while (!isReviewMade) await Utilities.Wait();
            return isReviewSuccess;
        }
        
        public void ReviewSuccess()
        {
            isReviewSuccess = true;
            isReviewMade = true;
        }
        
        public void ReviewFail(string Message)
        {
            isReviewSuccess = false;
            isReviewMade = true;
        }
    }
}
#endif