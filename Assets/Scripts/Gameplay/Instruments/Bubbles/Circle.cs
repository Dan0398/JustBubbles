using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    [System.Serializable]
    public partial class Circle : BaseInstrument
    {
        const int usualBubbleCount = 2;
        
        public bool MultiBallUsed { get; private set; }
        [SerializeField, Range(0.1f, 1f)] float BubbleSpeed; 
        [SerializeField, Range(0, 1.0f)] protected float mineCollisionSizeMultiplier = 0.5f;
        [SerializeField] float RotateRadius;
        [Header("Sub Components"), SerializeField] Collider2D BubbleSwitchButton;
        [SerializeField] SwitchAbilityIcon UserHelp;
        [SerializeField] Pools.BubblePool BubblePool;
        [SerializeField] MovingBubbleEffect MovingBubbles;
        [SerializeField] CollisionEffect Collisions;
        [SerializeField] MultiBall multiball;
        List<Gameplay.User.ICircleObject> bubblesInCircle;
        int bubbleCount;
        bool isBubblesOnReload, userWannaShoot;
        Coroutine CircleRotateAnimation;

        protected override int trajectoryCollisionsCount => 10;
        protected override float collisionSizeMultiplier => mineCollisionSizeMultiplier;
        public override bool RequireDrawTrajectory => !User.UsingTouch || userWannaShoot;
        
        public override void ReactOnClickDown()
        { 
            userWannaShoot = true;
            if (User.UsingTouch) 
            {
                Trajectory?.ProcessRay(mouseClampedDirection);
            }
            else 
            {
                TryShoot();
            }
        }
        
        public override void ReactOnClickUp()
        {
            userWannaShoot = false;
            if (User.UsingTouch)
            {
                TryShoot();
            }
        }
        
        void PlaceBubblesAndRecolorTrajectory()
        {
            var StepAngle = 360 / bubblesInCircle.Count;
            
            for (int i = 0; i < bubblesInCircle.Count; i++)
            {
                var Angle = Mathf.Repeat(StepAngle * i, 360);
                bubblesInCircle[i].MyTransform.localPosition = Angle2LocalPos(Angle);
            }
            Trajectory.ChangeColor(bubblesInCircle[0].TrajectoryColor.Invoke());
            TryActivateTrajectoryRecolor();
        }

        public override void ReactOnAdditional()
        {
            RotateBubbleCircle();
        }
    }
}