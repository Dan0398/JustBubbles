using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    [System.Serializable]
    public class MovingBubbleEffect
    {
        [SerializeField] Field.BubbleField Field;
        [SerializeField] ParticleSystem BubbleParticle;
        Transform BubbleParticleTransform;
        TrailRenderer BubbleTrail;
        
        public void ApplyParticleToMovingBubble(Gameplay.User.ICircleObject Target)
        {
            CheckComponents();
            Replace();
            ApplyEffects();
            
            void CheckComponents()
            {
                if (BubbleParticleTransform == null)
                {
                    BubbleParticleTransform = BubbleParticle.transform;
                    BubbleTrail = BubbleParticle.GetComponent<TrailRenderer>();
                }
                BubbleTrail.emitting = false;
                BubbleTrail.Clear();
            }
            
            void Replace()
            {
                BubbleParticleTransform.SetParent(Target.MyTransform);
                BubbleParticleTransform.localPosition = Vector3.zero;
                BubbleParticleTransform.localScale = Vector3.one;
            }
            
            void ApplyEffects()
            {
                var Color = ColorPicker.GetColorByEnum(Target.MyColor);
                
                if (Field != null)
                {
                    var shape = BubbleParticle.shape;
                    shape.radius = Field.BubbleSize * 0.5f * 1.2f;
                    BubbleTrail.widthMultiplier = Field.BubbleSize * 0.5f;
                }
                
                #pragma warning disable CS0618
                BubbleParticle.startColor = Color;
                #pragma warning restore CS0618
                BubbleParticle.Play();
                
                Color -= Color.black * 0.5f;
                BubbleTrail.startColor = Color;
                Color -= Color.black * 0.5f;
                BubbleTrail.endColor = Color;
                BubbleTrail.Clear();
                BubbleTrail.emitting = true;
            }
        }
        
        public void DisableBubbleParticle()
        {
            BubbleParticle.Stop();
            //BubbleParticleTransform.SetParent(transform);
            //BubbleParticleTransform.localScale = Vector3.one;
        }
    
    }
}