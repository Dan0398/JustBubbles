using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Pools
{
    public abstract class BasePool<T> : MonoBehaviour where T:IWithTransform
    {
        [SerializeField] protected GameObject Sample;
        //[SerializeField] Skinned.Bubble Skins;
        Queue<T> PooledBubbles;
        
        void Start()
        {
            PooledBubbles ??= new Queue<T>();
        }
        
        public T GiveItem()
        {
            if (PooledBubbles != null && PooledBubbles.Count > 0)
            {
                var Result = PooledBubbles.Dequeue();
                Result.MyTransform.gameObject.SetActive(true);
                return Result;
            }
            return ConstructNew();
        }
        
        protected abstract T ConstructNew();
        
        public void Hide(T UselessObject)
        {
            PooledBubbles.Enqueue(UselessObject);
            UselessObject.MyTransform.SetParent(transform);
            UselessObject.MyTransform.gameObject.SetActive(false);
        }
        
        public void Hide(IEnumerable<T> UselessBubbles)
        {
            foreach(var Bubble in UselessBubbles) Hide(Bubble);
        }
        
        protected void Clear()
        {
            PooledBubbles ??= new Queue<T>();
            for (int i = 0; i < PooledBubbles.Count; i++)
            {
                GameObject.Destroy(PooledBubbles.Dequeue().MyTransform.gameObject);
            }
            PooledBubbles.Clear();
        }
    }
}