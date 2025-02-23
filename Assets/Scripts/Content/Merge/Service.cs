using UnityEngine;

namespace Content.Merge
{
    public class Service: Services.IService
    {
        public ThemesList ThemesConfig  { get; private set; }
        public SizesList SizesConfig    { get; private set; }
        public Processor Processor      { get; private set; }
        
        public Service()
        {
            LoadThemes();
            LoadSizes();
            Processor = new Processor();
        }
        
        private async void LoadThemes()
        {
            var LoadRequest = Resources.LoadAsync<ThemesList>("Config/MergeThemes");
            while (LoadRequest.isDone) await Utilities.Wait();
            ThemesConfig = (ThemesList) LoadRequest.asset;
        }
        
        private async void LoadSizes()
        {
            var LoadRequest = Resources.LoadAsync<SizesList>("Config/MergeSizes");
            while (LoadRequest.isDone) await Utilities.Wait();
            SizesConfig = (SizesList) LoadRequest.asset;
        }
    }
}