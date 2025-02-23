namespace Services.Advertisements 
{
    public partial class Controller : Services.IService
    {   
        void CreateAdSystem()
        {
            
    #if   UNITY_EDITOR
            _adSystem = new IdleAds();
    #elif UNITY_WEBGL
            _adSystem = new WebGLAds();
    #elif UNITY_ANDROID
            _adSystem = new IdleAds();
    #else
            _adSystem = new IdleAds();
    #endif
            _adSystem.Init();
        }
    }
}