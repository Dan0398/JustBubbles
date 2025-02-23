using System.Collections.Generic;
using Utils.Observables;
using UnityEngine;

namespace Gameplay.Field
{
    [System.Serializable]
    public class ColorStatistic
    {
        public System.Action<List<Bubble.BubbleColor>> OnColorRemove;
        public bool RequireFilter;
        public ObsInt ColorsCount   { get; private set; }
        public ObsInt FullCount     { get; private set; }
        [SerializeField] private Pair[] _pairs;
        private List<Bubble.BubbleColor> _availableColors;
        
        public List<Bubble.BubbleColor> AvailableColors
        {
            get => _availableColors;
            set 
            {
                _availableColors = value;
                ColorsCount.Value = AvailableColors.Count;
            }
        }
        
        public ColorStatistic()
        {
            FullCount = 0;
            ColorsCount = 0;
            var ColorValues = System.Enum.GetValues(typeof(Bubble.BubbleColor));
            _pairs = new Pair[ColorValues.Length];
            for (int i = 0; i < _pairs.Length; i ++)
            {
                _pairs[i] = new Pair((Bubble.BubbleColor)ColorValues.GetValue(i));
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
        
        private void InnerDecount(Bubble decounted)
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
        
        private void InnerIncrement(Bubble incremented)
        {
            Convert(incremented).Count++;
            FullCount.Value++;
        }
        
        private Pair Convert(Bubble target)
        {
            if (target == null) throw new System.ArgumentException("Target bubble cannot be null.");
            foreach(var pair in _pairs)
            {
                if (pair.Color == target.MyColor)
                {
                    return pair;
                }
            }
            return null;
        }
        
        private void TryFilter()
        {
            if (!RequireFilter) return;
            List<Bubble.BubbleColor> Removed = null;
            foreach(var pair in _pairs)
            {
                if (pair.Count == pair.CountBeforeCheck) continue;
                if (pair.Count == 0 && pair.CountBeforeCheck != 0)
                {
                    Removed ??= new List<Bubble.BubbleColor>(1);
                    AvailableColors.Remove(pair.Color);
                    Removed.Add(pair.Color);
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