using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class TouchInfoTwoFingers
    {
        public TouchInfo FirstFinger;
        public TouchInfo SecondFinger;
        public TouchInfoTwoFingers(TouchInfo firstFinger, TouchInfo secondFinger)
        {
            FirstFinger=firstFinger;
            SecondFinger = secondFinger;
        }
        private Vector2? _Distance=null;
        public Vector2 Distance
        {

            get
            {
                if (_Distance == null)
                    _Distance = SecondFinger.Position - FirstFinger.Position;
                return (Vector2)_Distance;
            }
        }
        private Vector2? _ScreenDistance;
        public Vector2 ScreenDistance
        {
            get
            {
                if (_ScreenDistance == null)
                    _ScreenDistance = SecondFinger.ScreenPosition - FirstFinger.ScreenPosition;
                return (Vector2)_ScreenDistance;
            }
        }
    }
}
