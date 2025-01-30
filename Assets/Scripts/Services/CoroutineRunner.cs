using UnityEngine;

namespace Services
{
    public class CoroutineRunner : MonoBehaviour, IService
    {
        static CoroutineRunner Instance;
        
        public static CoroutineRunner CreateCoroutineRunner()
        {
            if (Instance == null)
            {
                var OnScene = new GameObject("Coroutine Runner");
                DontDestroyOnLoad(OnScene);
                Instance = OnScene.AddComponent<CoroutineRunner>();
            }
            return Instance;
        }
    }
}