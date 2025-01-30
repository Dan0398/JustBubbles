using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] AnimationCurve ShakeDynamic;
        [SerializeField] float ShakeScale;
        [SerializeField] Vector3 BasePos;
        Transform target;
        WaitForEndOfFrame Wait;
        
        void Start()
        {
            target = transform;
            Wait = new WaitForEndOfFrame();
        }
        
        public void ApplyShake(Vector3 ShakeDirection, float Multiplier = 1)
        {
            StartCoroutine(ProcessShake(ShakeDirection, Multiplier));
        }
        
        IEnumerator ProcessShake(Vector3 ShakeDirection, float Multiplier = 1)
        {
            const int Steps = 20;
            for (int Step = 0; Step <= Steps; Step ++)
            {
                target.position = ShakeDynamic.Evaluate(Step / (float) Steps) * ShakeScale * Multiplier *  ShakeDirection + BasePos;
                yield return Wait;
            }
            target.position = BasePos;
        }
    }
}