using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
    public class SuperMooFlightController:IDisposable, IGetCloseTo
    {
        private enum FlyingStates {Main, Uprighting, Touchdown}
        private FlyingStates _FlyingState;
        private float _MinDistanceFromTarget = 1f;
        private float _MaxDistanceFromTarget = 2f;
        public float MinDistanceFromTarget
        {
            get
            {
                return _MinDistanceFromTarget;
            }
            set
            {
                _MinDistanceFromTarget = value;
            }
        }
        public float MaxDistanceFromTarget
        {
            get
            {
                return _MaxDistanceFromTarget >= _MinDistanceFromTarget ? _MaxDistanceFromTarget : _MinDistanceFromTarget;
            }
            set
            {
                _MaxDistanceFromTarget = value;
            }
        }
        private const float VERTICAL_TOUCHDOWN = 0f;
        private const float SPEED = 4f;
        private const float RADIUS_UPRIGHT = 0.7f;
        private Vector2 _Position;
        private Angle _Angle;
        private float[] _SteppedDistances;
        private Vector2 _Center;
        private bool _GoingRight;
        private bool _WithinSemicircleX;
        private bool _WithinSemicircleY;
        private bool _Below;
        private MotionControllerEventHandler _MotionControllerEventHandler;
        private MotionControllerEventHandler _MotionControllerEventHandlerOrbitAngleChanged;
        private List<float> SteppedDistances
        {
            get
            {
                if (_SteppedDistances == null)
                {
                    _SteppedDistances = new float[11];
                    int i = 0;
                    while (i <= 10)
                    {
                        float distance = MinDistanceFromTarget + ((i * (MaxDistanceFromTarget - MinDistanceFromTarget)) / 10);
                        _SteppedDistances[i] = distance;
                        i++;
                    }
                }
                return new List<float>((float[])_SteppedDistances.Clone());
            }
        }
        private Transform _Transform;
        private Rect[] _AllowedRegions;
        private IGetRect _IGetRect;
        private IBodyType _IBodyType;
        private Action<bool> _IsRepositioning;
        private KinematicMotionController _KinematicMotionController;
        public SuperMooFlightController(Action<bool> reachedTarget, ILooper iLooper, Transform transform, Rect[] allowedRegions, IGetRect iGetRect, IBodyType iBodyType)
        {
            _Transform = transform;
            _IGetRect = iGetRect;
            _AllowedRegions = allowedRegions;
            _IBodyType = iBodyType;
            _IsRepositioning = reachedTarget;
            _MotionControllerEventHandler = new MotionControllerEventHandler(MotionControllerEventReachedTarget);
            _MotionControllerEventHandlerOrbitAngleChanged = new MotionControllerEventHandler(MotionControllerEventOrbitAngleChanged);
            _KinematicMotionController = new KinematicMotionController(iLooper, transform, SPEED, 0.05f);
            _KinematicMotionController.AddEventHandlerReachedTarget(_MotionControllerEventHandler);
            _KinematicMotionController.AddEventHandlerOrbitAngleChanged(_MotionControllerEventHandlerOrbitAngleChanged);
        }
        private bool MotionControllerEventOrbitAngleChanged(MotionControllerEventArgs e)
        {
            if(_Transform!=null)
            _Transform.eulerAngles = new Vector3(0, 0, _GoingRight? 90 - e.Angle.Degrees:270 - e.Angle.Degrees);
            return false;
        }
        private bool MotionControllerEventReachedTarget(MotionControllerEventArgs e)
        {
            if (_FlyingState.Equals(FlyingStates.Main))
            {
                _FlyingState = FlyingStates.Uprighting;
                _KinematicMotionController.Orbit(_Center, _GoingRight ? 90:-90, !_GoingRight, RADIUS_UPRIGHT);
                return true;
            }
            else
            {
                if (_FlyingState.Equals(FlyingStates.Uprighting))
                {
                    _FlyingState = FlyingStates.Touchdown;
                    _KinematicMotionController.MoveTo(_Position);
                }
                else
                {
                    _IBodyType.ReleaseKinematic(this);
                    _Transform.localEulerAngles = new Vector3(0, 0, 0);
                    _IsRepositioning(false);
                    return true;
                }
            }
            return false;
        }
        private float Positive(float value)
        {
            return value * Mathf.Sign(value);
        }
        public void FlyTo(Vector2 position)
        {
            _IsRepositioning(true);
            _Position = position;
            _FlyingState = FlyingStates.Main;
            _GoingRight = position.x > _Transform.position.x;
            float a = (position - (Vector2)_Transform.position).x;
            _WithinSemicircleX = a*Mathf.Sign(a) < RADIUS_UPRIGHT;

            if (_Transform != null)
            {
                _Below = _Transform.position.y < position.y + VERTICAL_TOUCHDOWN - RADIUS_UPRIGHT;
                _Center = new Vector2(position.x + (_GoingRight ? -RADIUS_UPRIGHT : RADIUS_UPRIGHT), position.y + VERTICAL_TOUCHDOWN);
                Vector2 aD = new Vector2(_Center.x - _Transform.position.x, _Center.y - _Transform.position.y);
                float sin = RADIUS_UPRIGHT / aD.magnitude;
                sin = sin > 1 ? 1 : (sin < -1 ? -1 : sin);
                float asin = aD.magnitude != 0 ? Positive(Mathf.Asin(sin)) : 0;
                float atan = aD.x != 0 ? Positive(Mathf.Atan(Positive(aD.y / aD.x))) : 0;
                Vector2 first;
                if (_Below)
                {
                    _Angle = new Angle(atan - asin);
                    first = new Vector2(_Center.x - ((_GoingRight ? -1 : 1) * RADIUS_UPRIGHT * Mathf.Sin(_Angle.Radians)), _Center.y - (RADIUS_UPRIGHT * Mathf.Cos(_Angle.Radians)));
                    _Transform.eulerAngles = new Vector3(0, 0, _GoingRight ? _Angle.Degrees - 90 : 90 - _Angle.Degrees);
                }
                else
                {
                    if (_WithinSemicircleX)
                        atan = Mathf.PI - atan;
                    _Angle = new Angle(atan + asin);
                    first = new Vector2(_Center.x + ((_GoingRight ? -1 : 1) * RADIUS_UPRIGHT * Mathf.Sin(_Angle.Radians)), _Center.y - (RADIUS_UPRIGHT * Mathf.Cos(_Angle.Radians)));
                    _Transform.eulerAngles = new Vector3(0, 0, _GoingRight ? 270 - _Angle.Degrees : 90 + _Angle.Degrees);
                }
                _IBodyType.TakeKinematic(this);
                _KinematicMotionController.MoveTo(first);
            }
        }
        public void GetCloseTo(Vector2 position)
        {
            Vector2? to = MyGeometry.GetCloseTo(position, _AllowedRegions, MinDistanceFromTarget, MaxDistanceFromTarget, _IGetRect, SteppedDistances);
            if (to != null)
                FlyTo((Vector2)to);
        }
        public void Dispose()
        {
            _KinematicMotionController.Dispose();
        }
    }
}