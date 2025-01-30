using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Observables
{
    [System.Serializable]
    public class ObsList<T> : IList<T>
    {
        public delegate void ChangedDelegate(int index, T oldValue, T newValue);

        [SerializeField] private List<T> _list = new List<T>();

        // NOTE: I changed the signature to provide a bit more information
        // now it returns index, oldValue, newValue
        public event ChangedDelegate Changed;

        public event Action Updated;
        
        public event Action<IEnumerable<T>> OnAdded, OnRemoved;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _list.Add(item);
            Updated?.Invoke();
            OnAdded?.Invoke(new List<T>{item});
        }

        public void Clear()
        {
            _list.Clear();
            Updated?.Invoke();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var output = _list.Remove(item);
            Updated?.Invoke();
            OnRemoved?.Invoke(new List<T>{item});

            return output;
        }

        public int Count => _list.Count;
        
        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            Updated?.Invoke();
            OnAdded?.Invoke(new List<T>{item});
        }

        public void RemoveAt(int index)
        {
            OnRemoved?.Invoke(new List<T>{_list[index]});
            _list.RemoveAt(index);
            Updated?.Invoke();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _list.AddRange(collection);
            Updated?.Invoke();
            OnAdded?.Invoke(collection);
        }

        public void RemoveAll(Predicate<T> predicate)
        {
            OnRemoved?.Invoke(_list);
            _list.RemoveAll(predicate);
            Updated?.Invoke();
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            _list.InsertRange(index, collection);
            Updated?.Invoke();
            OnAdded?.Invoke(collection);
        }

        public void RemoveRange(int index, int count)
        {
            LinkedList<T> forRemove = new LinkedList<T>();
            for (int i=0; i< count; i++)
            {
                forRemove.AddLast(_list[index + i]);
            }
            OnRemoved?.Invoke(forRemove);
            _list.RemoveRange(index, count);
            Updated?.Invoke();
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                var oldValue = _list[index];
                _list[index] = value;
                Changed?.Invoke(index, oldValue, value);
                // I would also call the generic one here
                Updated?.Invoke();
            }
        }
    }
}