using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class DictionaryOfLists<T, U>
    {
        private Dictionary<T, List<U>> _Dictionary = new Dictionary<T, List<U>>();
        public virtual void Add(T key, U value)
        {
            if (key != null && value != null)
            {
                List<U> list;
                if (!_Dictionary.Keys.Contains(key))
                {
                    list = new List<U>();
                    _Dictionary.Add(key, list);
                }
                else
                    list = _Dictionary[key];
                if (!list.Contains(value))
                    list.Add(value);
            }
        }
        public virtual void Remove(T key, U value)
        {
            if (key != null&&value!=null)
            {
                if (_Dictionary.Keys.Contains(key))
                {
                    List<U> list = _Dictionary[key];
                    if (list.Contains(value))
                    {
                        list.Remove(value);
                        if (list.Count < 1)
                            _Dictionary.Remove(key);
                    }
                }
            }
        }
        public virtual void Remove(T key)
        {
            if (key != null)
            {
                if (_Dictionary.Keys.Contains(key))
                {
		_Dictionary.Remove(key);
                }
            }
        }
        public List<U> this[T key]
        {
            get
            {
                if (_Dictionary.ContainsKey(key))
                    return _Dictionary[key];
                return new List<U>();
            }
        }
    }

}
