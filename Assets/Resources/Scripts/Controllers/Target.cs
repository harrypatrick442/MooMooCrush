using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class Target
    {
        public enum Types { Transform, Position }
        private Types Type;
        private Transform _Transform;
        private Vector2 _Position;
       public bool IsAlive
        {
            get
            {
                switch (Type)
                {
                    case Types.Transform:
                        if (_Transform != null)
                        {
                            if (_Transform.position != null)
                                return true;
                        }
                        break;
                    default:
                        return true;
                }
                return false;
            }
        }
        public Vector2 Position
        {
            get
            {
                switch (Type)
                {
                    case Types.Transform:
                        return _Transform.position;
                    default:
                        return _Position;
                }
            }
        }
        public Target(Vector2 position)
        {
            _Position = position;
            Type = Types.Position;
        }
        public Target(Transform transform)
        {
            _Transform = transform;
            Type = Types.Transform;
        }
    }
}