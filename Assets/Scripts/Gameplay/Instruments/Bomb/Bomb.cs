using UnityEngine;

namespace Gameplay.Instruments.Bomb
{
    [System.Serializable]
    public partial class Bomb : BaseInstrument
    {
        [SerializeField, Range(0,2f)] private float _bombFlySpeed;
        [SerializeField] private int _explodeRadius;
        [SerializeField] private Color _trajectoryColor;
        [SerializeField] private GameObject _bombOnScene;
        [SerializeField] private Effects _effects;
        bool _wannaShoot, _inFly;

        protected override int TrajectoryCollisionsCount => 1;

        public override bool RequireDrawTrajectory => !User.UsingTouch || _wannaShoot;
    }
}