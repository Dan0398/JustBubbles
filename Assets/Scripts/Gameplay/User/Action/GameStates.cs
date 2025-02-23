namespace Gameplay.User
{
    public partial class Action: BaseUser<Field.BubbleField>
    {
        public override void StartGameplayAndAnimate(float Duration = 1f)
        {
            if (!_started) Start();
            _selectedInstrument = _bubble;
            _selectedInstrument.ShowAnimated(Duration);
            _instrumentInUse = false;
            Unpause();
        }
        
        public override void StopGameplayAndAnimate(float Duration, System.Action OnEnd = null)
        {
            Pause();
            _selectedInstrument.HideAnimated(Duration, OnAnimationEnd);
            
            void OnAnimationEnd()
            {
                _selectedInstrument = null;
                OnEnd?.Invoke();
            }
        }
    }
}