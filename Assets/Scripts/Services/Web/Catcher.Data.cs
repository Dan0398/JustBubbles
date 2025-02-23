#pragma warning disable CS0618 
#if UNITY_WEBGL
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        private static System.Action<System.Type, object> _onDataLoaded;
        private static System.Reflection.PropertyInfo[] _complexProperties;
        private static Data.Complex _dataComplex;
        private static bool _dataRequestCalled;
        
        public static async UniTask<TResult> GetDataFromServer<TResult>(System.Type DataType)
        {
            bool Loaded = false;
            TResult Result = (TResult)default;
            System.Action<System.Type, object> OnLoaded = null;
            OnLoaded = (type,obj) =>
            {
                if (DataType == type)
                {
                    Loaded = true;
                    Result = (TResult)obj;
                    _onDataLoaded -= OnLoaded;
                }
            };
            _onDataLoaded += OnLoaded;
            if (!_dataRequestCalled)
            {
                _dataRequestCalled = true;
                Application.ExternalCall("LoadData");
            }
            while (!Loaded) await Utilities.Wait();
            _dataRequestCalled = false;
            return Result;
        }
        
        public void TranslateData(string ComplexInJSON)
        {
            _dataComplex = new Data.Complex();
            try
            {
                _dataComplex = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.Complex>(ComplexInJSON);
            }
            catch(System.Exception ex)
            {
                Debug.Log("Error while receive ComplexData. Message:" + ex.Message);
                _dataComplex = new Data.Complex();
            }
            if (_complexProperties == null)
            {
                _complexProperties = typeof(Data.Complex).GetProperties();
            }
            foreach(var Property in _complexProperties)
            {
                _onDataLoaded?.Invoke(Property.PropertyType, Property.GetValue(_dataComplex));
            }
        }
        
        public static void SendDataToServer(object UnderSave)
        {
            var ObjType = UnderSave.GetType();
            if (_complexProperties == null)
            {
                _complexProperties = typeof(Data.Complex).GetProperties();
            }
            bool Found = false;
            _dataComplex ??= new Data.Complex();
            foreach(var Property in _complexProperties)
            {
                if (Property.PropertyType == ObjType)
                {
                    Property.SetValue(_dataComplex, UnderSave);
                    Found = true;
                    break;
                }
            }
            if (!Found)
            {
                Debug.Log("Try to save object with type \"" + ObjType.ToString() + "\". Type not found in \"DataComplex\"");
                return;
            }
            var ComplexJSON = Newtonsoft.Json.JsonConvert.SerializeObject(_dataComplex);
            Application.ExternalCall("SaveData", ComplexJSON);
        }
    }
}
#endif