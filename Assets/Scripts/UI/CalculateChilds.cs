using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateChilds : LayoutGroup
{
    [SerializeField, Min(0)] float Spacing;
    float FullWidth, ChangedWidth, RealWidth;
    
    public override void CalculateLayoutInputVertical()
    {
        Apply();
    }

    public override void SetLayoutHorizontal()
    {
        Apply();
    }
    
    void Apply()
    {
        FullWidth = rectTransform.rect.width;
        ChangedWidth = FullWidth - Spacing * (rectChildren.Count-1);
        AspectRatioFitter ChildRatio = null;
        int RescalableCount = 0;
        float[] Shifts = new float[rectChildren.Count];
        for (int i = 0; i< rectChildren.Count; i++)
        {
            ChildRatio = rectChildren[i].GetComponent<AspectRatioFitter>();
            if (ChildRatio != null)
            {
                Shifts[i] = (rectTransform.rect.height-padding.vertical) * ChildRatio.aspectRatio;
                ChangedWidth -= Shifts[i];
            }
            else 
            {
                RescalableCount++;
                Shifts[i] = -1;
            }
        }
        if (RescalableCount == 0) return;
        RealWidth = (ChangedWidth - padding.horizontal) / (float) RescalableCount;
        float Pos = - rectTransform.sizeDelta.x * rectTransform.pivot.x + padding.left;
        float CurrentWidth = 0;
        for (int i = 0; i< rectChildren.Count; i++)
        {
            CurrentWidth = (Shifts[i] == -1?  RealWidth : Shifts[i]);
            
            rectChildren[i].sizeDelta = new Vector2(CurrentWidth, rectTransform.rect.height - padding.vertical);
            rectChildren[i].localPosition = Vector3.right * (Pos + CurrentWidth * rectChildren[i].pivot.x);
            
            Pos += CurrentWidth + Spacing;
        }
    }

    public override void SetLayoutVertical()
    {
        
        Apply();
    }
}
