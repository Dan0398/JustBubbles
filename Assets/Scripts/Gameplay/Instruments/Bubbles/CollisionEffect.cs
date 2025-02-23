using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    [System.Serializable]
    public class CollisionEffect
    {
        [SerializeField] private ParticleSystem _wallParticle;
        [SerializeField] private Effects.CameraShaker _shaker;
        private Transform _wallParticleTransform;
        private Services.Audio.Sounds.Service _sounds;
        
        public void ReactOnCollision(User.CollisionType Info, Vector3 CollisionPoint)
        {
            if (IsBarrier(Info))
            {
                bool IsLeft = Info == User.CollisionType.LeftBarrier;
                PlayWallParticle(CollisionPoint, IsLeft);
                _sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                _sounds.Play(Services.Audio.Sounds.SoundType.BubbleWallHit);
                Vector3 Direction = Vector3.right;
                if (Info == User.CollisionType.LeftBarrier)
                {
                    Direction = Vector3.left;
                }
                else if (Info == User.CollisionType.TopBarrier)
                {
                    Direction = Vector3.down;
                }
                _shaker.ApplyShake(Direction);
            }

            static bool IsBarrier(User.CollisionType Info)
            {
                return  Info == User.CollisionType.LeftBarrier 
                     || Info == User.CollisionType.RightBarrier
                     || Info == User.CollisionType.TopBarrier;
            }
        }
        
        private void PlayWallParticle(Vector3 Point, bool isLeft)
        {
            const int AngleShift = 15;
            if (_wallParticleTransform == null)
            {
                _wallParticleTransform = _wallParticle.transform;
            }
            _wallParticleTransform.position = Point;
            _wallParticleTransform.eulerAngles = Vector3.forward * (AngleShift + 90 * (isLeft? -1 : 1));
            _wallParticle.Emit(30);
        }
    }
}