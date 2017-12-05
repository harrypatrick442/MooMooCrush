using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class RitualKnife : MonoBehaviour, ITouchable, ILooperFixedUpdate
    {
        private KnifeInfo _CurrentlyBeingKnifed;
        private const float ROTATE_TO_ANGULAR_VELOCITY = 320f;
        private const float ROTATE_FROM_ANGULAR_VELOCITY = -320f;
        private const float ROTATE_TO_ANGLE = 20;
        private List<KnifeInfo> _Finishing = new List<KnifeInfo>();
        private float startAngle;
        private IGhosts _IGhosts;
        private ITouchSensor _ITouchSensor;
        private class KnifeInfo
        {
            public float StartTime;
            public enum Modes { Start, RotatingTo, RotatingFrom, Done }
            public Modes Mode = Modes.Start;
             public  IKnifable IKnifable;
            public float startTime;
            private IGhosts _IGhosts;
            public KnifeInfo(IKnifable iKnifable, IGhosts iGhosts)
            {
                IKnifable = iKnifable;
                _IGhosts = iGhosts;
            }
            public void RitualKnife()
            {
                IKnifable.OnSlaughterStart();
                IKnifable.RitualKnife();
                startTime = Time.time;
            }
            public void DoneRitualKnife()
            {
                IKnifable.DoneRitualKnife();
                if (_IGhosts != null) _IGhosts.Add(IKnifable);
            }
            public bool IsRitualKnifed()
            {
                return IKnifable.IsRitualKnifed();
            }
        }
        private Vector2?_FingerPosition;
        private enum Modes { Resting, Activating, Deactivating, Active}
        private bool touching = false;
        private Modes Mode;
        private ILooper _ILooper;
        private Transform _LeftMarkerTransform;
        private Transform LeftMarkerTransform
        {
            get
            {
                if (_LeftMarkerTransform == null) _LeftMarkerTransform = transform.Find("left_marker").transform;
                return _LeftMarkerTransform;
            }
        }
        private Transform _RightMarkerTransform;
        private Transform RightMarkerTransform
        {
            get
            {
                if (_RightMarkerTransform == null) _RightMarkerTransform = transform.Find("right_marker").transform;
                return _RightMarkerTransform;
            }
        }
        private Transform _MiddleMarkerTransform;
        private Transform MiddleMarkerTransform
        {
            get
            {
                if (_MiddleMarkerTransform == null) _MiddleMarkerTransform = transform.Find("right_marker").transform;
                return _MiddleMarkerTransform;
            }
        }
        public void Activate()
        {
            gameObject.SetActive(true);
            Mode = Modes.Activating;
            _ITouchSensor.AddTouchable(TouchPriority.Weapon, this);
        }
        public void Deactivate()
        {
            gameObject.SetActive(false);
            Mode = Modes.Deactivating;
            _ITouchSensor.RemoveTouchable(this);

        }
        public void Swipe(SwipingInfo swipingInfo)
        {
            if (!_Handled)
                if (swipingInfo.Type.Equals(SwipingInfo.Types.Traveled))
                {
                    Mode = Modes.Active;
                    _FingerPosition = swipingInfo.Position;
                    if (_CurrentlyBeingKnifed == null)
                    {
                        UpdatePosition();
                        Vector2 difference = (Vector2)(LeftMarkerTransform.position - RightMarkerTransform.position);
                        RaycastHit2D hit = Physics2D.Raycast((Vector2)RightMarkerTransform.position, (difference).normalized, difference.magnitude);
                        if (hit)
                        {
                            IKnifable iKnifable = Lookup.GetBody<IKnifable>(hit.collider.gameObject);
                            if (iKnifable != null)
                            {
                                if (iKnifable.IsInRitualKnifeBounds(transform.position))
                                {
                                    if (!iKnifable.IsBeingSlaughtered())
                                    {
                                        Vector2 position = iKnifable.GetRitualKnifeSnapInPlacePosition() + (Vector2)(MiddleMarkerTransform.position - transform.position);
                                        transform.position = new Vector3(position.x, position.y, transform.position.z);
                                        _ILooper.AddFixedUpdate(this);
                                        _CurrentlyBeingKnifed = new KnifeInfo(iKnifable, _IGhosts);
                                    }
                                }
                            }
                        }
                    }
            }
        }
        public bool LooperFixedUpdate()
        {
            bool r = true;
            if (!Mode.Equals(Modes.Resting))
            {
                if (Mode.Equals(Modes.Activating))
                {
                    r = false;
                    _Activating();
                }
                else
                {
                    if (Mode.Equals(Modes.Active))
                    {
                        r= _Active();
                    }
                    else
                    {
                        if (Mode.Equals(Modes.Deactivating))
                        {
                            r= _Deactivating();
                        }
                    }
                }
                if (_Finishing.Count > 0)
                {
                    r = false;
                    List<KnifeInfo> removes = new List<KnifeInfo>();
                    foreach (KnifeInfo kI in _Finishing)
                    {
                        if (kI.IsRitualKnifed() || kI.startTime + 2 < Time.time)
                        {
                            kI.DoneRitualKnife();
                            removes.Add(kI);

                        }
                    }
                    foreach (KnifeInfo knifeInfo in removes)
                        _Finishing.Remove(knifeInfo);
                }
            }
            return r;
        }
        private void _Activating()
        {
            Mode = Modes.Active;
        }
        private void RotateTo()
        {
            float a = transform.localEulerAngles.z + (ROTATE_TO_ANGULAR_VELOCITY * Time.fixedDeltaTime);
            if (a > ROTATE_TO_ANGLE)
            {
                _CurrentlyBeingKnifed.RitualKnife();
                a = ROTATE_TO_ANGLE;
                _CurrentlyBeingKnifed.Mode = KnifeInfo.Modes.RotatingFrom;
            }
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, a);
        }
        private void RotateFrom()
        {
            float a = transform.localEulerAngles.z + (ROTATE_FROM_ANGULAR_VELOCITY * Time.fixedDeltaTime);
            if (a < startAngle)
            {
                a = startAngle;
                _CurrentlyBeingKnifed.Mode = KnifeInfo.Modes.Done;
            }
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, a);

        }
        private bool _Active()
        {
            if(_CurrentlyBeingKnifed!=null)
            {
                switch(_CurrentlyBeingKnifed.Mode)
                {
                    case KnifeInfo.Modes.Start:
                        _CurrentlyBeingKnifed.Mode = KnifeInfo.Modes.RotatingTo;
                        break;
                    case KnifeInfo.Modes.RotatingTo:
                        RotateTo();
                        break;
                    case KnifeInfo.Modes.RotatingFrom:
                        RotateFrom();
                        break;
                    default:
                        _Finishing.Add(_CurrentlyBeingKnifed);
                        _CurrentlyBeingKnifed = null;
                        break;
                }
            }
            return false;
        }
        private bool _Deactivating()
        {
            gameObject.SetActive(false);
            if(_Finishing.Count<1)
                Mode = Modes.Resting;
            return false;
        }
        private void UpdatePosition()
        {
            transform.position = new Vector3(((Vector2)_FingerPosition).x-0.2f, ((Vector2)_FingerPosition).y+0.04f, transform.position.z);
        }
        private bool _Handled = false;
        public void Touch(TouchInfo touchInfo)
        {
            if (!touchInfo.Handled)
            {
                touchInfo.SetHandled();
                _Handled = false;
                Mode = Modes.Active;
                touching = true;
                _FingerPosition = touchInfo.Position;
                UpdatePosition();
            }
            else
                _Handled = true;
        }
        public void TouchEnded(Vector2 position)
        {
            touching = false;
            _FingerPosition = null;
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
            if (typeof(IGhosts).IsAssignableFrom(type))
                _IGhosts = (IGhosts)o;
            if (typeof(ITouchSensor).IsAssignableFrom(type))
                _ITouchSensor = (ITouchSensor)o;
        }
        void Start()
        {
            startAngle = transform.rotation.eulerAngles.z;
        }
    }
}
