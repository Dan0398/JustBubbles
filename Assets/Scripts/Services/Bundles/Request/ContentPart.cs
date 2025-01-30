using UnityEngine;

namespace Services.Bundles
{
    public class ContentPart
    {
        public AssetBundle BundleInMemory {get; private set;}
        public System.Action<AssetBundle> OnSuccessLoad;
        public bool IsReady => BundleInMemory != null;
        
        public void ApplyBundle(AssetBundle NewData)
        {
            if (IsReady)
            {
                Debug.LogError("Bundle content. Try to rewrite bundle");
                return;
            }
            BundleInMemory = NewData;
            OnSuccessLoad?.Invoke(BundleInMemory);
        }
        
        public void Dispose()
        {
            BundleInMemory?.Unload(true);
            BundleInMemory = null;
            OnSuccessLoad = null;
        }
    }
}
