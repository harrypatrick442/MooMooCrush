using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class Tuple<T, U>
    {
        public T A;
        public U B;
        public Tuple(T t, U u)
        {
            A = t;
            B = u;
        }
    }
    class Tuple<T, U, V>
    {
        public T A;
        public U B;
        public V C;
        public Tuple(T t, U u, V v)
        {
            A = t;
            B = u;
            C = v;
        }
    }
}
