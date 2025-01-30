using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.InGame
{
    [System.Serializable]
    public class InstrumentPlace
    {
        [SerializeField] Transform parent;
        [SerializeField] AspectRatioFitter.AspectMode mode;
        [SerializeField] int Place;
        
        public void Apply(AspectRatioFitter target)
        {
            target.transform.SetParent(parent);
            target.transform.localScale = Vector3.one;
            target.transform.SetSiblingIndex(Place);
            target.aspectMode = mode;
        }
    }
}