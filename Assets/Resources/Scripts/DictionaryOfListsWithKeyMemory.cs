using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class DictionaryOfListsWithKeyMemory<T, U>:DictionaryOfLists<T, U>
    {
        private DictionaryOfLists<U, T> _KeyMemory = new DictionaryOfLists<U, T>();
        public override void Add(T key, U value)
        {
            if (key != null && value != null)
            {
                _KeyMemory.Add(value, key);
                base.Add(key, value);
            }
        }
        public override void Remove(T key, U value)
        {
            if (key != null && value != null)
            {
                _KeyMemory.Remove(value, key);
                base.Remove(key, value);




            }
        }
        public override void Remove(T key)
        {
            if (key != null)
            {
                List<U> values = base[key];
                    foreach (U value in values)
                        _KeyMemory.Remove(value, key);
                base.Remove(key);
            }
        }
        public void Remove(U value)
        {
            if (value != null)
            {
                List<T> keys = _KeyMemory[value];
                    foreach (T key in keys)
                        base.Remove(key, value);
                _KeyMemory.Remove(value);
            }
        }
    }

}
