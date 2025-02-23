using UnityEngine.UI;
using UnityEngine;

public class CalculateChilds : LayoutGroup
{
    [SerializeField, Min(0)] private float _spacing;
    private float _fullWidth, _changedWidth, _realWidth;
    
    public override void CalculateLayoutInputVertical()
    {
        Apply();
    }

    public override void SetLayoutHorizontal()
    {
        Apply();
    }
    
    private void Apply()
    {
        _fullWidth = rectTransform.rect.width;
        _changedWidth = _fullWidth - _spacing * (rectChildren.Count-1);
        AspectRatioFitter ChildRatio = null;
        int RescalableCount = 0;
        float[] Shifts = new float[rectChildren.Count];
        for (int i = 0; i< rectChildren.Count; i++)
        {
            ChildRatio = rectChildren[i].GetComponent<AspectRatioFitter>();
            if (ChildRatio != null)
            {
                Shifts[i] = (rectTransform.rect.height-padding.vertical) * ChildRatio.aspectRatio;
                _changedWidth -= Shifts[i];
            }
            else 
            {
                RescalableCount++;
                Shifts[i] = -1;
            }
        }
        if (RescalableCount == 0) return;
        _realWidth = (_changedWidth - padding.horizontal) / (float) RescalableCount;
        float Pos = - rectTransform.sizeDelta.x * rectTransform.pivot.x + padding.left;
        float CurrentWidth = 0;
        for (int i = 0; i< rectChildren.Count; i++)
        {
            CurrentWidth = (Shifts[i] == -1?  _realWidth : Shifts[i]);
            
            rectChildren[i].sizeDelta = new Vector2(CurrentWidth, rectTransform.rect.height - padding.vertical);
            rectChildren[i].localPosition = Vector3.right * (Pos + CurrentWidth * rectChildren[i].pivot.x);
            
            Pos += CurrentWidth + _spacing;
        }
    }

    public override void SetLayoutVertical()
    {
        
        Apply();
    }
}
