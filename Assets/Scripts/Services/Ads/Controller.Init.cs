namespace Services.Advertisements 
{
    public partial class Controller : Services.IService
    {   
        void CreateAdSystem()
        {
            
    #if   UNITY_EDITOR
            AdSystem = new IdleAds();
    #elif UNITY_WEBGL
            AdSystem = new WebGLAds();
    #elif UNITY_ANDROID
            AdSystem = new IdleAds();
    #else
            AdSystem = new IdleAds();
    #endif
            AdSystem.Init();
        }
    }
}