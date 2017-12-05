using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    class RouletteWheel<T>
    {
        private Action<T> _OnSpin;
        public Action<T> OnSpin
        {
            get
            {
                return _OnSpin;
            }
            set
            {
                _OnSpin = value;
            }
        }
        private RouletteSlot<T>[] _Slots;
     public RouletteWheel(RouletteSlot<T>[] slots)
        {
            _Slots = slots;
        }
        public T Spin()
        {
            float percent = Random.Range(0f, 100f);
            float totalPercent = 0;
            int i = 0;
            while(i<_Slots.Length)
            {
                totalPercent += _Slots[i].Probability;
                if(totalPercent>percent)
                {
                    return _Slots[i].Value;
                }
                i = i + 1;
            }
            T value = _Slots[_Slots.Length - 1].Value;
            if (_OnSpin != null)
                _OnSpin(value);
            return value;
        }
    }
}
