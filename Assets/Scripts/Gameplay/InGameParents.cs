using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class InGameParents
    {
        [field:SerializeField] public GameObject Bubble { get; private set; }
        [field:SerializeField] public GameObject Merge  { get; private set; }
    }
}