using UnityEngine.Networking;
using System.Collections;
using Utils.Observables;

namespace Services.Bundles
{
    class Loader
    {
        public ObsBool IsBusy {get; private set;} = false;
        private Services.CoroutineRunner _runner;
        
        public Loader(Services.CoroutineRunner Runner)
        {
            this._runner = Runner;
        }
        
        public void ProcessTicket(Request Source)
        {
            IsBusy.Value = true;
            _runner.StartCoroutine(ProcessTicketInternal(Source));
        }
        
        private IEnumerator ProcessTicketInternal(Request Source)
        {
            var DownloadRequest = UnityWebRequestAssetBundle.GetAssetBundle(Source.FullPathInStreamingAssets);
            yield return DownloadRequest.SendWebRequest();
            if (DownloadRequest.result != UnityWebRequest.Result.Success)
            {
                bool Critical = DownloadRequest.result == UnityWebRequest.Result.ConnectionError;
                Source.SubmitError("Trying to download bundle \"" + Source.FullPathInStreamingAssets + "\". Failure. Reason:" + DownloadRequest.error, Critical);
            }
            else 
            {
                try
                {
                    Source.Content.ApplyBundle(DownloadHandlerAssetBundle.GetContent(DownloadRequest));
                }
                catch(System.Exception ex)
                {
                    Source.SubmitError("Trying to unwrap bundle  \"" + Source.FullPathInStreamingAssets + "\". Failure. Reason:" + ex.Message, true);
                }
            }
            DownloadRequest.Dispose();
            IsBusy.Value = false;
        }
    }
}