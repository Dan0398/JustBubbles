using System.Collections;
using UnityEngine;
using System;

namespace Gameplay.Merge
{
    public class ExplodedUnit: IComparable
    {
        public readonly Unit Target;
        public readonly float Distance;
        
        public ExplodedUnit(Unit Target, float Distance)
        {
            this.Target = Target;
            this.Distance = Distance;
        }

        public int CompareTo(object obj)
        {        
            if(obj == null) throw new System.NullReferenceException();
            if(obj is ExplodedUnit other) return Mathf.RoundToInt(Mathf.Sign(Distance - other.Distance));
            else throw new ArgumentException("Некорректное значение параметра");
        }
        
        public IEnumerator AnimateExplode(WaitForFixedUpdate Wait, System.Action<Unit> AfterEnd)
        {
            const int Steps = 5;
            var renderer = Target.GetComponent<SpriteRenderer>();
            var oldColor = renderer.color;
            for (int i = 0; i <= Steps; i++)
            {
                var Lerp = EasingFunction.EaseInSine(0,1, i/(float)Steps);
                renderer.color = Color.Lerp (oldColor, Color.black, Lerp);
                yield return Wait;
            }
            yield return Wait;
            yield return Wait;
            for (int i = 0; i <= Steps; i++)
            {
                var Lerp = EasingFunction.EaseInSine(0,1, i/(float)Steps);
                renderer.color = Color.black * (1 - Lerp);
                yield return Wait;
            }
            renderer.color = oldColor;
            Target.gameObject.SetActive(false);
            AfterEnd?.Invoke(Target);
        }
    }
}