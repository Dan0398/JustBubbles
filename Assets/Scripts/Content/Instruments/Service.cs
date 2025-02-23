using UnityEngine;

namespace Content.Instrument
{
    public class Service: Services.IService
    {
        public Config Config   { get; private set; }
        
        public Service()
        {
            LoadConfig();
        }
        
        private async void LoadConfig()
        {
            var LoadRequest = Resources.LoadAsync<Config>("Config/Instruments");
            while (LoadRequest.isDone) await Utilities.Wait();
            Config = (Config) LoadRequest.asset;
        }
    }
}