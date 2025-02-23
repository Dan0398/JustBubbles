using UnityEngine.UI;
using UnityEngine;

namespace UI.Player.Sessions
{
    [ExecuteAlways]
    public class HighlightMaskController : MonoBehaviour
    {
        [SerializeField, Range(0.0f, 1.0f)] private float _size;
        [SerializeField, Range(0.0f, 1.0f)] private float _range;
        [SerializeField] private bool _clearMat;
        private float _oldSize, _oldRange;
        private Material _mat;
        
        private void OnValidate()
        {
            if (_mat == null)
            {
                _mat = GetComponent<RawImage>().materialForRendering;
            }
            _mat.SetFloat("_USize", _size);
            _mat.SetFloat("_Range", _range);
        }
        
        private void Update()
        {
            if (_oldSize != _size || _oldRange != _range)
            {
                _oldSize = _size;
                _oldRange = _range;
                OnValidate();
            }
            if (_clearMat)
            {
                _mat = null;
                OnValidate();
                _clearMat = false;
            }
        }
    }
}