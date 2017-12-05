using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
namespace Assets.Scripts
{
    class StopWatch
    {
        float startTime;
        public StopWatch()
        {
            startTime = Time.time;
        }
        public void Reset()
        {
            startTime = Time.time;
        }
        public float GetS()
        {
            return Time.time - startTime;
        }
        public float GetMilliseconds()
        {
            return 1000 * (Time.time - startTime);
        }
    }
}
