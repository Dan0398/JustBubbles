using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    [System.Serializable]
    public class MovingBubbleEffect
    {
        [SerializeField] private Field.BubbleField _field;
        [SerializeField] private ParticleSystem _bubbleParticle;
        private Transform _bubbleParticleTransform;
        private TrailRenderer _bubbleTrail;
        
        public void ApplyParticleToMovingBubble(Gameplay.User.ICircleObject Target)
        {
            CheckComponents();
            Replace();
            ApplyEffects();
            
            void CheckComponents()
            {
                if (_bubbleParticleTransform == null)
                {
                    _bubbleParticleTransform = _bubbleParticle.transform;
                    _bubbleTrail = _bubbleParticle.GetComponent<TrailRenderer>();
                }
                _bubbleTrail.emitting = false;
                _bubbleTrail.Clear();
            }
            
            void Replace()
            {
                _bubbleParticleTransform.SetParent(Target.MyTransform);
                _bubbleParticleTransform.localPosition = Vector3.zero;
                _bubbleParticleTransform.localScale = Vector3.one;
            }
            
            void ApplyEffects()
            {
                var Color = ColorPicker.GetColorByEnum(Target.MyColor);
                
                if (_field != null)
                {
                    var shape = _bubbleParticle.shape;
                    shape.radius = _field.BubbleSize * 0.5f * 1.2f;
                    _bubbleTrail.widthMultiplier = _field.BubbleSize * 0.5f;
                }
                
                #pragma warning disable CS0618
                _bubbleParticle.startColor = Color;
                #pragma warning restore CS0618
                _bubbleParticle.Play();
                
                Color -= Color.black * 0.5f;
                _bubbleTrail.startColor = Color;
                Color -= Color.black * 0.5f;
                _bubbleTrail.endColor = Color;
                _bubbleTrail.Clear();
                _bubbleTrail.emitting = true;
            }
        }
        
        public void DisableBubbleParticle()
        {
            _bubbleParticle.Stop();
        }
    }
}