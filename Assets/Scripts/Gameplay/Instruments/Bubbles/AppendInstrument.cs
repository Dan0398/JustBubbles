using System.Collections;
using Gameplay.User;
using UnityEngine;

namespace Gameplay.Instruments.Bubble
{
    public partial class Circle : BaseInstrument
    {
        internal void UseMultiBall()
        {
            MultiBallUsed = true;
            multiball.MyTransform.gameObject.SetActive(true);
            StartCoroutine(AnimateAppend(multiball));
        }
        
        IEnumerator AnimateAppend(ICircleObject appended)
        {
            const int Steps = 25;
            
            isBubblesOnReload = true;
            var AngleStep = 360 / (bubblesInCircle.Count+1);
            
            BubblePack[] Rotated = new BubblePack[bubblesInCircle.Count];
            for (int i = 1; i < Rotated.Length; i++)
            {
                Rotated[i] = new BubblePack(bubblesInCircle[i], ObjToAngle(bubblesInCircle[i]), (i) * AngleStep);
                //Rotated[i] = new BubblePack(bubblesInCircle[i], (i+1) * AngleStep, (i+2) * AngleStep);
            }
            Rotated[0] = new BubblePack(bubblesInCircle[0], ObjToAngle(bubblesInCircle[0]), (-1) * AngleStep);
            Vector2 newPos = Angle2LocalPos(0);
            
            StartCoroutine(RecolorTrajectory(bubblesInCircle[0].TrajectoryColor, appended.TrajectoryColor, Steps+1));
            
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
            
            var First = bubblesInCircle[0];
            bubblesInCircle[0] = appended;
            bubblesInCircle.Add(First);
            PlaceBubblesAndRecolorTrajectory();
            
            isBubblesOnReload = false;
        }
    }
}
