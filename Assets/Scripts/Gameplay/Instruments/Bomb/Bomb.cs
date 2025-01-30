using UnityEngine;

namespace Gameplay.Instruments.Bomb
{
    [System.Serializable]
    public partial class Bomb : BaseInstrument
    {
        [SerializeField, Range(0,2f)] float BombFlySpeed;
        [SerializeField] int ExplodeRadius;
        [SerializeField] Color TrajectoryColor;
        [SerializeField] GameObject BombOnScene;
        [SerializeField] Effects Effects;
        bool wannaShoot, inFly;

        protected override int trajectoryCollisionsCount => 1;

        public override bool RequireDrawTrajectory => !User.UsingTouch || wannaShoot;
    }
}