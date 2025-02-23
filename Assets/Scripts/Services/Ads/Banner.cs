namespace Services.Advertisements
{
    public class Banner : Services.IService
    {
        private bool _bannerShown, _paused, _isMapTypeSuitable;
        
        private bool Paused 
        {
            get => _paused;
            set 
            {
                _paused = value;
                RefreshView();
            }
        }
        private bool IsMapTypeSuitable
        {
            get => _isMapTypeSuitable;
            set
            {
                _isMapTypeSuitable = value;
                RefreshView();
            }
        }
        
        public void ApplyAvailableStatus(bool Available)
        {
            IsMapTypeSuitable = Available;
        }
        
        private void RefreshView()
        {
            bool BannerAvailable = Paused || IsMapTypeSuitable;
            if (BannerAvailable == _bannerShown) return;
            _bannerShown = BannerAvailable;
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