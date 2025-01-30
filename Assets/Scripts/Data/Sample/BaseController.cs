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
        bool savingInvoked, changesSubscribed, isNewData;
        
        public BaseController()
        {
            savingInvoked = false;
            PreloadData();
        }
        
        async void PreloadData()
        {
            await LoadData();
            TrySubscribeOnChanges();
            isDataLoaded = true;
        }
        
        protected async Task LoadData()
        {
            Data = await Services.IO.LoadData<T>();
            isNewData = Data == null || Data.Equals(default(T));
            if (isNewData)
            {
                UnityEngine.Debug.Log("Found new data. Type:" + typeof(T).ToString());
                Data = System.Activator.CreateInstance<T>();
                Data.SetValuesAsFromStart();
                SaveData();
            }
        }
        
        public /*async*/ void SaveData()
        {
            if (savingInvoked) return;
            savingInvoked = true;
            //await Task.Delay(3000);
            Services.IO.SaveData(Data);
            savingInvoked = false;
        }
        
        void TrySubscribeOnChanges()
        {
            if (changesSubscribed) return;
            changesSubscribed = true;
            SubscribeOnChanges();
        }
        
        protected abstract void SubscribeOnChanges();
    }
}