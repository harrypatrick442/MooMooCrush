using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class RouletteSlot<T>
    {
        public float Probability;
        public T Value;
        public RouletteSlot(float probability, T  value)
        {
            Probability = probability;
            Value = value;
        }
    }
}
