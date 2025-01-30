using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace UI.Components.Shift
{
    [AddComponentMenu("Shift'On'Click Reactor")]
    public class Reactor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] float ShiftScale;
        Shifted[] shifted;
        Vector2 shiftVector;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (shifted == null) return;
            foreach(var UnderShift in shifted) UnderShift.Shift(shiftVector);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (shifted == null) return;
            foreach(var UnderShift in shifted) UnderShift.Shift(Vector2.zero);
        }

        void Start()
        {
            var ShiftedChildren = new List<Shifted>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<RectTransform>(out RectTransform rect))
                {
                    ShiftedChildren.Add(new Shifted(rect));
                }
            }
            shifted = ShiftedChildren.ToArray();
            OnValidate();
        }
        
        void OnValidate()
        {
            shiftVector = Vector2.down * ShiftScale;
        }
        
        class Shifted
        {
            RectTransform myRect;
            Vector2 offsetMin, offsetMax;
            
            public Shifted(RectTransform onScene)
            {
                myRect = onScene;
                offsetMin = onScene.offsetMin;
                offsetMax = onScene.offsetMax;
            }
            
            public void Shift(Vector2 Vector)
            {
                myRect.offsetMin = offsetMin + Vector;
                myRect.offsetMax = offsetMax + Vector;
            }
        }
    }
}