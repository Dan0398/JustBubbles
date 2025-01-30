using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Advertisements
{
    public class Banner : Services.IService
    {
        bool Paused 
        {
            get => paused;
            set  { paused = value; RefreshView();}
        }
        bool IsMapTypeSuitable
        {
            get => isMapTypeSuitable;
            set {isMapTypeSuitable = value; RefreshView();}
        }
        bool bannerShown, paused, isMapTypeSuitable;
        
        /*
        public Banner()
        {
            SubscribeToPause();
        }
        
        void SubscribeToPause()
        {
            var Pauser = Services.DI.Single<Services.Pause>();
            System.Action<Services.Pause.PauseType> ReactToPause = (s) => Paused = s != Services.Pause.PauseType.None;
            ReactToPause.Invoke(Pauser.CurrentPauseType);
            Pauser.OnPauseChanges += ReactToPause;
        }
        */
        
        public void ApplyAvailableStatus(bool Available)
        {
            IsMapTypeSuitable = Available;
        }
        
        void RefreshView()
        {
            bool BannerAvailable = Paused || IsMapTypeSuitable;
            if (BannerAvailable == bannerShown) return;
            bannerShown = BannerAvailable;
            if (BannerAvailable)
            {
                Services.DI.Single<Services.Advertisements.Controller>().ShowBanner();
            }
            else 
            {
                Services.DI.Single<Services.Advertisements.Controller>().HideBanner();
            }
        }
    }
}