using UnityEngine;

namespace Services
{
    public class CoroutineRunner : MonoBehaviour, IService
    {
        private static CoroutineRunner _instance;
        
        public static CoroutineRunner CreateCoroutineRunner()
        {
            if (_instance == null)
            {
                var OnScene = new GameObject("Coroutine Runner");
                DontDestroyOnLoad(OnScene);
                _instance = OnScene.AddComponent<CoroutineRunner>();
            }
            return _instance;
        }
    }
}