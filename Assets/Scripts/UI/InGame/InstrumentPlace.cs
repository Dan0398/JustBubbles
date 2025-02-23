using UnityEngine.UI;
using UnityEngine;

namespace UI.InGame
{
    [System.Serializable]
    public class InstrumentPlace
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private AspectRatioFitter.AspectMode _mode;
        [SerializeField] private int _place;
        
        public void Apply(AspectRatioFitter target)
        {
            target.transform.SetParent(_parent);
            target.transform.localScale = Vector3.one;
            target.transform.SetSiblingIndex(_place);
            target.aspectMode = _mode;
        }
    }
}