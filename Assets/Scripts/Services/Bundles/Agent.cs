using System.Collections.Generic;
using UnityEngine;

namespace Services.Bundles
{
    public class Agent: Services.IService
    {
        List<Request> allRequests, forLoad;
        Loader ticketLoader;
        int prioritiesCount;
        bool isLoading;
        Services.CoroutineRunner Runner;
        
        public Agent()
        {
            Runner = Services.DI.Single<Services.CoroutineRunner>();
            allRequests = new List<Request>(2);
            forLoad = new List<Request>(2);
            ticketLoader = new Loader(Runner);
            ticketLoader.IsBusy.Changed += ProcessLoadingRoutine;
            prioritiesCount = System.Enum.GetValues(typeof(Request.Priority)).Length;
        }
        
        public ContentPart GiveMeContent(string Path, object Client, Request.Priority Importance = Request.Priority.None)
        {
            //Debug.Log("Creating load request at path: " + Path);
            Request target = null;
            foreach(var request in allRequests)
            {
                if (request.FullPathInStreamingAssets.Equals(Path))
                {
                    target = request;
                    break;
                }
            }
            if (target == null)
            {
                target = CreateRequest(Path, Importance);
            }
            target.AddClient(Client);
            return target.Content;
        }
        
        Request CreateRequest(string FullPathInStreamingAssets, Request.Priority Importance)
        {
            var ResultTicket = new Request(FullPathInStreamingAssets, Importance);
            allRequests.Add(ResultTicket);
            forLoad.Add(ResultTicket);
            ProcessLoadingRoutine();
            System.Action OnChanged = null;
            OnChanged = () => 
            {
                ProcessNewStatus(ResultTicket, ()=> { ResultTicket.Status.Changed -= OnChanged; });
            };
            ResultTicket.Status.Changed += OnChanged;
            return ResultTicket;
        }
        
        void ProcessNewStatus(Request target, System.Action Unsubscribe)
        {
            if (target.Status.Value == Request.LoadedStatus.Success)
            {
                forLoad.Remove(target);
                Unsubscribe.Invoke();
                return;
            }
        }
        
        public void ReleaseContentUsage(ContentPart Data, object Client)
        {
            if (Data == null)
            {
                Debug.Log("Try to release null content");
                return;
            }
            foreach(var ticket in allRequests)
            {
                if (ticket.Content.Equals(Data))
                {
                    ticket.RemoveClient(Client);
                    RunCleanup();
                    return;
                }
            }
            Debug.Log("Error while release data resource");
        }
        
        void RunCleanup()
        {
            for (int i = 0; i < allRequests.Count; i++)
            {
                if (allRequests[i].ClientsCount > 0) continue;
                allRequests[i].Status.Changed = null;
                allRequests[i].Content.Dispose();
                allRequests.RemoveAt(i);
            }
        }
        
        void ProcessLoadingRoutine()
        {
            if (ticketLoader.IsBusy.Value) return;
            if (forLoad.Count == 0) return;
            for (int prior = 0; prior < prioritiesCount; prior ++)
            {
                var CurrentPriority = (Request.Priority)prior;
                for (int i = 0; i < forLoad.Count; i++)
                {
                    if (forLoad[i].Importance != CurrentPriority) continue;
                    if (forLoad[i].Status.Value == Request.LoadedStatus.Success) continue;
                    if (forLoad[i].Status.Value == Request.LoadedStatus.CriticalFailture) continue;
                    ticketLoader.ProcessTicket(forLoad[i]);
                    return;
                }
            }
        }
    }
}