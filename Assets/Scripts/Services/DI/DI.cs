using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class DI : MonoBehaviour
    {
        static Dictionary<System.Type, IService> AllSystems;
        
        static DI()
        {
            AllSystems = new Dictionary<System.Type, IService>();
        }
        
        public static Service Single<Service>() where Service: IService
        {
            if (AllSystems.TryGetValue(typeof(Service), out IService Value))
            {
                return (Service) Value;
            }
            Debug.LogError("Try to call service \"" + typeof(Service).ToString() + "\". Error: didn't found.");
            return default(Service);
        }
        
        public static void Register<Service>(IService ServiceObject) where Service: IService
        {
            if (AllSystems.TryGetValue(typeof(Service), out IService Value))
            {
                AllSystems[typeof(Service)] = ServiceObject;
            }
            else 
            {
                AllSystems.Add(typeof(Service), ServiceObject);
            }
        }
    }
}