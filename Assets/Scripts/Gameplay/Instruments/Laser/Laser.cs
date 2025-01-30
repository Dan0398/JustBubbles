using System.Collections.Generic;
using Services.Audio;
using UnityEngine;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public partial class Laser : BaseInstrument
    {
        [SerializeField] int BubbleResistFrames;
        [SerializeField] float MaxEnergy, Energy;
        [Space(), SerializeField] Transform laserOnScene;
        [Header("Burn Effects"), SerializeField] Transform burnPoint;
        [SerializeField] ParticleSystem burnEdge;
        [SerializeField] ParticleSystem burnSmoke;
        [SerializeField] ParticleSystem laserSmoke;
        [SerializeField] LineRenderer burnLine;
        List<DamagedBubble> underAttack;
        SpriteRenderer laserSpite;
        Gameplay.User.Trajectory traj;
        DamagedBubble TargetBubble;
        Coroutine processRoutine;
        Vector3 burnWorldPos;
        int oldLinesCount;
        bool IsSlicing;
        
        public override bool RequireDrawTrajectory => false;

        protected override int trajectoryCollisionsCount => 1;
        protected override float collisionSizeMultiplier => 0.5f;
    }
}