using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading;
namespace Assets.Scripts
{
    #region Events
    public delegate void RotationHandler(RotatedEventArgs e);
    public class RotatedEventArgs : EventArgs
    {
        private bool _IsLandscape;
        public bool IsLandscape
        {
            get
            {
                return _IsLandscape;
            }
        }
        public DeviceOrientation DeviceOrientation;
        public RotatedEventArgs(DeviceOrientation deviceOrientation, bool isLandscape)
        {
            _IsLandscape = isLandscape;
            DeviceOrientation = deviceOrientation;
        }
    }
    #endregion
    public class RotationSensor : UIBehaviour, IRotationSensor, ILooperFixedUpdate
    {
        #region Events
        public void AddEventHandler(RotationHandler h)
        {
            _EventHandlerList.Add(h);
        }
        public void RemoveEventHandler(RotationHandler h)
        {
            _EventHandlerList.Remove(h);
        }
        private void FireEvent(RotatedEventArgs e)
        {
            foreach (RotationHandler rotationHandler in _EventHandlerList)
            {
                try
                {
                    rotationHandler(e);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }
        private WeakCollection<RotationHandler> _EventHandlerList = new WeakCollection<RotationHandler>();
        #endregion

        private ILooper _ILooper;
        private bool _FireEventOnNextUpdate = false;
        static DeviceOrientation orientation;
        protected override void OnRectTransformDimensionsChange()
        {
            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.Unknown:
                    FireEventOnNextUpdate();
                    orientation = Input.deviceOrientation;
                    break;
                case DeviceOrientation.FaceUp:
                case DeviceOrientation.FaceDown:
                    break;
                default:
                    if (orientation != Input.deviceOrientation)
                    {
                        orientation = Input.deviceOrientation;
                        FireEventOnNextUpdate();
                    }
                    break;
            }
        }
        private void FireEventOnNextUpdate()
        {
            if (_ILooper != null)
            {
                _ILooper.AddFixedUpdate(this);
                _FireEventOnNextUpdate = true;
            }
        }
        private bool GetIsLandscape()
        {
            return Camera.main.aspect >= 1;
        }
        private int _LooperCycleSinceFireEventOnNextUpdate = 0;
        public bool LooperFixedUpdate()
        {
            if (_LooperCycleSinceFireEventOnNextUpdate < 1)
            {
                _LooperCycleSinceFireEventOnNextUpdate++;
                return false;
            }
            else
            {
                if (_FireEventOnNextUpdate)
                {
                    _FireEventOnNextUpdate = false;
                    FireEvent(new RotatedEventArgs(orientation, GetIsLandscape()));
                }
                _LooperCycleSinceFireEventOnNextUpdate = 0;
                return true;
            }
        }
        protected override void Start()
        {
            base.Start();
            orientation = Input.deviceOrientation;
            FireEventOnNextUpdate();
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
        }
    }
}