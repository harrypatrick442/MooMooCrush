using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class DynamicMotionController :IMotionController
    {
        #region Events
        public void AddEventHandlerReachedTarget(MotionControllerEventHandler h)
        {
            _OnReachedTarget.Add(h);
        }
        public void RemoveEventHandlerReachedTarget(MotionControllerEventHandler h)
        {
            _OnReachedTarget.Remove(h);
        }
        private WeakCollection<MotionControllerEventHandler> _OnReachedTarget = new WeakCollection<MotionControllerEventHandler>();
        private bool ReachedTarget()
        {
            MotionControllerEventArgs motionControlerEventArgs = new MotionControllerEventArgs(_Rigidbody2D.transform.position);
            bool remove = true;
            List<MotionControllerEventHandler> removes = new List<MotionControllerEventHandler>();
            foreach(MotionControllerEventHandler motionControllerEventHandler in _OnReachedTarget)
            {
                    try
                    {
                        bool r = motionControllerEventHandler(motionControlerEventArgs);
                        remove = remove && r;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
            }
            return remove;
        }
        #endregion
        private ILooper _ILooper;
        public ILooper ILooper { set { _ILooper = value; } }
        private Rigidbody2D _Rigidbody2D;
        private Target _Target;
        private float _ToVelocity;
        private float _MaxVelocity;
        private float _MaxForce;
        private float _Gain;
        private Func<Vector2> _GetDist;
        private Func<bool> _IsAtTarget;
        public enum DimensionsTypes { XOnly, YOnly, All }
        private DimensionsTypes _Dimensions;
        public DimensionsTypes Dimensions {
            set
            {
                _Dimensions = value;
            }
        }
        private float _WithinThreshold = 0.01f;
        private float WithinThreshold
        {
            get
            {
                return _WithinThreshold;
            }
            set
            {
                _WithinThreshold = value;
            }
        }
        public DynamicMotionController(ILooper iLooper, Rigidbody2D rigidbody2D, float toVelocity, float maxVelocity, float maxForce, float gain, float? withinThreshold, DimensionsTypes dimensions = DimensionsTypes.All)
        {
            _ILooper = iLooper;
            _Rigidbody2D = rigidbody2D;
            _Dimensions = dimensions;
            _ToVelocity = toVelocity;
            _MaxVelocity = maxVelocity;
            _MaxForce = maxForce;
            _Gain = gain;
            if(withinThreshold!=null)
            {
                WithinThreshold = (float)withinThreshold;
            }
            if (dimensions.Equals(DimensionsTypes.XOnly))
            {
                Debug.Log(_Rigidbody2D.transform.position);
                _GetDist = new Func<Vector2>(() => { return new Vector2(_Target.Position.x - _Rigidbody2D.transform.position.x, 0); });
                _IsAtTarget = new Func<bool>(() => {
                    return new Vector2(_Target.Position.x - _Rigidbody2D.transform.position.x, 0).magnitude <= WithinThreshold;
                });
            }
            else
            {
                if (dimensions.Equals(DimensionsTypes.YOnly))
                {
                    _GetDist = new Func<Vector2>(() => { return new Vector2(0, _Target.Position.y - _Rigidbody2D.transform.position.y); });
                    _IsAtTarget = new Func<bool>(() => {
                        return new Vector2(0, _Target.Position.y - _Rigidbody2D.transform.position.y).magnitude <= WithinThreshold;
                    });
                }
                else {
                    _GetDist = new Func<Vector2>(() => { return new Vector2(_Target.Position.x - _Rigidbody2D.transform.position.x, _Target.Position.y - _Rigidbody2D.transform.position.y); });
                    _IsAtTarget = new Func<bool>(() => {
                        return new Vector2(_Target.Position.x - _Rigidbody2D.transform.position.x, _Target.Position.y - _Rigidbody2D.transform.position.y).magnitude <= WithinThreshold;
                    });
                }
            }
        }
        public void MoveTo(Vector2 position)
        {
            _ILooper.AddFixedUpdate(this);
            _Target = new Target(position);
        }
        public void MoveTo(Transform transform)
        {
            _Target = new Target(transform);
            _ILooper.AddFixedUpdate(this);
        }
        public void MoveTo(Target target)
        {
            _Target = target;
            _ILooper.AddFixedUpdate(this);
        }
        public void Cancel()
        {
            _Target = null;
        }
        private void Update()
        {
            Vector2 dist = _GetDist();
            // calc a target vel proportional to distance (clamped to maxVel)
            Vector2 tgtVel = Vector2.ClampMagnitude(_ToVelocity * dist, _MaxVelocity);
            // calculate the velocity error
            Vector2 error = tgtVel - (_Rigidbody2D.velocity);
            // calc a force proportional to the error (clamped to maxForce)
            Vector2 force = Vector2.ClampMagnitude(_Gain * error,   _MaxForce);
            _Rigidbody2D.AddForce(force);//.velocity+=new Vector2(dV, y);
            _Rigidbody2D.velocity += new Vector2(0, 0);
        }
        public bool LooperFixedUpdate()
        {
            if (_Target != null && _Rigidbody2D != null)
            {
                if (_Target.IsAlive)
                {
                    if (!_IsAtTarget())
                    {
                        Update();
                        return false;
                    }
                    else
                    {
                        bool stopTracking = ReachedTarget();
                        if (stopTracking)
                            _Target = null;
                        else
                            return false;
                    }
                }
                else
                    _Target = null;
            }
            return true;
        }
        public void Dispose()
        {
            if(_ILooper!=null)
                _ILooper.RemoveFixedUpdate(this);
        }
    }
}