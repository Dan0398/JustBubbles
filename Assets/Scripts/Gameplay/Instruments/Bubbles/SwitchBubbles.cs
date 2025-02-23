using System.Collections;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public partial class Circle : BaseInstrument
    {
        private Coroutine _multiColorTrajectoryRoutine;
        
        private void RotateBubbleCircle()
        {
            LastToFirst();
            if (_circleRotateAnimation != null)
            {
                User.StopCoroutine(_circleRotateAnimation);
            }
            _circleRotateAnimation = User.StartCoroutine(AnimateCircleRotation());
            _userHelp.ReceiveUserSwitched();
            Sounds.Play(Services.Audio.Sounds.SoundType.CircleBubbleSwitch);
        }
        
        private void LastToFirst()
        {
            var first = _bubblesInCircle[_bubblesInCircle.Count-1];
            _bubblesInCircle.RemoveAt(_bubblesInCircle.Count-1);
            _bubblesInCircle.Insert(0, first);
        }
        
        private IEnumerator AnimateCircleRotation()
        {
            const int Steps = 20;
            
            var AngleStep = 360 / _bubblesInCircle.Count;
            var Bubbles = new BubblePack[_bubblesInCircle.Count];
            for (int i = 0; i < _bubblesInCircle.Count; i++)
            {
                Bubbles[i] = new BubblePack(_bubblesInCircle[i], (i - 1) * AngleStep, i * AngleStep);
            }
            StartCoroutine(RecolorTrajectory(_bubblesInCircle[1].TrajectoryColor, _bubblesInCircle[0].TrajectoryColor, Steps));
            
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
            _circleRotateAnimation = null;
        }
        
        private IEnumerator RecolorTrajectory(System.Func<Color> Old, System.Func<Color> New, int Steps, bool ActivateAnims = true)
        {
            if (_multiColorTrajectoryRoutine != null) StopCoroutine(_multiColorTrajectoryRoutine);
            for (int step = 1; step < Steps; step ++)
            {
                Trajectory.ChangeColor(Color.Lerp(Old.Invoke(), New.Invoke(), step/(float) Steps));
                yield return Wait;
            }
            if (ActivateAnims) TryActivateTrajectoryRecolor();
        }
        
        void TryActivateTrajectoryRecolor()
        {
            if (_multiColorTrajectoryRoutine != null) StopCoroutine(_multiColorTrajectoryRoutine);
            if (_bubblesInCircle[0] is MultiBall ball)
            {
                _multiColorTrajectoryRoutine = StartCoroutine(AnimateMultiColorTrajectory(ball));
            }
        }
        
        private IEnumerator AnimateMultiColorTrajectory(MultiBall target)
        {
            while(true)
            {
                Trajectory.ChangeColor(target.TrajectoryColor.Invoke());
                yield return Wait;
            }
        }
        
        private Vector2 Angle2LocalPos(float Angle)
        {
            return new Vector2(Mathf.Sin(Angle * Mathf.Deg2Rad), Mathf.Cos(Angle * Mathf.Deg2Rad) - 1) * _rotateRadius;
        }
        
        private float ObjToAngle(Gameplay.User.ICircleObject obj)
        {
            var Angle = Vector2.Angle(obj.MyTransform.localPosition + Vector3.up * _rotateRadius, Vector2.up);
            Angle *= Mathf.Sign(obj.MyTransform.localPosition.x);
            Angle = Mathf.Repeat(Angle, 360);
            return Angle;
        }
    }
}