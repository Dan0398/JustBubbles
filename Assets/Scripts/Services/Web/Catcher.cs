#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        static Catcher Instance;
		
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            var ObjOnScene = new GameObject();
            ObjOnScene.name = "WebGLCatcher";
            GameObject.DontDestroyOnLoad(ObjOnScene);
            Instance = ObjOnScene.AddComponent<Web.Catcher>();
        }
    }
}
#endif
#pragma warning restore CS0618