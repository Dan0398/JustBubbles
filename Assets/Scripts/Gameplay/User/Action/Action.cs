using UnityEngine;
using Utils.Observables;

namespace Gameplay.User
{
    [AddComponentMenu("Help/User Actions")]
    public partial class Action: BaseUser<Field.BubbleField>
    {
        public bool UsingTouch          { get; private set; }
        [SerializeField] Instruments.Bubble.Circle bubble;
        [SerializeField] Instruments.Bomb.Bomb bomb;
        [SerializeField] Instruments.SniperShot sniperShot;
        [SerializeField] Instruments.Laser laser;
        [SerializeField] RayTrajectory Trajectory;
        [SerializeField] Effects.Controller Effects;
        //[SerializeField] Field.BubbleField Field;
        [SerializeField] UI.InGame.InGameCanvas InGameCanvas;
        Instruments.BaseInstrument SelectedInstrument;
        bool Started;
        ObsFloat RayBaseDistance;
        
        protected override void Start()
        {
            if (Started) return;
            Started = true;
            base.Start();
            Trajectory.Init(this, Field.TryResponseCollision);
            Field.OnFieldRefreshed += ReactOnFieldMove;
            SetupEnvironment();
            RayBaseDistance ??= 0;
            InitInstruments();
        }
        
        void InitInstruments()
        {
            bubble.Init(this, Field, Effects, Trajectory, RayBaseDistance);
            bomb.Init(this, Field, Effects, Trajectory, RayBaseDistance);
            sniperShot.Init(this, Field, Effects, Trajectory, RayBaseDistance);
            laser.Init(this, Field, Effects, Trajectory, RayBaseDistance);
        }
        
        void ReactOnFieldMove()
        {
            SelectedInstrument?.ReactOnFieldMove();
        }
        
        void SetupEnvironment()
        {
            var Env = Services.DI.Single<Services.Environment>();
            ReactOnEnvironment(Env.IsUsingTouch.Value);
            Env.IsUsingTouch.Changed += () => ReactOnEnvironment(Env.IsUsingTouch.Value);
            
            void ReactOnEnvironment(bool IsTouch)
            {
                UsingTouch = IsTouch;
            }
        }
        
        void Update()
        {
            Trajectory.ReceiveAvailableAndTryDraw(!Paused && MouseInsideField && (SelectedInstrument != null && SelectedInstrument.RequireDrawTrajectory));
        }
    }
}