#pragma warning disable CS0618 
#if UNITY_WEBGL
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Services.Web
{
    public partial class Catcher : MonoBehaviour
    {
        static System.Action<System.Type, object> OnDataLoaded;
        static System.Reflection.PropertyInfo[] ComplexProperties;
        static Data.Complex DataComplex;
        static bool dataRequestCalled;
        
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
                    OnDataLoaded -= OnLoaded;
                }
            };
            OnDataLoaded += OnLoaded;
            if (!dataRequestCalled)
            {
                dataRequestCalled = true;
                Application.ExternalCall("LoadData");
            }
            while (!Loaded) await Utilities.Wait();
            dataRequestCalled = false;
            return Result;
        }
        
        public void TranslateData(string ComplexInJSON)
        {
            DataComplex = new Data.Complex();
            try
            {
                DataComplex = Newtonsoft.Json.JsonConvert.DeserializeObject<Data.Complex>(ComplexInJSON);
            }
            catch(System.Exception ex)
            {
                Debug.Log("Error while receive ComplexData. Message:" + ex.Message);
                DataComplex = new Data.Complex();
            }
            if (ComplexProperties == null)
            {
                ComplexProperties = typeof(Data.Complex).GetProperties();
            }
            foreach(var Property in ComplexProperties)
            {
                OnDataLoaded?.Invoke(Property.PropertyType, Property.GetValue(DataComplex));
            }
        }
        
        public void TranslateDataError(string Reason)
        {
            /*
            var Options = new Canvases.Error.Option[2];
            Options[0] = new Canvases.Error.Option("TryAgain", () => Application.ExternalCall("LoadData"));
            Options[1] = new Canvases.Error.Option("StartWithCleanProfile", () => TranslateData("{}"));
            Canvases.Error.Window.ShowError("ErrorLoadData_Formatted", new string[]{Reason}, Options);
            */
        }
        
        public static void SendDataToServer(object UnderSave)
        {
            var ObjType = UnderSave.GetType();
            if (ComplexProperties == null)
            {
                ComplexProperties = typeof(Data.Complex).GetProperties();
            }
            bool Found = false;
            if (DataComplex == null)
            {
                DataComplex = new Data.Complex();
            }
            foreach(var Property in ComplexProperties)
            {
                if (Property.PropertyType == ObjType)
                {
                    Property.SetValue(DataComplex, UnderSave);
                    Found = true;
                    break;
                }
            }
            if (!Found)
            {
                Debug.Log("Try to save object with type \"" + ObjType.ToString() + "\". Type not found in \"DataComplex\"");
                return;
            }
            var ComplexJSON = Newtonsoft.Json.JsonConvert.SerializeObject(DataComplex);
            Application.ExternalCall("SaveData", ComplexJSON);
        }
    }
}
#endif