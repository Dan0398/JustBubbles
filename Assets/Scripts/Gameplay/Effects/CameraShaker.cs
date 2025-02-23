using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _shakeDynamic;
        [SerializeField] private float _shakeScale;
        [SerializeField] private Vector3 _basePos;
        private Transform _target;
        private WaitForEndOfFrame _wait;
        
        private void Start()
        {
            _target = transform;
            _wait = new WaitForEndOfFrame();
        }
        
        public void ApplyShake(Vector3 ShakeDirection, float Multiplier = 1)
        {
            StartCoroutine(ProcessShake(ShakeDirection, Multiplier));
        }
        
        private IEnumerator ProcessShake(Vector3 ShakeDirection, float Multiplier = 1)
        {
            const int Steps = 20;
            for (int Step = 0; Step <= Steps; Step ++)
            {
                _target.position = _shakeDynamic.Evaluate(Step / (float) Steps) * _shakeScale * Multiplier *  ShakeDirection + _basePos;
                yield return _wait;
            }
            _target.position = _basePos;
        }
    }
}