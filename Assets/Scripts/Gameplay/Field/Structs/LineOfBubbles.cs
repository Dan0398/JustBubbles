using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    [System.Serializable]
    public class LineOfBubbles
    {
        [field:SerializeField] public Transform OnScene { get; private set; }
        Bubble[] Bubbles;
        public bool Shifted     { get; private set; }
        public int MaxCapacity  { get; private set; }
        int Capacity;
        
        public Bubble this[int ID]
        {
            get => Bubbles[ID];
            set
            {
                if (value != null && Bubbles[ID] != null)
                {
                    throw new System.Exception("Bubble not null!");
                }
                if (value == null && Bubbles[ID] == null)
                {
                    throw new System.Exception("Try to clean place where bubble not exist! Place Id is " + ID);
                }
                Bubbles[ID] = value;
                value?.PlaceInLine(OnScene, ID);
            }
        }
        
        public bool RequireToClean()
        {
            for (int i=0; i< Capacity; i++)
            {
                if (Bubbles[i] != null) return false;
            }
            return true;
        }
        
        public LineOfBubbles(Transform Parent, bool Shifted, int CurrCapacity = 0)
        {
            var Obj = new GameObject("Bubble line");
            OnScene = Obj.transform;
            OnScene.SetParent(Parent);
            Capacity = CurrCapacity;
            MaxCapacity = CurrCapacity;
            Bubbles = new Bubble[CurrCapacity];
            this.Shifted = Shifted;
        }
        
        public List<Bubble> CleanLineAndGetCount(Pools.BubblePool BubblesPool)
        {
            List<Bubble> Result = new List<Bubble>(1);
            for (int i = 0; i < Bubbles.Length; i ++)
            {
                if (Bubbles[i] == null) continue;
                BubblesPool.Hide(Bubbles[i]);
                if (i < Capacity)
                {
                    Result.Add(Bubbles[i]);
                }
                Bubbles[i] = null;
            }
            Object.Destroy(OnScene.gameObject);
            return Result;
        }
        
        public void ExtendOverMax(ref Bubble[] newBubbles)
        {
            var Mine = Bubbles.Length;
            MaxCapacity = Mine + newBubbles.Length;
            System.Array.Resize(ref Bubbles, MaxCapacity);
            for (int Gain = 0; Mine < MaxCapacity; Gain++, Mine++, Capacity++)
            {
                Bubbles[Mine] = newBubbles[Gain];
                Bubbles[Mine].PlaceInLine(OnScene, Mine);
            }
        }
        
        public ResizeResult GetTrimmedAfterTryTrimCapacity(int NewCapacity)
        {
            if (MaxCapacity < NewCapacity)
            {
                throw new System.Exception($"New capacity({NewCapacity}) is more then max ({MaxCapacity})");
            }
            bool Extended = false;
            List<Bubble> Result = new(1);
            int BubbleShift = 0;
            while (Capacity < NewCapacity)
            {
                Extended = true;
                if (Bubbles[Capacity] != null && !Bubbles[Capacity].OnScene.activeInHierarchy)
                {
                    Bubbles[Capacity].OnScene.SetActive(true);
                    Result.Add(Bubbles[Capacity]);
                    BubbleShift++;
                }
                Capacity++;
            }
            while (Capacity > NewCapacity)
            {
                if (Bubbles[Capacity-1] != null && Bubbles[Capacity-1].OnScene.activeInHierarchy)
                {
                    Result.Add(Bubbles[Capacity-1]);
                    Bubbles[Capacity-1].OnScene.SetActive(false);
                    BubbleShift--;
                }
                Capacity--;
            }
            return new ResizeResult(){Added = Result, Extended = Extended};
        }
        
        public class ResizeResult
        {
            public List<Bubble> Added;
            public bool Extended;
        }
    }
}