using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class SwipingInfoTwoFingers
    {
        public SwipingInfo FirstFinger;
        public SwipingInfo SecondFinger;
        private Vector2? _Distance;
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
                    _ScreenDistance = SecondFinger.PositionScreen - FirstFinger.PositionScreen;
                return (Vector2)_ScreenDistance;
            }
        }
        private    Vector2? _StartDistance;
        public Vector2 StartDistance
        {

            get
            {
                if (_StartDistance == null)
                    _StartDistance = SecondFinger.StartPosition-FirstFinger.StartPosition;
                return (Vector2)_StartDistance;
            }
        }
        private Vector2? _TotalChangeDistance;
        public Vector2 TotalChangeDistance
        {
            get
            {
                if (_TotalChangeDistance == null)
                    _TotalChangeDistance = Distance - StartDistance;
                return (Vector2)_TotalChangeDistance;
            }
        }
        private float? _TotalChangeDistanceDirectional;
        public float TotalChangeDistanceDirectional
        {
            get
            {
                if (_TotalChangeDistanceDirectional == null)
                    _TotalChangeDistanceDirectional = Distance.magnitude - StartDistance.magnitude;
                return (float)_TotalChangeDistanceDirectional;
            }
        }
        public SwipingInfoTwoFingers(SwipingInfo swipingInfoFirstFinger, SwipingInfo swipingInfoSecondFinger)
        {
            FirstFinger = swipingInfoFirstFinger;
            SecondFinger = swipingInfoSecondFinger;
        }
    }
}
