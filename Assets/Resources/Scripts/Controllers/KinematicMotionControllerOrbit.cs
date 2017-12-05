using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class KinematicMotionControllerOrbit:IMotionControllerRadual
    {
        private ILooper _ILooper;
        public ILooper ILooper { set { _ILooper = value; } }
        private Transform _Transform;
        private Func<bool> Update;
        private float _StartAngle;
        private Vector2 _Centre;
        private float _StartRadius;
        private float _DRadius;
        private float _DAngle;
        private float _StartTime;
        private float _SpeedMultiplier;
        private Action<Angle> _Callback;
        private Action _ReachedTarget;
        public KinematicMotionControllerOrbit(ILooper iLooper, Transform transform)
        {
            _ILooper = iLooper;
            _Transform = transform;
            if(_Transform!=null)
                Update = new Func<bool>(() =>
                {
                    bool done = false;
                    float proportionComplete = (Time.time -_StartTime)*_SpeedMultiplier;
                    if(proportionComplete >=1)
                    {
                        proportionComplete = 1;
                        _ReachedTarget();
                        done = true;
                    }
                    float currentAngleDegrees = _StartAngle + (_DAngle * proportionComplete);
                    float currentAngle = currentAngleDegrees * Mathf.PI / 180;
                    float radius = _StartRadius + (_DRadius * proportionComplete);
                    if (_Transform != null)
                        _Transform.position = new Vector3(_Centre.x + (radius * Mathf.Sin(currentAngle)), _Centre.y + (Mathf.Cos(currentAngle) * radius), _Transform.position.z);
                    _Callback(new Scripts.Angle(currentAngle));
                    return done;
                });
        }
        public void Orbit(Vector2 point, float angularVelocityDegreesPerSecond, float toAngleDegrees, bool clockwise, float endRadius, Action<Angle> callback, Action reachedTarget)
        {
            _ReachedTarget = reachedTarget==null?new Action(()=> { }): reachedTarget;
            _Centre = point;
            _Callback = callback==null?new Action<Angle>((angle)=> { }): callback;
            angularVelocityDegreesPerSecond = clockwise ? (angularVelocityDegreesPerSecond > 0 ? angularVelocityDegreesPerSecond : -angularVelocityDegreesPerSecond) : (angularVelocityDegreesPerSecond > 0 ? -angularVelocityDegreesPerSecond : angularVelocityDegreesPerSecond);
            _StartRadius = (_Centre - (Vector2)_Transform.position).magnitude;
            _DRadius = endRadius - _StartRadius;
            Vector2 direction = ((Vector2)_Transform.position - point);
            _StartAngle = (180 * Mathf.Atan2(direction.x, direction.y) / Mathf.PI);
            _DAngle = MyGeometry.GetAngleDifferenceDegrees(_StartAngle, toAngleDegrees, clockwise);
            _SpeedMultiplier = angularVelocityDegreesPerSecond / _DAngle;
            _SpeedMultiplier = Mathf.Sign(_SpeedMultiplier) * _SpeedMultiplier;
            _StartTime = Time.time;
            _ILooper.AddFixedUpdate(this);
        }
        public void Cancel()
        {
            _ILooper.RemoveFixedUpdate(this);
        }
        public bool LooperFixedUpdate()
        { 
            return Update();
        }
        public void Dispose()
        {
            if(_ILooper!=null)
                _ILooper.RemoveFixedUpdate(this);
        }
    }
}