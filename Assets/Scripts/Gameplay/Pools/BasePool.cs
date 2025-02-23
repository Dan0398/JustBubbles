using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Pools
{
    public abstract class BasePool<T> : MonoBehaviour where T:IWithTransform
    {
        [SerializeField] protected GameObject Sample;
        private Queue<T> _pooledBubbles;
        
        private void Start()
        {
            _pooledBubbles ??= new Queue<T>();
        }
        
        public T GiveItem()
        {
            if (_pooledBubbles != null && _pooledBubbles.Count > 0)
            {
                var Result = _pooledBubbles.Dequeue();
                Result.MyTransform.gameObject.SetActive(true);
                return Result;
            }
            return ConstructNew();
        }
        
        protected abstract T ConstructNew();
        
        public void Hide(T UselessObject)
        {
            _pooledBubbles.Enqueue(UselessObject);
            UselessObject.MyTransform.SetParent(transform);
            UselessObject.MyTransform.gameObject.SetActive(false);
        }
        
        public void Hide(IEnumerable<T> UselessBubbles)
        {
            foreach(var Bubble in UselessBubbles) Hide(Bubble);
        }
        
        protected void Clear()
        {
            _pooledBubbles ??= new Queue<T>();
            for (int i = 0; i < _pooledBubbles.Count; i++)
            {
                Destroy(_pooledBubbles.Dequeue().MyTransform.gameObject);
            }
            _pooledBubbles.Clear();
        }
    }
}