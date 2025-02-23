using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
    using Cysharp.Threading.Tasks;
#else
    using System.Threading.Tasks;
    using System.IO;
    using Newtonsoft.Json;
#endif

namespace Services
{
    public static class IO
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public static async UniTask<TResult> LoadData<TResult>()
        {
            return await Services.Web.Catcher.GetDataFromServer<TResult>(typeof(TResult));
        }
        
        public static void SaveData(object Serialized)
        {
            Services.Web.Catcher.SendDataToServer(Serialized);
        }
    
#else
        public static async Task<TResult> LoadData<TResult>()
        {
            var SplittedFullName = typeof(TResult).ToString().Split('.');
            var RealName = SplittedFullName[SplittedFullName.Length-1];
            var RawData = await GetDataInJSON(RealName);
            if (RawData == null) return (TResult)default; 
            var JsonData = System.Text.Encoding.UTF8.GetString(RawData);
            return (TResult) JsonConvert.DeserializeObject<TResult>(JsonData);
        }
        
        private async static Task<byte[]> GetDataInJSON(string Name)
        {
            string Path = GetFullPath(Name);
            if (!File.Exists(Path))
            {
                return null;
            }
            byte[] Result = null;
            using (var Reader = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Result = new byte[Reader.Length];
                await Reader.ReadAsync(Result, 0, Result.Length);
                Reader.Close();
                Reader.Dispose();
            }
            return Result;
        }
        
        public static void SaveData(object Serialized)
        {
            var JsonString = JsonConvert.SerializeObject(Serialized);
            WriteData(Serialized.GetType().Name, JsonString);
        }
            
        private static async void WriteData(string Name, string Data)
        {
            var RawData = System.Text.Encoding.UTF8.GetBytes(Data);
            string Path = GetFullPath(Name);
            using (var Writer = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                await Writer.WriteAsync(RawData,0, Data.Length);
                Writer.Close();
                Writer.Dispose();
            }
        }
        
        private static string GetFullPath(string FileName)
        {
            return Application.persistentDataPath + "/" + FileName + ".json";
        }
    #endif
    }
}