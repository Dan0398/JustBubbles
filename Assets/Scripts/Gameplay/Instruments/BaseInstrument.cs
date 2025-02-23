using UnityEngine;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public abstract class BaseInstrument: MonoBehaviour
    {
        public System.Action AfterUse;
        [SerializeField, Range(0, 90f)] private float _angle;
        protected bool InstrumentShown;
        protected Field.BubbleField Field                                       { get; private set; }
        protected User.RayTrajectory Trajectory                                 { get; private set; }
        protected User.Action User                                              { get; private set; }
        protected Vector2 MouseClampedDirection                                 { get; private set; }
        protected readonly Vector2 OutOfViewPos = new(0, -2f);
        protected WaitForFixedUpdate Wait = new();
        protected Services.Audio.Sounds.Service Sounds  { get; private set; }
        private Vector2 _maxAngle;
        
        protected virtual float CollisionSizeMultiplier => 1f;
        protected virtual int TrajectoryCollisionsCount => 2;
        protected virtual float TrajectoryDistance => 0f;
        protected float CollisionRadius => Field.BubbleSize * 0.5f * CollisionSizeMultiplier;
        public virtual bool RequireDrawTrajectory { get; }
        float trajectoryBaseDistance = 0;
        
        public abstract void ReactOnClickDown();
        
        public abstract void ReactOnClickUp();
        
        public virtual void ReactOnAdditional() {}
        
        public abstract void ShowAnimated(float Duration = 1, System.Action OnEnd = null);
                
        public abstract void HideAnimated(float Duration = 1, System.Action OnEnd = null);
        
        public virtual void ReactOnFieldMove() => RefreshTrajectory();
        
        public virtual void ProcessAimVector(Vector2 Vector)
        {
            MouseClampedDirection = Vector.normalized;
            if (MouseClampedDirection.y <= _maxAngle.y)
            {
                MouseClampedDirection = new Vector3(_maxAngle.x * Mathf.Sign(MouseClampedDirection.x), _maxAngle.y);
            }
            RefreshTrajectory();
        }
        
        protected void RefreshTrajectory()
        {
            if (!RequireDrawTrajectory) return;
            Trajectory.ProcessRay(MouseClampedDirection);
        }
        
        public void OnValidate()
        {
            _maxAngle = new Vector2(Mathf.Sin(_angle*Mathf.Deg2Rad), Mathf.Cos(_angle * Mathf.Deg2Rad));
        }
        
        public void Init(User.Action user, Field.BubbleField field, Effects.Controller effects, User.RayTrajectory trajectory, Utils.Observables.ObsFloat rayBaseDistance)
        {
            User = user;
            Field = field;
            Trajectory = trajectory;
            RefreshTrajectory();
            rayBaseDistance.Changed += RefreshTrajectory;
            
            Sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
            OnValidate();
            
            void RefreshTrajectory() 
            {
                trajectoryBaseDistance = rayBaseDistance.Value;
                ReInitTrajectory();
            };
        }
        
        protected void ReInitTrajectory()
        {
            Trajectory.RefreshConfig(CollisionRadius, TrajectoryCollisionsCount, trajectoryBaseDistance + TrajectoryDistance);
            Trajectory.ProcessRay(MouseClampedDirection);
        }
    }
}