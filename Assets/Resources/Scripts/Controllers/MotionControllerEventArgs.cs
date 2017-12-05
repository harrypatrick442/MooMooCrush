using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public delegate bool MotionControllerEventHandler(MotionControllerEventArgs e);
    public class MotionControllerEventArgs: EventArgs
    {
        private Types _Type;
        public enum Types { ReachedTarget, OrbitAngleChanged}
        public Types Type
        {
            get
            {
                return _Type;
            }
        }
        private Vector2 _Position;
        public Vector2 Position
        {
            get
            {
                return _Position;
            }
        }
        private Angle _Angle;
        public Angle Angle
        {
            get
            {
                return _Angle;
            }
        }
        public MotionControllerEventArgs(Vector2 position)
        {
            _Type = Types.ReachedTarget;
            _Position = position;
        }
        public MotionControllerEventArgs(Angle angle)
        {
            _Type = Types.OrbitAngleChanged;
            _Angle = angle;
        }
    }
}