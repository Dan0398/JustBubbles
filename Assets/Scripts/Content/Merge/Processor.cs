using UnityEngine;

namespace Content.Merge
{
    public class Processor
    {
        Services.Bundles.Agent Loader;
        
        public Processor()
        {
            Loader = Services.DI.Single<Services.Bundles.Agent>();
        }
        
        public void LoadTheme(ThemesList.Theme Theme, System.Action AfterLoad)
        {
            if (Theme.Loaded != null)
            {
                AfterLoad?.Invoke();
                return;
            }
            if (Theme.DownloadRequest == null)
            {
                var Path = Application.streamingAssetsPath + '/' + Theme.BundlePath;
                Theme.DownloadRequest = Loader.GiveMeContent(Path, this);
            }
            System.Action<AssetBundle> After = null;
            After = (s) =>
            {
                var UnpackRequest = s.LoadAssetAsync<ViewConfig>("Config");
                UnpackRequest.completed += (s)=>
                {
                    Theme.Loaded = (ViewConfig)UnpackRequest.asset;
                    AfterLoad?.Invoke();
                };
                Theme.DownloadRequest.OnSuccessLoad -= After;
            };
            Theme.DownloadRequest.OnSuccessLoad += After;
        }
    }
}