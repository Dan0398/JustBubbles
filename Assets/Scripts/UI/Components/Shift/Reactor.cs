using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace UI.Components.Shift
{
    [AddComponentMenu("Shift'On'Click Reactor")]
    public class Reactor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _shiftScale;
        private Shifted[] _shifted;
        private Vector2 _shiftVector;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_shifted == null) return;
            foreach(var UnderShift in _shifted) UnderShift.Shift(_shiftVector);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_shifted == null) return;
            foreach(var UnderShift in _shifted) UnderShift.Shift(Vector2.zero);
        }

        private void Start()
        {
            var ShiftedChildren = new List<Shifted>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<RectTransform>(out RectTransform rect))
                {
                    ShiftedChildren.Add(new Shifted(rect));
                }
            }
            _shifted = ShiftedChildren.ToArray();
            OnValidate();
        }
        
        private void OnValidate()
        {
            _shiftVector = Vector2.down * _shiftScale;
        }
        
        private class Shifted
        {
            private RectTransform _myRect;
            private Vector2 _offsetMin, _offsetMax;
            
            public Shifted(RectTransform onScene)
            {
                _myRect = onScene;
                _offsetMin = onScene.offsetMin;
                _offsetMax = onScene.offsetMax;
            }
            
            public void Shift(Vector2 Vector)
            {
                _myRect.offsetMin = _offsetMin + Vector;
                _myRect.offsetMax = _offsetMax + Vector;
            }
        }
    }
}