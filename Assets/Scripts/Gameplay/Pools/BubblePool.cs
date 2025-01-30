using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Pools
{
    public class BubblePool : BasePool<Bubble>
    {
        [SerializeField] Content.Bubble Skins;
        
        protected override Bubble ConstructNew() => new Bubble(Sample, Skins);
        /*
        [SerializeField] GameObject BubbleSample;
        Queue<Bubble> PooledBubbles;
        
        void Start()
        {
            PooledBubbles = new Queue<Bubble>();
        }
        
        public Bubble GiveMeBubble()
        {
            if (PooledBubbles.Count > 0)
            {
                var Result = PooledBubbles.Dequeue();
                Result.MyTransform.gameObject.SetActive(true);
                return Result;
            }
            return new Bubble(BubbleSample, Skins);
        }
        
        public void Hide(Bubble UselessBubble)
        {
            PooledBubbles.Enqueue(UselessBubble);
            UselessBubble.MyTransform.SetParent(transform);
            UselessBubble.MyTransform.gameObject.SetActive(false);
        }
        
        public void Hide(IEnumerable<Bubble> UselessBubbles)
        {
            foreach(var Bubble in UselessBubbles) Hide(Bubble);
        }
        */
    }
}