using System.Collections.Generic;
using Utils.Observables;
using UnityEngine;

namespace Gameplay.Field
{
    [System.Serializable]
    public class ColorStatistic
    {
        public System.Action<List<Bubble.BubbleColor>> OnColorRemove;
        List<Bubble.BubbleColor> availableColors;
        public bool RequireFilter;
        public List<Bubble.BubbleColor> AvailableColors
        {
            get => availableColors;
            set 
            {
                availableColors = value;
                ColorsCount.Value = AvailableColors.Count;
            }
        }
        public ObsInt ColorsCount   { get; private set; }
        public ObsInt FullCount     { get; private set; }
        [SerializeField] Pair[] pairs;
        
        public ColorStatistic()
        {
            FullCount = 0;
            ColorsCount = 0;
            var ColorValues = System.Enum.GetValues(typeof(Bubble.BubbleColor));
            pairs = new Pair[ColorValues.Length];
            for (int i = 0; i < pairs.Length; i ++)
            {
                pairs[i] = new Pair((Bubble.BubbleColor)ColorValues.GetValue(i));
            }
        }
        
        public void DecountByBubble(IEnumerable<Bubble> underHide)
        {
            foreach(var bubble in underHide)
            {
                InnerDecount(bubble);
            }
            TryFilter();
        }
        
        public void DecountByBubble(Bubble underHide)
        {
            InnerDecount(underHide);
            TryFilter();
        }
        
        void InnerDecount(Bubble decounted)
        {
            Convert(decounted).Count--;
            FullCount.Value--;
        }
        
        public void IncrementByBubble(IEnumerable<Bubble> underShow)
        {
            foreach(var bubble in underShow)
            {
                InnerIncrement(bubble);
            }
            TryFilter();
        }
        
        public void IncrementByBubble(Bubble underShow)
        {
            InnerIncrement(underShow);
            TryFilter();
        }
        
        void InnerIncrement(Bubble inremented)
        {
            Convert(inremented).Count++;
            FullCount.Value++;
        }
        
        Pair Convert(Bubble target)
        {
            if (target == null) throw new System.Exception("!!");
            foreach(var pair in pairs)
            {
                if (pair.Color == target.MyColor)
                {
                    return pair;
                }
            }
            return null;
        }
        
        void TryFilter()
        {
            if (!RequireFilter) return;
            List<Bubble.BubbleColor> Removed = null;
            //bool Filtered = false;
            foreach(var pair in pairs)
            {
                if (pair.Count == pair.CountBeforeCheck) continue;
                if (pair.Count == 0 && pair.CountBeforeCheck != 0)
                {
                    if (Removed == null) Removed = new List<Bubble.BubbleColor>(1);
                    AvailableColors.Remove(pair.Color);
                    Removed.Add(pair.Color);
                    //Filtered = true;
                }
                pair.CountBeforeCheck = pair.Count;
            }
            if (Removed != null)
            {
                OnColorRemove?.Invoke(Removed);
                ColorsCount.Value = AvailableColors.Count;
            }
        }
        
        [System.Serializable]
        public class Pair
        {
            [field:SerializeField] public Bubble.BubbleColor Color { get; private set; }
            public int Count, CountBeforeCheck;
            
            public Pair(Bubble.BubbleColor color)
            {
                Color = color;
                Count = 0;
                CountBeforeCheck = 0;
            }
        }
    }
}