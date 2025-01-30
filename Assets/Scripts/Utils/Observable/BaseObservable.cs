using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Utils.Observables
{
    [System.Serializable]
    public class Observable<T>
#if !(UNITY_ANDROID && !UNITY_EDITOR)
     : ISerializationCallbackReceiver
#endif
    {
        [JsonRequired] T myValue;
        [JsonIgnore, SerializeField] T inspector;
        [JsonIgnore] public System.Action Changed;
        [JsonIgnore] public T Value
        {
            get => myValue;
            set 
            {
                if (value == null && myValue == null) return;
                if (value != null && myValue != null)
                {
                    #if UNITY_ANDROID && !UNITY_EDITOR
                        bool equals = false;
                        ThreadHelper.RunOnMainThread(() => equals = value.Equals(myValue)).Wait();
                        if (equals) return;
                    #else
                        if (value.Equals(myValue)) return;
                    #endif
                }
                
                myValue = value;
                Changed?.Invoke();
            }
        }
        
        public Observable(T BaseValue) => myValue = BaseValue;
        
        public static implicit operator Observable<T>(T value) => new Observable<T>(value);

#if !(UNITY_ANDROID && !UNITY_EDITOR)
        public void OnBeforeSerialize() => inspector = Value;
        public void OnAfterDeserialize() => Value = inspector;
#else
        public static class ThreadHelper
        {
            private static readonly TaskFactory _taskFactory = new
                TaskFactory(System.Threading.CancellationToken.None,
                            TaskCreationOptions.None,
                            TaskContinuationOptions.None,
                            TaskScheduler.FromCurrentSynchronizationContext());

            public static Task RunOnMainThread(System.Action action)
            {
                return _taskFactory.StartNew(action);
            }
        }
#endif
    }
}