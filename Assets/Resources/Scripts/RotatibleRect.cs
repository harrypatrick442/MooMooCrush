using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class RotatibleRect
    {
        private float _X;
        public float X
        {
            get
            {

                return _X;
            }
            set
            {
                _X = value;
            }
        }
        private float _Y;
        public float Y
        {
            get
            {

                return _Y;
            }
            set
            {
                _Y = value;
            }
        }
        private float _Height;
        public float Height
        {
            get
            {

                return _Height;
            }
            set
            {
                _Height = value;
            }
        }
        private float _Width;
        public float Width
        {
            get
            {

                return _Width;
            }
            set
            {
                _Width = value;
            }
        }
        private float _Angle;
        public float Angle
        {
            get
            {

                return _Angle;
            }
            set
            {
                _Angle = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return new Vector2(X, Y);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">middle x</param>
        /// <param name="y">middle y</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"> in degrees</param>

        public RotatibleRect(float x, float y, float width, float height, float angle)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        private Vector3 MoveAroundPointWithAngle(Vector3 wantedP, Vector3 aroundP, float angle)
        {
            // Quaternion r = Quaternion.LookRotation(wantedP - aroundP);
            //Vector3 newP = wantedP * r;
            throw new NotImplementedException();// newP;

        }
        public bool Contains(Vector2 point)
        {
            Vector2 rotatedPoint = MoveAroundPointWithAngle(point, Position, -Angle);
            return new Rect(X, Y, Width, Height).Contains(rotatedPoint);
        }
    }
}
                