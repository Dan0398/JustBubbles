using UnityEngine;

namespace Services.Env {
    public class PlatformDependentObject : MonoBehaviour
    {
        [SerializeField] GameObject[] PCObjects, TouchObjects;
        
        void Start()
        {
            var Env = Services.DI.Single<Services.Environment>();
            RefreshActivity(Env.IsUsingTouch.Value);
            Env.IsUsingTouch.Changed += () => RefreshActivity(Env.IsUsingTouch.Value);
        }
        
        void RefreshActivity(bool IsTouch)
        {
            for (int i=0; i< PCObjects.Length; i++)
            {
                PCObjects[i].SetActive(!IsTouch);
            }
            for (int i=0; i< TouchObjects.Length; i++)
            {
                TouchObjects[i].SetActive(IsTouch);
            }
        }
    }
}