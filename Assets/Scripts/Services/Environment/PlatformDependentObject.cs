using UnityEngine;

namespace Services.Env {
    public class PlatformDependentObject : MonoBehaviour
    {
        [SerializeField] private GameObject[] _pcObjects;
        [SerializeField] private GameObject[] _touchObjects;
        
        private void Start()
        {
            var Env = Services.DI.Single<Services.Environment>();
            RefreshActivity(Env.IsUsingTouch.Value);
            Env.IsUsingTouch.Changed += () => RefreshActivity(Env.IsUsingTouch.Value);
        }
        
        private void RefreshActivity(bool IsTouch)
        {
            for (int i=0; i< _pcObjects.Length; i++)
            {
                _pcObjects[i].SetActive(!IsTouch);
            }
            for (int i=0; i< _touchObjects.Length; i++)
            {
                _touchObjects[i].SetActive(IsTouch);
            }
        }
    }
}