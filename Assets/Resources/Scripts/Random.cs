using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Random
    {
        public static int Range(int fromInclusive, int toExclusive)
        {
            //UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            return UnityEngine.Random.Range(fromInclusive, toExclusive);
        }
        public static float Range(float fromInclusive, float toInclusive)
        {
            //UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            return UnityEngine.Random.Range(fromInclusive, toInclusive);
        }
    }
}