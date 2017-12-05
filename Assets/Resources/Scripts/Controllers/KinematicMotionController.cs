using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class KinematicMotionController:IMotionController
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

        public void AddEventHandlerOrbitAngleChanged(MotionControllerEventHandler h)
        {
            _OrbitAngleChanged.Add(h);
        }
        public void RemoveEventHandlerAngleChanged(MotionControllerEventHandler h)
        {
            _OrbitAngleChanged.Remove(h);
        }
        private WeakCollection<MotionControllerEventHandler> _OrbitAngleChanged = new WeakCollection<MotionControllerEventHandler>();

        private bool ReachedTarget()
        {
            if (_Transform == null)
                return true;
            MotionControllerEventArgs motionControlerEventArgs = new MotionControllerEventArgs(_Transform.position);
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
        private bool OrbitAngleChanged(Angle angle)
        {
            MotionControllerEventArgs motionControlerEventArgs = new MotionControllerEventArgs(angle);
            bool remove = true;
            List<MotionControllerEventHandler> removes = new List<MotionControllerEventHandler>();
            foreach (MotionControllerEventHandler motionControllerEventHandler in _OrbitAngleChanged)
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
        public ILooper ILooper {
            set
            {
                _ILooper = value;
                if(_KinematicMotionControllerOrbit!=null)
                _KinematicMotionControllerOrbit.ILooper = value;
            }
        }
        private Transform _Transform;
        private KinematicMotionControllerOrbit _KinematicMotionControllerOrbit;
        private KinematicMotionControllerOrbit KinematicMotionContrtollerOrbit
        {
            get
            {
                if (_KinematicMotionControllerOrbit == null)
                    _KinematicMotionControllerOrbit = new KinematicMotionControllerOrbit(_ILooper, _Transform);
                return _KinematicMotionControllerOrbit;
            }
        }
        private float _Acceleration;
        private float _FullSpeed;
        private Vector2 _Velocity;
        private Target _Target;
        private float _CurrentSpeed;
        private Action Update;
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
        private float _EasingFactor = 2;
        public float EasingFactor
        {
            get
            {
                return _EasingFactor;
            }
            set
            {
                _EasingFactor = value;
            }
        }
        private float _DecelerationDistance;
        private float DecelerationDistance
        {
            get
            {
                _DecelerationDistance = GetAccelerationDistance(_Velocity.magnitude, 0);
                return _DecelerationDistance *EasingFactor;
            }
        }
        private float GetAccelerationDistance(float fromVelocity, float toVelocity)
        {
            return (Mathf.Pow(fromVelocity, 2) - Mathf.Pow(toVelocity, 2)) / (2 * _Acceleration);
        }
        private float GetMaxVelocityForDeceleration(float distance)
        {
            return Mathf.Pow(2 * (_Acceleration/EasingFactor) * distance, 0.5f);
        }
        public KinematicMotionController(ILooper iLooper, Transform transform, float fullSpeed, float? withinThreshold, float? acceleration = null, DimensionsTypes dimensions = DimensionsTypes.All)
        {
            _ILooper = iLooper;
            _FullSpeed = fullSpeed;
            _Transform = transform;
            _Dimensions = dimensions;
            if(withinThreshold!=null)
            {
                WithinThreshold = (float)withinThreshold;
            }
            if (acceleration != null)
            {
                _Acceleration = (float)acceleration;
                Update = new Action(() =>
                {
                    throw new NotImplementedException();
                            Vector2 distance = _Target.Position - (Vector2)_Transform.position;
                            Vector2 requiredDirection = distance.normalized;
                            float desiredVelocityMagnitude;
                            if (distance.magnitude <= DecelerationDistance)
                            {
                                float maxVelocityMagnitudeToDecelerate = GetMaxVelocityForDeceleration(distance.magnitude);
                                desiredVelocityMagnitude = maxVelocityMagnitudeToDecelerate < _FullSpeed ? maxVelocityMagnitudeToDecelerate : _FullSpeed;
                            }
                            else
                                desiredVelocityMagnitude = _FullSpeed;
                            float maxDVMagnitude = desiredVelocityMagnitude - _Velocity.magnitude;
                            float accelerationChangeMaxMagnitude = _Acceleration * Time.fixedDeltaTime;
                            float dvMagnitude = maxDVMagnitude < accelerationChangeMaxMagnitude ? (maxDVMagnitude < -accelerationChangeMaxMagnitude ? -accelerationChangeMaxMagnitude : maxDVMagnitude) : accelerationChangeMaxMagnitude;
                            Vector2 dV = new Vector2(requiredDirection.x * dvMagnitude, requiredDirection.y * dvMagnitude);
                            _Velocity = _Velocity + dV;
                            _Transform.position = _Transform.position + new Vector3(  
                                _Dimensions!= DimensionsTypes.YOnly?Time.fixedDeltaTime * _Velocity.x:
                                0, 
                                _Dimensions!= DimensionsTypes.XOnly?Time.fixedDeltaTime * _Velocity.y:
                                0
                                , 0f);

                });
            }
            else
                Update = new Action(() =>
                {
                            Vector2 requiredDirection = _Target.Position - (Vector2)_Transform.position;
                    requiredDirection = new Vector2(_Dimensions != DimensionsTypes.YOnly ? requiredDirection.x : 0, _Dimensions != DimensionsTypes.XOnly ? requiredDirection.y : 0);
                            Vector2 requiredDirectionNormalized = requiredDirection.normalized;
                    float dX = requiredDirectionNormalized.x * _FullSpeed * Time.deltaTime;
                    float dY = requiredDirectionNormalized.y * _FullSpeed * Time.deltaTime;
                    bool dontUpdate = true;
                    float dXMagnitude = Mathf.Sign(dX) * dX;
                    float dYMagnitude = Mathf.Sign(dY) * dY;
                    float requiredDirectionXMagnitude = Mathf.Sign(requiredDirection.x) * requiredDirection.x;
                    float requiredDirectionYMagnitude = Mathf.Sign(requiredDirection.y) * requiredDirection.y;
                    if (dXMagnitude > requiredDirectionXMagnitude)
                    {
                        if (requiredDirectionXMagnitude > 0)
                            dontUpdate = false;
                        dX = requiredDirection.x;
                    } else dontUpdate = false;
                    if (dYMagnitude > requiredDirectionYMagnitude)
                    {
                        if (requiredDirectionYMagnitude > 0)
                            dontUpdate = false;
                        dY = requiredDirection.y;
                    }
                    else dontUpdate = false;
                    if (!dontUpdate)
                    {
                        Vector3 dS = new Vector3(dX, dY, 0f);
                        _Transform.position = _Transform.position + dS;
                    }
                });
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
        public void Orbit(Vector2 point, float toAngle, bool clockwise, float endRadius)
        {
            KinematicMotionContrtollerOrbit.Orbit(point, 180*_FullSpeed/(endRadius  * Mathf.PI), toAngle, clockwise, endRadius, (Angle angle) =>
            {
                OrbitAngleChanged(angle);
            },
            ()=> {
                ReachedTarget();
            });
        }
        public void Cancel()
        {
            _Target = null;
        }
        private int _NRepeatedValues = 0;
        private Vector2 _LastPosition;
        private bool IsAtTarget()
        {
            Vector2 position = _Target.Position;
            if (_Transform.position.x == _LastPosition.x && _Transform.position.y == _LastPosition.y)
            {
                _NRepeatedValues++;
                if (_NRepeatedValues > 5){
                    return true;
                }
            }
            else
            {
                if(_NRepeatedValues>0)
                    _NRepeatedValues --;
            }
            _LastPosition = _Transform.position;
            float magnitude = new Vector2(
                _Dimensions != DimensionsTypes.YOnly ?
                position.x - _Transform.position.x
                : 0,
                _Dimensions != DimensionsTypes.XOnly ?
                position.y - _Transform.position.y
                : 0).magnitude;
            return  magnitude <= WithinThreshold;
        }
        public bool LooperFixedUpdate()
        {
            if (_Target != null&& _Transform!= null)
            {
                if (_Target.IsAlive)
                {
                    if (!IsAtTarget())
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