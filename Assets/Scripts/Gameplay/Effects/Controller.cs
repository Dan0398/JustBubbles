using System.Collections;
using UnityEngine;

namespace Gameplay.Effects
{
    public partial class Controller : MonoBehaviour
    {
        [Header("Fallen"), SerializeField] ParticleSystem BubblePopParticle;
        Transform BubblePopTransform;
        [SerializeField] float FallenForceScale;
        [Header("Shake"), SerializeField] Transform CameraTransform;
        [SerializeField] AnimationCurve CamShakeDynamic;
        [SerializeField] float CamShakeScale;
        [SerializeField] Transform MovingParent;
        [Header ("Scene components"), SerializeField] Gameplay.Field.BubbleField Field;
        [SerializeField] Pools.BubblePool BubblesPool;
        [SerializeField] Sounds Sounds;
        WaitForEndOfFrame Wait;
        int DefaultLayer, BackgroundLayer;
        
        void Start()
        {
            Wait = new WaitForEndOfFrame();
            DefaultLayer = LayerMask.NameToLayer("Default");
            BackgroundLayer = LayerMask.NameToLayer("Background");
        }
        
        void TryResetMovingParent()
        {
            if (PopAnimationsCount > 0 || FallAnimationsCount > 0) return;
            MovingParent.position = Vector3.zero;
        }
    }
}