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
        private CancellationTokenSource _downloadBreaker;
        
        public void StartDownloadTexture()
        {
            if (AvatarTexture != null) return;
            throw new System.NotImplementedException();
        }
        
        public void SelfDelete()
        {
            if (_downloadBreaker != null)
            {
                _downloadBreaker.Cancel();
                _downloadBreaker.Dispose();
                _downloadBreaker = null;
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