#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif

namespace Data
{
    public abstract class BaseController<T> : Services.IService  where T:IAbstractData
    {
        public bool isDataLoaded    {get; private set;}
        public T Data               {get; protected set;}
        private bool _savingInvoked, _changesSubscribed, _isNewData;
        
        public BaseController()
        {
            _savingInvoked = false;
            PreloadData();
        }
        
        private async void PreloadData()
        {
            await LoadData();
            TrySubscribeOnChanges();
            isDataLoaded = true;
        }
        
        protected async Task LoadData()
        {
            Data = await Services.IO.LoadData<T>();
            _isNewData = Data == null || Data.Equals(default(T));
            if (_isNewData)
            {
                UnityEngine.Debug.Log("Found new data. Type:" + typeof(T).ToString());
                Data = System.Activator.CreateInstance<T>();
                Data.SetValuesAsFromStart();
                SaveData();
            }
        }
        
        public void SaveData()
        {
            if (_savingInvoked) return;
            _savingInvoked = true;
            Services.IO.SaveData(Data);
            _savingInvoked = false;
        }
        
        private void TrySubscribeOnChanges()
        {
            if (_changesSubscribed) return;
            _changesSubscribed = true;
            SubscribeOnChanges();
        }
        
        protected abstract void SubscribeOnChanges();
    }
}