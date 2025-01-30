using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player.Sessions
{
    [ExecuteAlways]
    public class HighlightMaskController : MonoBehaviour
    {
        [SerializeField, Range(0.0f, 1.0f)] float Size, Range;
        float oldSize, oldRange;
        Material mat;
        [SerializeField] bool ClearMat;
        
        void OnValidate()
        {
            if (mat == null)
            {
                mat = GetComponent<RawImage>().materialForRendering;
            }
            mat.SetFloat("_USize", Size);
            mat.SetFloat("_Range", Range);
        }
        
        void Update()
        {
            if (oldSize != Size || oldRange != Range)
            {
                oldSize = Size;
                oldRange = Range;
                OnValidate();
            }
            if (ClearMat)
            {
                mat = null;
                OnValidate();
                ClearMat = false;
            }
        }
    }
}