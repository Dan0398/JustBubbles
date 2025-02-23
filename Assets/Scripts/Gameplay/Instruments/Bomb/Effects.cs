using UnityEngine;

namespace Gameplay.Instruments.Bomb
{
    [System.Serializable]
    public class Effects : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _explosionEffect;
        [SerializeField] private Gameplay.Effects.CameraShaker _shaker;
        
        public void PlayExplosionEffects(Vector3 ExplosionWorldPos)
        {
            _explosionEffect.transform.position = new Vector3(ExplosionWorldPos.x, ExplosionWorldPos.y, -0.5f);
            _explosionEffect.Emit(1);
            _shaker.ApplyShake(Random.insideUnitCircle.normalized, 1.5f);
        }
    }
}