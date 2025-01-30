#if UNITY_WEBGL
using Task = Cysharp.Threading.Tasks.UniTask;
#else
using Task = System.Threading.Tasks.Task;
#endif
using System.Threading;
using UnityEngine;
using System;

namespace Services.Leaderboards
{
    [System.Serializable]
    public class Line
    {
        public string ID, Name, AvatarURL;
        public int Score, Rank;
        public bool IsUser;
        
        [field:NonSerialized] public Texture AvatarTexture {get; private set;}
        [NonSerialized] public System.Action OnAvatarLoaded;
        CancellationTokenSource DownloadBreaker;
        
        public void StartDownloadTexture()
        {
            if (AvatarTexture != null) return;
            throw new System.NotImplementedException();
        }
        /*
            DonwloadBreaker = new CancellationTokenSource();
            GetTextureFromServer(DonwloadBreaker.Token);
        }
        
        async void GetTextureFromServer(CancellationToken token)
        {
            var Request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(AvatarURL);
            Request.SendWebRequest();
            while (!Request.isDone)
            {
                if (!Application.isPlaying) return;
                if (token.IsCancellationRequested)
                {
                    Request.Abort();
                    Request.Dispose();
                    return;
                }
                await Utilities.Wait();
            }
            AvatarTexture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(Request);
            OnAvatarLoaded?.Invoke();
            Request.Dispose();
        }
        */
        
        public void SelfDelete()
        {
            if (DownloadBreaker != null)
            {
                DownloadBreaker.Cancel();
                DownloadBreaker.Dispose();
                DownloadBreaker = null;
            }
            if (AvatarTexture != null)
            {
                GameObject.DestroyImmediate(AvatarTexture, true);
                AvatarTexture = null;
            }
            OnAvatarLoaded = null;
        }
    }
}