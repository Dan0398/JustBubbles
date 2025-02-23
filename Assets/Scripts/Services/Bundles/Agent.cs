using System.Collections.Generic;
using UnityEngine;

namespace Services.Bundles
{
    public class Agent: Services.IService
    {
        private List<Request> _allRequests, _forLoad;
        private Loader _ticketLoader;
        private int _prioritiesCount;
        private Services.CoroutineRunner _runner;
        
        public Agent()
        {
            _runner = Services.DI.Single<Services.CoroutineRunner>();
            _allRequests = new List<Request>(2);
            _forLoad = new List<Request>(2);
            _ticketLoader = new Loader(_runner);
            _ticketLoader.IsBusy.Changed += ProcessLoadingRoutine;
            _prioritiesCount = System.Enum.GetValues(typeof(Request.Priority)).Length;
        }
        
        public ContentPart GiveMeContent(string Path, object Client, Request.Priority Importance = Request.Priority.None)
        {
            Request target = null;
            foreach(var request in _allRequests)
            {
                if (request.FullPathInStreamingAssets.Equals(Path))
                {
                    target = request;
                    break;
                }
            }
            target ??= CreateRequest(Path, Importance);
            target.AddClient(Client);
            return target.Content;
        }
        
        private Request CreateRequest(string FullPathInStreamingAssets, Request.Priority Importance)
        {
            var ResultTicket = new Request(FullPathInStreamingAssets, Importance);
            _allRequests.Add(ResultTicket);
            _forLoad.Add(ResultTicket);
            ProcessLoadingRoutine();
            ResultTicket.Status.Changed += OnChanged;
            return ResultTicket;

            void OnChanged()
            {
                ProcessNewStatus(ResultTicket, () => { ResultTicket.Status.Changed -= OnChanged; });
            }
        }
        
        private void ProcessNewStatus(Request target, System.Action Unsubscribe)
        {
            if (target.Status.Value == Request.LoadedStatus.Success)
            {
                _forLoad.Remove(target);
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
            foreach(var ticket in _allRequests)
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
        
        private void RunCleanup()
        {
            for (int i = 0; i < _allRequests.Count; i++)
            {
                if (_allRequests[i].ClientsCount > 0) continue;
                _allRequests[i].Status.Changed = null;
                _allRequests[i].Content.Dispose();
                _allRequests.RemoveAt(i);
            }
        }
        
        private void ProcessLoadingRoutine()
        {
            if (_ticketLoader.IsBusy.Value) return;
            if (_forLoad.Count == 0) return;
            for (int prior = 0; prior < _prioritiesCount; prior ++)
            {
                var CurrentPriority = (Request.Priority)prior;
                for (int i = 0; i < _forLoad.Count; i++)
                {
                    if (_forLoad[i].Importance != CurrentPriority) continue;
                    if (_forLoad[i].Status.Value == Request.LoadedStatus.Success) continue;
                    if (_forLoad[i].Status.Value == Request.LoadedStatus.CriticalFailture) continue;
                    _ticketLoader.ProcessTicket(_forLoad[i]);
                    return;
                }
            }
        }
    }
}