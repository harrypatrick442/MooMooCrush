using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    class WeakCollection<T> : ICollection<T>
    {
        private List<WeakReference> list = new List<WeakReference>();
        private List<WeakReference> removes = new List<WeakReference>();

        public int Count
        {
            get
            {
                DoRemoves();
                return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        private void DoRemoves()
        {
            foreach (WeakReference weakReference in list)
            {
                if (!weakReference.IsAlive)
                {
                    removes.Add(weakReference);
                }
            }
            foreach (WeakReference weakReference in removes)
                list.Remove(weakReference);
            removes.Clear();
        }
        private void DoRemovesAndAliveCallback(Action<WeakReference> callback)
        {
            foreach (WeakReference weakReference in list)
            {
                if (!weakReference.IsAlive)
                {
                    removes.Add(weakReference);
                }
                else
                {
                    callback(weakReference);
                }
            }
            foreach (WeakReference weakReference in removes)
                list.Remove(weakReference);
            removes.Clear();
        }
        public void Add(T h)
        {
            DoRemoves();
            foreach (WeakReference weakReference in list)
            {
                if (weakReference.IsAlive)
                {
                    if (((T)weakReference.Target).Equals(h))
                        return;
                }
            }
            list.Add(new WeakReference(h));
        }

        public int IndexOf(T item)
        {
            int index = -1;
            int i = 0;
            DoRemovesAndAliveCallback((WeakReference weakReference) => {
                if (((T)weakReference.Target).Equals(item))
                    index = i;
                i++;
            });
            return index;
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            bool contains = false;
            DoRemovesAndAliveCallback((WeakReference weakReference) =>
            {
                if (((T)weakReference.Target).Equals(item)) contains = true;
            });
            return contains;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            bool removed = false;
            DoRemovesAndAliveCallback((WeakReference weakReference) => {
                if (((T)weakReference.Target).Equals(item))
                {
                    removes.Add(weakReference);
                    removed = true;
                }
            });
            return removed;
        }
        bool ICollection<T>.Remove(T item)
        {
            bool removed = false;
            DoRemovesAndAliveCallback((WeakReference weakReference) => {
                if (((T)weakReference.Target).Equals(item))
                {
                    removes.Add(weakReference);
                    removed = true;
                }
            });
            return removed;
        }

        public IEnumerator<T> GetEnumerator()
        {
            List<T> ts = new List<T>();
            DoRemovesAndAliveCallback((WeakReference weakReference) => {
                ts.Add((T)weakReference.Target);
            });
            return ts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            List<T> ts = new List<T>();
            DoRemovesAndAliveCallback((WeakReference weakReference) => {
                ts.Add((T)weakReference.Target);
            });
            return ts.GetEnumerator();
        }
    }
}