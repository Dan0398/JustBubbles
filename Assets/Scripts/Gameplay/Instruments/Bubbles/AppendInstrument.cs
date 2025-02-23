using System.Collections;
using Gameplay.User;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public partial class Circle : BaseInstrument
    {
        public void UseMultiBall()
        {
            MultiBallUsed = true;
            _multiBall.MyTransform.gameObject.SetActive(true);
            StartCoroutine(AnimateAppend(_multiBall));
        }
        
        private IEnumerator AnimateAppend(ICircleObject appended)
        {
            const int Steps = 25;
            
            _isBubblesOnReload = true;
            var AngleStep = 360 / (_bubblesInCircle.Count+1);
            
            BubblePack[] Rotated = new BubblePack[_bubblesInCircle.Count];
            for (int i = 1; i < Rotated.Length; i++)
            {
                Rotated[i] = new BubblePack(_bubblesInCircle[i], ObjToAngle(_bubblesInCircle[i]), (i) * AngleStep);
            }
            Rotated[0] = new BubblePack(_bubblesInCircle[0], ObjToAngle(_bubblesInCircle[0]), (-1) * AngleStep);
            Vector2 newPos = Angle2LocalPos(0);
            
            StartCoroutine(RecolorTrajectory(_bubblesInCircle[0].TrajectoryColor, appended.TrajectoryColor, Steps+1));
            
            for (int i = 1; i < Steps; i ++)
            {
                float Lerp = i/(float)Steps;
                for (int k = 0; k < Rotated.Length; k++)
                {
                    var Angle = Mathf.Lerp(Rotated[k].OldAngle, Rotated[k].NewAngle, Lerp);
                    Rotated[k].Bubble.MyTransform.localPosition = Angle2LocalPos(Angle);
                }
                appended.MyTransform.localPosition = Vector2.Lerp(OutOfViewPos, newPos, EasingFunction.EaseOutSine(0,1,Lerp));
                yield return Wait;
            }
            
            var First = _bubblesInCircle[0];
            _bubblesInCircle[0] = appended;
            _bubblesInCircle.Add(First);
            PlaceBubblesAndRecolorTrajectory();
            
            _isBubblesOnReload = false;
        }
    }
}