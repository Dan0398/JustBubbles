using UnityEngine;

namespace Content.Merge
{
    [CreateAssetMenu(fileName = "MergeView", menuName = "Config/Merge/View", order = 155)]
    public class ViewConfig : ScriptableObject
    {
        [field:SerializeField] public AudioClip CollisionSound  { get; private set; }
        [field:SerializeField] public Item[] Items              { get; private set; }
        
        [System.Serializable]
        public class Item
        {
            [SerializeField] private string _name;
            [field:SerializeField] public GameObject Sample     { get; private set; }
        }
    }
}