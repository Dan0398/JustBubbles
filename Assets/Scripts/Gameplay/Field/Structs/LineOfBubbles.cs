using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    [System.Serializable]
    public class LineOfBubbles
    {
        [field:SerializeField] public Transform OnScene { get; private set; }
        public bool Shifted     { get; private set; }
        public int MaxCapacity  { get; private set; }
        private Bubble[] _bubbles;
        private int _capacity;
        
        public Bubble this[int ID]
        {
            get => _bubbles[ID];
            set
            {
                if (value != null && _bubbles[ID] != null)
                {
                    throw new System.Exception("Bubble not null!");
                }
                if (value == null && _bubbles[ID] == null)
                {
                    throw new System.Exception("Try to clean place where bubble not exist! Place Id is " + ID);
                }
                _bubbles[ID] = value;
                value?.PlaceInLine(OnScene, ID);
            }
        }
        
        public bool RequireToClean()
        {
            for (int i=0; i< _capacity; i++)
            {
                if (_bubbles[i] != null) return false;
            }
            return true;
        }
        
        public LineOfBubbles(Transform Parent, bool Shifted, int CurrCapacity = 0)
        {
            var Obj = new GameObject("Bubble line");
            OnScene = Obj.transform;
            OnScene.SetParent(Parent);
            _capacity = CurrCapacity;
            MaxCapacity = CurrCapacity;
            _bubbles = new Bubble[CurrCapacity];
            this.Shifted = Shifted;
        }
        
        public List<Bubble> CleanLineAndGetCount(Pools.BubblePool BubblesPool)
        {
            List<Bubble> Result = new List<Bubble>(1);
            for (int i = 0; i < _bubbles.Length; i ++)
            {
                if (_bubbles[i] == null) continue;
                BubblesPool.Hide(_bubbles[i]);
                if (i < _capacity)
                {
                    Result.Add(_bubbles[i]);
                }
                _bubbles[i] = null;
            }
            Object.Destroy(OnScene.gameObject);
            return Result;
        }
        
        public void ExtendOverMax(ref Bubble[] newBubbles)
        {
            var Mine = _bubbles.Length;
            MaxCapacity = Mine + newBubbles.Length;
            System.Array.Resize(ref _bubbles, MaxCapacity);
            for (int Gain = 0; Mine < MaxCapacity; Gain++, Mine++, _capacity++)
            {
                _bubbles[Mine] = newBubbles[Gain];
                _bubbles[Mine].PlaceInLine(OnScene, Mine);
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
            while (_capacity < NewCapacity)
            {
                Extended = true;
                if (_bubbles[_capacity] != null && !_bubbles[_capacity].OnScene.activeInHierarchy)
                {
                    _bubbles[_capacity].OnScene.SetActive(true);
                    Result.Add(_bubbles[_capacity]);
                    BubbleShift++;
                }
                _capacity++;
            }
            while (_capacity > NewCapacity)
            {
                if (_bubbles[_capacity-1] != null && _bubbles[_capacity-1].OnScene.activeInHierarchy)
                {
                    Result.Add(_bubbles[_capacity-1]);
                    _bubbles[_capacity-1].OnScene.SetActive(false);
                    BubbleShift--;
                }
                _capacity--;
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