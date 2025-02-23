using Utils.Observables;
using UnityEngine;

namespace Gameplay.User
{
    [AddComponentMenu("Help/User Actions")]
    public partial class Action: BaseUser<Field.BubbleField>
    {
        public bool UsingTouch          { get; private set; }
        [SerializeField] private Instruments.Bubble.Circle _bubble;
        [SerializeField] private Instruments.Bomb.Bomb _bomb;
        [SerializeField] private Instruments.SniperShot _sniperShot;
        [SerializeField] private Instruments.Laser _laser;
        [SerializeField] private RayTrajectory _trajectory;
        [SerializeField] private Effects.Controller _effects;
        [SerializeField] private UI.InGame.InGameCanvas _inGameCanvas;
        private Instruments.BaseInstrument _selectedInstrument;
        private bool _started;
        private ObsFloat _rayBaseDistance;
        
        protected override void Start()
        {
            if (_started) return;
            _started = true;
            base.Start();
            _trajectory.Init(this, Field.TryResponseCollision);
            Field.OnFieldRefreshed += ReactOnFieldMove;
            SetupEnvironment();
            _rayBaseDistance ??= 0;
            InitInstruments();
        }
        
        private void InitInstruments()
        {
            _bubble.Init(this, Field, _effects, _trajectory, _rayBaseDistance);
            _bomb.Init(this, Field, _effects, _trajectory, _rayBaseDistance);
            _sniperShot.Init(this, Field, _effects, _trajectory, _rayBaseDistance);
            _laser.Init(this, Field, _effects, _trajectory, _rayBaseDistance);
        }
        
        private void ReactOnFieldMove()
        {
            if (_selectedInstrument != null)
            {
                _selectedInstrument.ReactOnFieldMove();
            }
        }
        
        private void SetupEnvironment()
        {
            var Env = Services.DI.Single<Services.Environment>();
            ReactOnEnvironment(Env.IsUsingTouch.Value);
            Env.IsUsingTouch.Changed += () => ReactOnEnvironment(Env.IsUsingTouch.Value);
            
            void ReactOnEnvironment(bool IsTouch)
            {
                UsingTouch = IsTouch;
            }
        }
        
        private void Update()
        {
            _trajectory.ReceiveAvailableAndTryDraw(!Paused 
                                                && MouseInsideField 
                                                && _selectedInstrument != null 
                                                && _selectedInstrument.RequireDrawTrajectory);
        }
    }
}