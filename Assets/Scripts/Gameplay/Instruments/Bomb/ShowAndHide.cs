using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bomb
{
    public partial class Bomb : BaseInstrument
    {
        System.Action<float> ChangeBombIdleVolume;
        
        public override void ShowAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            gameObject.SetActive(true);
            BombOnScene.SetActive(true);
            inFly = false;
            ChangeBombIdleVolume = Sounds.PlayAndGiveVolumeChange(Services.Audio.Sounds.SoundType.BombIdle);
            ReInitTrajectory();
            RefreshTrajectory();
            
            StartCoroutine(AnimateShow(Duration, OnAnimationEnds));
            
            void OnAnimationEnds() => instrumentShown = true;
        }
        
        IEnumerator AnimateShow(float Duration, System.Action OnEnd, bool isInverted = false)
        {
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float Lerp = 0;
            Transform Target = BombOnScene.transform;
            
            var TrajFinalColor = TrajectoryColor;
            var TrajStartColor = TrajFinalColor - Color.black * TrajFinalColor.a;
            
            for (int Step = 0; Step <= Steps; Step++)
            {
                Lerp = Mathf.Sin(Step/(float)Steps * 90 * Mathf.Deg2Rad);
                if(isInverted) Lerp = 1 - Lerp;
                
                Trajectory.ChangeColor(Color.Lerp(TrajStartColor, TrajFinalColor, Lerp));
                ChangeBombIdleVolume.Invoke(Lerp);
                Target.localPosition = OutOfViewPos * (1-Lerp);
                
                yield return Wait;
            }
            OnEnd?.Invoke();
        }

        public override void HideAnimated(float Duration = 1, System.Action OnEnd = null)
        {
            instrumentShown = false;
            StartCoroutine(AnimateShow(Duration, AfterHide, true));
            Sounds.Stop(Services.Audio.Sounds.SoundType.BombIdle);
            //Effects.StopBombIdleSound(Duration);
            //Effects.RequireOff = true;
            
            void AfterHide()
            {
                gameObject.SetActive(false);
                OnEnd?.Invoke();
            }
        }
    }
}
