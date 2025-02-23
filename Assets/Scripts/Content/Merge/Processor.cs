using UnityEngine;

namespace Content.Merge
{
    public class Processor
    {
        private Services.Bundles.Agent _loader;
        
        public Processor()
        {
            _loader = Services.DI.Single<Services.Bundles.Agent>();
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
                Theme.DownloadRequest = _loader.GiveMeContent(Path, this);
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