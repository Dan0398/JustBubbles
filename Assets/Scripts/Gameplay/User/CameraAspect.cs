using UnityEngine;

namespace Gameplay.User
{
    public class CameraAspect : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        private float _aspect;
        private float _minimalAspect = 9/16f;
        
        private void Update()
        {
            var aspect = Screen.width / (float) Screen.height;
            if (aspect == _aspect) return;
            _aspect = aspect;
            _camera.orthographicSize = 5 * Mathf.Clamp((_minimalAspect+0.04f) / (float) _aspect, 1, 999f);
        }
    }
}