using UnityEngine;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public abstract class BaseInstrument: MonoBehaviour
    {
        public System.Action AfterUse;
        [SerializeField, Range(0, 90f)] float Angle;
        protected bool instrumentShown;
        protected Field.BubbleField Field                                       { get; private set; }
        protected User.RayTrajectory Trajectory                                 { get; private set; }
        protected User.Action User                                              { get; private set; }
        protected Vector2 mouseClampedDirection                                 { get; private set; }
        protected readonly Vector2 OutOfViewPos = new Vector2(0, -2f);
        protected WaitForFixedUpdate Wait = new();
        protected Services.Audio.Sounds.Service Sounds  { get; private set; }
        Vector2 MaxAngle;
        
        protected virtual float collisionSizeMultiplier => 1f;
        protected virtual int trajectoryCollisionsCount => 2;
        protected virtual float trajectoryDistance => 0f;
        protected float CollisionRadius => Field.BubbleSize * 0.5f * collisionSizeMultiplier;
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
            mouseClampedDirection = Vector.normalized;
            if (mouseClampedDirection.y <= MaxAngle.y)
            {
                mouseClampedDirection = new Vector3(MaxAngle.x * Mathf.Sign(mouseClampedDirection.x), MaxAngle.y);
            }
            RefreshTrajectory();
        }
        
        protected void RefreshTrajectory()
        {
            if (!RequireDrawTrajectory) return;
            Trajectory.ProcessRay(mouseClampedDirection);
        }
        
        public void OnValidate()
        {
            MaxAngle = new Vector2(Mathf.Sin(Angle*Mathf.Deg2Rad), Mathf.Cos(Angle * Mathf.Deg2Rad));
        }
        
        public void Init(User.Action user, Field.BubbleField field, Effects.Controller effects, User.RayTrajectory trajectory, Utils.Observables.ObsFloat rayBaseDistance)
        {
            User = user;
            Field = field;
            Trajectory = trajectory;
            //Effects = effects;
            System.Action Refresh = () => 
            {
                trajectoryBaseDistance = rayBaseDistance.Value;
                ReInitTrajectory();
            };
            Refresh.Invoke();
            rayBaseDistance.Changed += Refresh;
            
            Sounds = Services.DI.Single<Services.Audio.Sounds.Service>();
            OnValidate();
        }
        
        protected void ReInitTrajectory()
        {
            Trajectory.RefreshConfig(CollisionRadius, trajectoryCollisionsCount, trajectoryBaseDistance + trajectoryDistance);
            Trajectory.ProcessRay(mouseClampedDirection);
        }
    }
}