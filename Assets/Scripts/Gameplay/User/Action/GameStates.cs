namespace Gameplay.User
{
    public partial class Action: BaseUser<Field.BubbleField>
    {
        public override void StartGameplayAndAnimate(float Duration = 1f)
        {
            if (!Started) Start();
            SelectedInstrument = bubble;
            SelectedInstrument.ShowAnimated(Duration);
            instrumentInUse = false;
            Unpause();
        }
        
        public override void StopGameplayAndAnimate(float Duration, System.Action OnEnd = null)
        {
            Pause();
            SelectedInstrument.HideAnimated(Duration, OnAnimationEnd);
            
            void OnAnimationEnd()
            {
                SelectedInstrument = null;
                OnEnd?.Invoke();
            }
        }
    }
}