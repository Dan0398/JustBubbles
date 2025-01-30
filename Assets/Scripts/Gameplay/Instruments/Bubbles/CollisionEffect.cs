using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    [System.Serializable]
    public class CollisionEffect
    {
        [SerializeField] ParticleSystem WallParticle;
        [SerializeField] Effects.CameraShaker Shaker;
        Transform WallParticleTransform;
        Services.Audio.Sounds.Service Sounds;
        
        public void ReactOnCollision(User.CollisionType Info, Vector3 CollisionPoint)
        {
            if (IsBarrier(Info))
            {
                bool IsLeft = Info == User.CollisionType.LeftBarrier;
                PlayWallParticle(CollisionPoint, IsLeft);
                Sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>();
                Sounds.Play(Services.Audio.Sounds.SoundType.BubbleWallHit);
                Vector3 Direction = Vector3.right;
                if (Info == User.CollisionType.LeftBarrier)
                {
                    Direction = Vector3.left;
                }
                else if (Info == User.CollisionType.TopBarrier)
                {
                    Direction = Vector3.down;
                }
                Shaker.ApplyShake(Direction);
            }
            
            bool IsBarrier(User.CollisionType Info)
            {
                return  Info == User.CollisionType.LeftBarrier 
                     || Info == User.CollisionType.RightBarrier
                     || Info == User.CollisionType.TopBarrier;
            }
        }
        
        void PlayWallParticle(Vector3 Point, bool isLeft)
        {
            const int AngleShift = 15;
            if (WallParticleTransform == null)
            {
                WallParticleTransform = WallParticle.transform;
            }
            WallParticleTransform.position = Point;
            WallParticleTransform.eulerAngles = Vector3.forward * (AngleShift + 90 * (isLeft? -1 : 1));
            WallParticle.Emit(30);
        }
    }
}