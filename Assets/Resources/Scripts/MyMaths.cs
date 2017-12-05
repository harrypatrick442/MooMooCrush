using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MyMaths
    {
        public static bool Approximately(float a, float b, float maxProportionDifference)
        {
            float difference = (a - b);
            if (difference == 0)
                return true;
            difference = difference * Mathf.Sign(difference);
            float aMag = a * Mathf.Sign(a);
            float bMag = b * Mathf.Sign(b);
            float denominator = aMag > bMag ? (bMag!=0?bMag:aMag) : (aMag!=0?aMag:bMag);
            return difference / denominator < maxProportionDifference;
        }
    }
}