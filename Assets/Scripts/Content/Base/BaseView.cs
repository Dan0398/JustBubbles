using System.Collections;
using UnityEngine;
using Utils.Observables;

namespace Content
{
    public abstract class BaseView<TContent> : MonoBehaviour
    {
        [HideInInspector] public Observable<TContent> SelectedContent;
        [SerializeField] Canvas MyView;
        [SerializeField] BrakelessGames.Localization.TextTMPLocalized InfoOnCenter;
        [SerializeField] protected TContent[] AvailableContent;
        protected int SelectedID, ViewedID;
        protected Data.UserController User;
        WaitForFixedUpdate Wait;
        System.Action OnClose;
        
        bool SelectedIsOld => SelectedID == ViewedID;
        
        void Start()
        {
            SelectedContent = AvailableContent[0];
            ApplyAvailable();
            Wait = new WaitForFixedUpdate();
            //StartCoroutine(ReadOnStartup());
            //MyView.enabled = false;
        }
        
        IEnumerator ReadOnStartup()
        {
            User = Services.DI.Single<Data.UserController>();
            while(User == null)
            {
                yield return Wait;
                User = Services.DI.Single<Data.UserController>();
            }
            while (!User.isDataLoaded) 
            {
                yield return Wait;
            }
            SubscribeToUser();
            ApplyAvailable();
            SelectedContent.Value = AvailableContent[SelectedID];
        }
        
        protected abstract void ApplyAvailable();
        
        protected abstract void SubscribeToUser();
        
        protected abstract void SaveSelected();
        
        public void GoToSelector(System.Action OnClose)
        {
            this.OnClose = OnClose;
            ViewedID = SelectedID;
            RefreshView();
            MyView.enabled = true;
        }
        
        public void SwitchToNext()
        {
            if (AvailableContent.Length == 0) return;
            var Old = ViewedID;
            ViewedID++;
            if (ViewedID >= AvailableContent.Length)
            {
                ViewedID = 0;
            }
            RefreshView();
            ReactOnDeselect(Old);
        }
        
        public void SwitchToPrev()
        {
            if (AvailableContent.Length == 0) return;
            var Old = ViewedID;
            ViewedID--;
            if (ViewedID < 0)
            {
                ViewedID = AvailableContent.Length - 1;
            }
            RefreshView();
            ReactOnDeselect(Old);
        }
        
        void RefreshView()
        {
            InfoOnCenter?.SetNewKey(SelectedIsOld? "Cancel" : "WatchAdsAndSelect");
            SelectedContent.Value = AvailableContent[ViewedID];
        }
        
        protected virtual void ReactOnDeselect(int id) { }
        
        public async void TryToApply()
        {
            if (!SelectedIsOld)
            {
                var Ads = Services.DI.Single<Services.Advertisements.Controller>();
                bool Success = await Ads.IsRewardAdSuccess();
                if (Success)
                {
                    SelectedID = ViewedID;
                    SaveSelected();
                }
            }
            SelectedContent.Value = AvailableContent[SelectedID];
            MyView.enabled = false;
            OnClose?.Invoke();
            OnClose = null;
        }
    }
}