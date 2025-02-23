using UnityEngine;

namespace Gameplay.Effects
{
    public partial class Controller : MonoBehaviour
    {
        
        [Header("Fallen"), SerializeField] private ParticleSystem _bubblePopParticle;
        [SerializeField] private float _fallenForceScale;
        [Header("Shake"), SerializeField] private Transform _cameraTransform;
        [SerializeField] private AnimationCurve _camShakeDynamic;
        [SerializeField] private float _camShakeScale;
        [SerializeField] private Transform _movingParent;
        [Header ("Scene components"), SerializeField] private Gameplay.Field.BubbleField _field;
        [SerializeField] private Pools.BubblePool _bubblesPool;
        [SerializeField] private Sounds _sounds;
        private Transform _bubblePopTransform;
        private WaitForEndOfFrame _wait;
        private int _defaultLayer, _backgroundLayer;
        
        private void Start()
        {
            _wait = new WaitForEndOfFrame();
            _defaultLayer = LayerMask.NameToLayer("Default");
            _backgroundLayer = LayerMask.NameToLayer("Background");
        }
        
        private void TryResetMovingParent()
        {
            if (_popAnimationsCount > 0 || _fallAnimationsCount > 0) return;
            _movingParent.position = Vector3.zero;
        }
    }
}