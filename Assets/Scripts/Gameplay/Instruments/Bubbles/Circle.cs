using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    [System.Serializable]
    public partial class Circle : BaseInstrument
    {
        private const int usualBubbleCount = 2;
        
        public bool MultiBallUsed { get; private set; }
        [SerializeField, Range(0.1f, 1f)] private float _bubbleSpeed; 
        [SerializeField, Range(0, 1.0f)] protected float _mineCollisionSizeMultiplier = 0.5f;
        [SerializeField] private float _rotateRadius;
        [Header("Sub Components"), SerializeField] private Collider2D _bubbleSwitchButton;
        [SerializeField] private SwitchAbilityIcon _userHelp;
        [SerializeField] private Pools.BubblePool _bubblePool;
        [SerializeField] private MovingBubbleEffect _movingBubbles;
        [SerializeField] private CollisionEffect _collisions;
        [SerializeField] private MultiBall _multiBall;
        private List<Gameplay.User.ICircleObject> _bubblesInCircle;
        private int _bubbleCount;
        private bool _isBubblesOnReload, _userWannaShoot;
        private Coroutine _circleRotateAnimation;

        protected override int TrajectoryCollisionsCount => 10;
        protected override float CollisionSizeMultiplier => _mineCollisionSizeMultiplier;
        public override bool RequireDrawTrajectory => !User.UsingTouch || _userWannaShoot;
        
        public override void ReactOnClickDown()
        { 
            _userWannaShoot = true;
            if (User.UsingTouch) 
            {
                Trajectory?.ProcessRay(MouseClampedDirection);
            }
            else 
            {
                TryShoot();
            }
        }
        
        public override void ReactOnClickUp()
        {
            _userWannaShoot = false;
            if (User.UsingTouch)
            {
                TryShoot();
            }
        }
        
        private void PlaceBubblesAndRecolorTrajectory()
        {
            var StepAngle = 360 / _bubblesInCircle.Count;
            
            for (int i = 0; i < _bubblesInCircle.Count; i++)
            {
                var Angle = Mathf.Repeat(StepAngle * i, 360);
                _bubblesInCircle[i].MyTransform.localPosition = Angle2LocalPos(Angle);
            }
            Trajectory.ChangeColor(_bubblesInCircle[0].TrajectoryColor.Invoke());
            TryActivateTrajectoryRecolor();
        }

        public override void ReactOnAdditional()
        {
            RotateBubbleCircle();
        }
    }
}