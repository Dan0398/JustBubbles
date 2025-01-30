using System.Collections.Generic;
using Utils.Observables;

namespace Services.Bundles
{
    public partial class Request
    {
        public readonly Priority Importance;
        public readonly string FullPathInStreamingAssets;
        public Observable<LoadedStatus> Status {get; private set;}
        public ContentPart Content {get; private set;}
        public int ClientsCount {get; private set;}
        List<object> clients;
        Agent parent;
        
        public Request(string fullPathInStreamingAssets, Priority priority, Agent Parent = null)
        {
            FullPathInStreamingAssets = fullPathInStreamingAssets;
            Importance = priority;
            parent = Parent;
            clients = new List<object>(3);
            Content = new ContentPart();
            Status = LoadedStatus.NotLoaded;
            SubscribeToLoad();
        }
        
        void SubscribeToLoad()
        {
            System.Action<UnityEngine.AssetBundle> OnLoaded = null;
            OnLoaded = (bundle) => 
            {
                Status = LoadedStatus.Success;
                Content.OnSuccessLoad -= OnLoaded;
            };
            Content.OnSuccessLoad += OnLoaded;
        }
        
        public void AddClient(object client)
        {
            clients.Add(client);
            ClientsCount++;
        }
        
        public void RemoveClient(object client)
        {
            clients.Remove(client);
            ClientsCount--;
        }
        
        public void SubmitError(string ErrorMessage, bool IsCritical)
        {
            UnityEngine.Debug.LogError(ErrorMessage);
            Status = IsCritical? LoadedStatus.CriticalFailture : LoadedStatus.LittleFail;
        }
    }
}