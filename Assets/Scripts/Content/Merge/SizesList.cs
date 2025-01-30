using UnityEngine;

namespace Content.Merge
{
    [CreateAssetMenu(fileName = "MergeSizes", menuName = "Config/Merge/Sizes Cfg", order = 155)]
    public class SizesList: ScriptableObject
    {
        [field:SerializeField] public Size[] Orientations { get; private set; }
        
        [System.Serializable]
        public class Size
        {
            [field:SerializeField] public string NameLangKey                    { get; private set; }
            [field:SerializeField] public Sprite Preview                        { get; private set; }
            [field:SerializeField] public Gameplay.Merge.Barrier.SizeType Data  { get; private set; }
            [field:SerializeField] public bool MobileAvailable                  { get; private set; }
            [field:SerializeField] public float MinimalAspect                   { get; private set; }
        }
    }
}