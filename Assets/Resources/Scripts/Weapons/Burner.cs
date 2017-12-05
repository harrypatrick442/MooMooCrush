using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Burner : MonoBehaviour, ITouchable, ILooperFixedUpdate
    {
        private IBurnHandler _IBurnHandler;
        private List<IBurnable> _DoneTargets = new List<IBurnable>();
        private ParticleSystem _ParticleSystem;
        private float? flameStartTIme = null;
        private ParticleSystem ParticleSystem
        {
            get
            {
                if (_ParticleSystem == null) _ParticleSystem = GetComponentInChildren<ParticleSystem>();
                return _ParticleSystem;
            }
        }
        private Vector2?_FingerPosition;
        public float activeAngle= 110f;
        public float droppingAngularVelocity = 0.04f;
        public float trackingAngularVelocity = 0.28f;
        private enum Modes { Resting, Activating, Deactivating, Active}
        private bool touching = false;
        private Modes Mode;
        private ITouchSensor _ITouchSensor;
        private ILooper _ILooper;
        private Transform TransformNozel
        {
            get
            {
                return gameObject.transform.GetChild(0).GetChild(0);
            }
        }
        private void Start()
        {

        }
        public List<IBurnable> GetNewTargets(Vector2 position, float distance)
        {
            List<IBurnable> list = new List<IBurnable>();
            RaycastHit2D[] hits = Physics2D.RaycastAll(((Vector2)TransformNozel.transform.position), position- ((Vector2)TransformNozel.transform.position), distance);
            foreach(RaycastHit2D hit in hits)
            if (hit)
            {
                    IBurnable iBurnable = Lookup.GetBody<IBurnable>(hit.collider.gameObject);
                    if (iBurnable != null)
                    {
                        if (!_DoneTargets.Contains(iBurnable))
                        {
                            _DoneTargets.Add(iBurnable);
                            list.Add(iBurnable);
                        }
                    }
                }
            return list;
        }
        public void Activate()
        {
            Mode = Modes.Activating;
            _ILooper.AddFixedUpdate(this);
            _ITouchSensor.AddTouchable(TouchPriority.Weapon, this);
        }
        private bool _Handled = false;
        public void Deactivate()
        {
            Mode = Modes.Deactivating;
            _ITouchSensor.RemoveTouchable(this);
            _ILooper.AddFixedUpdate(this);
        }
        public void Swipe(SwipingInfo swipingInfo)
        {
            if (!_Handled)
            {
                if (swipingInfo.Type.Equals(SwipingInfo.Types.Traveled))
                {
                    Mode = Modes.Active;
                    _FingerPosition = swipingInfo.Position;
                }
            }
        }
        public bool LooperFixedUpdate()
        {
            bool remove = true;
            if (!Mode.Equals(Modes.Resting))
            {
                if (Mode.Equals(Modes.Activating))
                {
                    _Activating();
                    remove = false;
                }
                else
                {
                    if (Mode.Equals(Modes.Active))
                    {
                        remove = _Active();
                    }
                    else
                    {
                        if (Mode.Equals(Modes.Deactivating))
                        {
                            remove = _Deactivating();
                        }
                    }
                }
            }
            return remove;
        }
        private void _Activating()
        {
            StartBurner();
            float z = gameObject.transform.eulerAngles.z + (droppingAngularVelocity / Time.fixedDeltaTime);
            if (z > activeAngle)
            {
                z = activeAngle;
                Mode = Modes.Active;
            }
            gameObject.transform.localEulerAngles = new Vector3(0f, 0f, z);
        }
        private bool _Active()
        {
            if (_FingerPosition != null)
            {
                UpdatePosition((Vector2)_FingerPosition);
            }
            return !touching;
        }
        private float SetJetLength(Vector2 position)
        {
            Vector2 difference = ((Vector2)TransformNozel.position) - position;
            float magnitude = (difference).magnitude;
            float s = magnitude * 0.15f;
        ParticleSystem.startLifetime = s< 0.05f ? 0.05f : s;
            if (flameStartTIme == null)
            {
                flameStartTIme = Time.time;
                return 0;
            }
            else
            {
                float distance = ParticleSystem.startSpeed * (Time.time - (float)flameStartTIme);
                if (distance > magnitude) distance = magnitude;
                return distance;
            }
        }
        private bool _Deactivating()
        {
            StopBurner();
            float z = gameObject.transform.eulerAngles.z - (droppingAngularVelocity / Time.fixedDeltaTime);
            if (z < 0)
            {
                z = 0;
                Mode = Modes.Resting;
                transform.localEulerAngles = new Vector3(0, 0, z);
                return true;
            }
            transform.localEulerAngles = new Vector3(0, 0, z);
            return false;
        }
        private void UpdatePosition(Vector2 position)
        {
            float a = (Mathf.Atan2((gameObject.transform.position.y - ((Vector2)position).y), (gameObject.transform.position.x - ((Vector2)position).x)) * 180 / Mathf.PI);
            gameObject.transform.localEulerAngles = new Vector3(0, 0, a);
            float distance = SetJetLength(position);
            List<IBurnable> newBurnables = GetNewTargets(position, distance);
            foreach (IBurnable newBurnable in newBurnables)
                _IBurnHandler.Burn(newBurnable);
        }
        private void StartBurner()
        {
            PilotLight();
            ParticleSystem.Play();
        }
        private void StopBurner()
        {
            ParticleSystem.Stop();
        }
        private void PilotLight()
        {
            ParticleSystem.startLifetime = 0.05f;
        }
        public void Touch(TouchInfo touchInfo)
        {
            if (!touchInfo.Handled)
            {
                _Handled = false;
                touchInfo.SetHandled();
                Mode = Modes.Active;
                touching = true;
                _ILooper.AddFixedUpdate(this);
                _ITouchSensor.AddTouchable(TouchPriority.Weapon, this);
                _FingerPosition = touchInfo.Position;
            }
            else
            {
                _Handled = true;
            }
        }
        public void TouchEnded(Vector2 position)
        {
            touching = false;
            _FingerPosition = null;
            flameStartTIme = null;
            PilotLight();
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
            if (typeof(ITouchSensor).IsAssignableFrom(type))
                _ITouchSensor = (ITouchSensor)o;
            if (typeof(IBurnHandler).IsAssignableFrom(type))
                _IBurnHandler = (IBurnHandler)o;
        }
    }
}
