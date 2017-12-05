using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class EventDictionary<U, V>:IDictionary<U, V>
    #region Events
    {
        public delegate void DictionaryUpdateHandler(DictionaryEventArgs<U, V> e);
        public void AddEventHandler(DictionaryUpdateHandler h)
        {
            List<WeakReference> removes = new List<WeakReference>();
            foreach (WeakReference weakReference in _OnUpdate)
            {
                if (weakReference.IsAlive)
                {
                    if (((DictionaryUpdateHandler)weakReference.Target).Equals(h))
                        return;
                }
                else
                {
                    removes.Add(weakReference);
                }
            }
            foreach (WeakReference weakReference in removes)
                _OnUpdate.Remove(weakReference);
            _OnUpdate.Add(new WeakReference(h));

        }
        public void RemoveEventHandler(DictionaryUpdateHandler h)
        {
            List<WeakReference> removes = new List<WeakReference>();
            foreach (WeakReference weakReference in _OnUpdate)
            {
                if (weakReference.IsAlive)
                {
                    if (((DictionaryUpdateHandler)weakReference.Target).Equals(h))
                        removes.Add(weakReference);
                }
                else
                {
                    removes.Add(weakReference);
                }
            }
            foreach (WeakReference weakReference in removes)
                _OnUpdate.Remove(weakReference);

        }
        private List<WeakReference> _OnUpdate = new List<WeakReference>();
#endregion
        private Dictionary<U, V> _Dictionary = new Dictionary<U, V>();
        public V this[U key]
        {
            get
            {
                return _Dictionary[key];
            }
            set
            {
                _Dictionary[key] = value;
                DoCallbacks(key, value, DictionaryEventArgs<U, V>.UpdateTypes.Add);
            }
        }

        private void DoCallbacks(U key, V v, DictionaryEventArgs<U, V>.UpdateTypes updateType)
        {
            DictionaryEventArgs<U, V> dictionaryEventArgs = new DictionaryEventArgs<U, V>(key, v, updateType);
            DoCallbacks(dictionaryEventArgs);
        }
        private void DoCallbacks(DictionaryEventArgs<U, V> dictionaryEventArgs)
        {
            List<WeakReference> removes = new List<WeakReference>();
            foreach (WeakReference weakReference in _OnUpdate)
            {
                if (weakReference.IsAlive)
                {
                    try
                    {
                        ((DictionaryUpdateHandler)weakReference.Target)(dictionaryEventArgs);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
                else
                {
                    removes.Add(weakReference);
                }
            }
            foreach (WeakReference weakReference in removes)
                _OnUpdate.Remove(weakReference);
        }
        public IEnumerator<KeyValuePair<U, V>> GetEnumerator()
        {
            return _Dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Dictionary.GetEnumerator();
        }
        public ICollection<U> Keys { get { return _Dictionary.Keys; } }
        public ICollection<V> Values { get { return _Dictionary.Values; } }
        public void Add(U key, V value)
        {
            _Dictionary.Add(key, value);
            DoCallbacks(key, value, DictionaryEventArgs<U, V>.UpdateTypes.Add);
        }
        public bool ContainsKey(U key)
        {
            return _Dictionary.ContainsKey(key);
        }
        public bool Remove(U key)
        {
            V value = _Dictionary[key];
            bool r = _Dictionary.Remove(key);
            DoCallbacks(key, value, DictionaryEventArgs<U, V>.UpdateTypes.Remove);
            return r;
        }
        public bool TryGetValue(U key, out V value)
        {
            return _Dictionary.TryGetValue(key, out value);
        }




        public int Count { get { return _Dictionary.Count; } }
        public bool IsReadOnly { get { return false; } }
        public void Add(KeyValuePair<U, V> item)
        {
            _Dictionary.Add(item.Key, item.Value);
            DoCallbacks(item.Key, item.Value, DictionaryEventArgs<U, V>.UpdateTypes.Add);
        }
        public void Clear()
        {
            DictionaryEventArgs<U, V> dictionaryEventArgs = new DictionaryEventArgs<U, V>(_Dictionary.ToList(), DictionaryEventArgs<U, V>.UpdateTypes.Clear);
            _Dictionary.Clear();
            DoCallbacks(dictionaryEventArgs);
        }
        public bool Contains(KeyValuePair<U, V> item)
        {
            return _Dictionary.Contains(item);
        }
        public bool Remove(KeyValuePair<U, V> item)
        {
            U key = item.Key;
            V value = _Dictionary[key];
            bool r = _Dictionary.Remove(key);
            DoCallbacks(key, value, DictionaryEventArgs<U, V>.UpdateTypes.Remove);
            return r;
        }
        public void CopyTo(KeyValuePair<U, V>[] keyValuePair, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }

    public class DictionaryEventArgs<U, V> : EventArgs
    {
        public enum UpdateTypes { Add, Remove, Clear }
        public UpdateTypes UpdateType;
        public U Key;
        public V Value;
        public List<KeyValuePair<U, V>> List = new List<KeyValuePair<U, V>>();
        public DictionaryEventArgs(U key, V value, UpdateTypes updateType)
        {
            Key = key;
            Value = value;
         	List.Add(new KeyValuePair<U, V>(key, value));
            UpdateType = updateType;
        }
        public DictionaryEventArgs(List<KeyValuePair<U, V>> lst, UpdateTypes updateType)
        {
            List = lst;
            UpdateType = updateType;
        }
    }
}
