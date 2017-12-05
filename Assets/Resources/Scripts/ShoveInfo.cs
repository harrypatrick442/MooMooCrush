using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{public class ShoveInfo
    {
        public ShoveInfo(bool isLeft, float fingerPosition, float dT)
        {
            IsLeft = isLeft;
            FingerPosition = fingerPosition; DT = dT;
        }
        public bool IsLeft;
        public float FingerPosition;
        public float DT;
    }
}
