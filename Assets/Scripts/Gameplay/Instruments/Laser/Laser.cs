using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public partial class Laser : BaseInstrument
    {
        [SerializeField] private int _bubbleResistFrames;
        [SerializeField] private float _maxEnergy;
        [SerializeField] private float _energy;
        [Space(), SerializeField] private Transform _laserOnScene;
        [Header("Burn Effects"), SerializeField] private Transform _burnPoint;
        [SerializeField] private ParticleSystem _burnEdge;
        [SerializeField] private ParticleSystem _burnSmoke;
        [SerializeField] private ParticleSystem _laserSmoke;
        [SerializeField] private LineRenderer _burnLine;
        private List<DamagedBubble> _underAttack;
        private SpriteRenderer _laserSpite;
        private Gameplay.User.Trajectory _trajectory;
        private DamagedBubble _targetBubble;
        private Vector3 _burnWorldPos;
        private int _oldLinesCount;
        private bool _isSlicing;
        
        public override bool RequireDrawTrajectory => false;

        protected override int TrajectoryCollisionsCount => 1;
        
        protected override float CollisionSizeMultiplier => 0.5f;
    }
}