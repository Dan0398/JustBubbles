using Utils.Observables;
using UnityEngine;

namespace Content
{
    public abstract class BaseView<TContent> : MonoBehaviour
    {
        [HideInInspector] public Observable<TContent> SelectedContent;
        [SerializeField] protected TContent[] AvailableContent;
        [SerializeField] private Canvas _myView;
        [SerializeField] private BrakelessGames.Localization.TextTMPLocalized _infoOnCenter;
        protected int SelectedID, ViewedID;
        protected Data.UserController User;
        private System.Action _onClose;
        
        private bool SelectedIsOld => SelectedID == ViewedID;
        
        private void Start()
        {
            SelectedContent = AvailableContent[0];
            ApplyAvailable();
        }
        
        protected abstract void ApplyAvailable();
        
        protected abstract void SubscribeToUser();
        
        protected abstract void SaveSelected();
        
        public void GoToSelector(System.Action OnClose)
        {
            this._onClose = OnClose;
            ViewedID = SelectedID;
            RefreshView();
            _myView.enabled = true;
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
        
        private void RefreshView()
        {
            if (_infoOnCenter != null) _infoOnCenter.SetNewKey(SelectedIsOld? "Cancel" : "WatchAdsAndSelect");
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
            _myView.enabled = false;
            _onClose?.Invoke();
            _onClose = null;
        }
    }
}