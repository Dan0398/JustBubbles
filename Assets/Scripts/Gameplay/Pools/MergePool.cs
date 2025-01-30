using Gameplay.Merge;
using UnityEngine;

namespace Gameplay.Pools
{
    public class MergePool : BasePool<Unit>
    {
        public int PointOfSample { get; private set; }
        
        protected override Unit ConstructNew()
        {
            var unit = Object.Instantiate(Sample).GetComponent<Unit>();
            unit.OriginalScale = Sample.transform.localScale;
            return unit;
        }
        
        public void Init(Unit sample)
        {
            if (Sample != sample.gameObject)
            {
                Clear();
            }
            Sample = sample.gameObject;
            PointOfSample = sample.Point;
        }
    }
}