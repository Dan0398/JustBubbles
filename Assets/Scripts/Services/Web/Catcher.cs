#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        private static Catcher _instance;
		
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            var ObjOnScene = new GameObject();
            ObjOnScene.name = "WebGLCatcher";
            GameObject.DontDestroyOnLoad(ObjOnScene);
            _instance = ObjOnScene.AddComponent<Web.Catcher>();
        }
    }
}
#endif
#pragma warning restore CS0618