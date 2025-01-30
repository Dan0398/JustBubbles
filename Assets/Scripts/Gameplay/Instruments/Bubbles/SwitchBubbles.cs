using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public partial class Circle : BaseInstrument
    {
        void RotateBubbleCircle()
        {
            LastToFirst();
            if (CircleRotateAnimation != null)
            {
                User.StopCoroutine(CircleRotateAnimation);
            }
            CircleRotateAnimation = User.StartCoroutine(AnimateCircleRotation());
            UserHelp.ReceiveUserSwitched();
            Sounds.Play(Services.Audio.Sounds.SoundType.CircleBubbleSwitch);
        }
        
        void LastToFirst()
        {
            var first = bubblesInCircle[bubblesInCircle.Count-1];
            bubblesInCircle.RemoveAt(bubblesInCircle.Count-1);
            bubblesInCircle.Insert(0, first);
        }
        
        IEnumerator AnimateCircleRotation()
        {
            const int Steps = 20;
            
            var AngleStep = 360 / bubblesInCircle.Count;
            BubblePack[] Bubbles = new BubblePack[bubblesInCircle.Count];
            for (int i = 0; i < bubblesInCircle.Count; i++)
            {
                //Bubbles[i] = new BubblePack(bubblesInCircle[i], LocalPosToAngle(bubblesInCircle[i]), i * AngleStep);
                Bubbles[i] = new BubblePack(bubblesInCircle[i], (i - 1) * AngleStep, i * AngleStep);
            }
            StartCoroutine(RecolorTrajectory(bubblesInCircle[1].TrajectoryColor, bubblesInCircle[0].TrajectoryColor, Steps));
            
            for (int step = 1; step < Steps; step ++)
            {
                float Lerp = step/(float) Steps;
                for (int i = 0; i < Bubbles.Length; i++)
                {
                    var Angle = Mathf.Lerp(Bubbles[i].OldAngle, Bubbles[i].NewAngle, Lerp);
                    Bubbles[i].Bubble.MyTransform.localPosition = Angle2LocalPos(Angle);
                }
                yield return Wait;
            }
            PlaceBubblesAndRecolorTrajectory();
            CircleRotateAnimation = null;
        }
        
        IEnumerator RecolorTrajectory(System.Func<Color> Old, System.Func<Color> New, int Steps, bool ActivateAnims = true)
        {
            if (MultiColorTrajectoryRoutine != null) StopCoroutine(MultiColorTrajectoryRoutine);
            for (int step = 1; step < Steps; step ++)
            {
                Trajectory.ChangeColor(Color.Lerp(Old.Invoke(), New.Invoke(), step/(float) Steps));
                yield return Wait;
            }
            if (ActivateAnims) TryActivateTrajectoryRecolor();
        }
        
        Coroutine MultiColorTrajectoryRoutine;
        
        void TryActivateTrajectoryRecolor()
        {
            if (MultiColorTrajectoryRoutine != null) StopCoroutine(MultiColorTrajectoryRoutine);
            if (bubblesInCircle[0] is MultiBall ball)
            {
                MultiColorTrajectoryRoutine = StartCoroutine(AnimateMultiColorTrajectory(ball));
            }
        }
        
        IEnumerator AnimateMultiColorTrajectory(MultiBall target)
        {
            while(true)
            {
                Trajectory.ChangeColor(target.TrajectoryColor.Invoke());
                yield return Wait;
            }
        }
        
        class BubblePack
        {
            public readonly Gameplay.User.ICircleObject Bubble;
            public readonly float OldAngle;
            public readonly float NewAngle;
            
            public BubblePack(Gameplay.User.ICircleObject bubble, float oldAngle, float newAngle)
            {
                Bubble = bubble;
                OldAngle = oldAngle;
                NewAngle = newAngle;
            }
        }
        
        Vector2 Angle2LocalPos(float Angle)
        {
            return new Vector2(Mathf.Sin(Angle * Mathf.Deg2Rad), Mathf.Cos(Angle * Mathf.Deg2Rad) - 1) * RotateRadius;
        }
        
        float ObjToAngle(Gameplay.User.ICircleObject obj)
        {
            var Angle = Vector2.Angle(obj.MyTransform.localPosition + Vector3.up * RotateRadius, Vector2.up);
            Angle *= Mathf.Sign(obj.MyTransform.localPosition.x);
            Angle = Mathf.Repeat(Angle, 360);
            return Angle;
        }
    }
}