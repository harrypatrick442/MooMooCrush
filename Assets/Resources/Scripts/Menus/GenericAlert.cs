using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Assets.Scripts
{
    public class GenericAlert : MonoBehaviour, ITouchable, IShow
    {
        private IPause _IPause;
        private ITouchSensor _ITouchSensor;
        private IMenuHidden _IMenuHidden;
        private ControlButton _ControlButton;
        private ControlButton ControlButton
        {
            get
            {
                if (_ControlButton == null) _ControlButton = GetComponentInChildren<ControlButton>();
                return _ControlButton;
            }
        }
        private RectTransform _RectTransform;
        private RectTransform RectTransform
        {
            get { if (_RectTransform == null) _RectTransform = transform.Find("CustomMenu").transform.GetComponent<RectTransform>();
                return _RectTransform;
            }
        }
        private Rect? _Bounds;
        private Rect Bounds
        {
            get
            {
                if (_Bounds == null)
                {
                    _Bounds = MyGeometry.GetWorldRect(RectTransform);
                }
                return (Rect)_Bounds;
            }
        }
        private void Start()
        {
            ControlButton.SetDelegates(null, () => { Hide(); });
        }
        public virtual void Show()
        {
            gameObject.SetActive(true);
            _IPause.Pause();
            _ITouchSensor.AddTouchable(TouchPriority.UI0, this);
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
            _IPause.Unpause();
            _ITouchSensor.RemoveTouchable(this);
        }
        public void Touch(TouchInfo touchInfo)
        {
            if(!Bounds.Contains(touchInfo.Position))
            {
                Hide();
            }
        }
        public void TouchEnded(Vector2 positoin)
        {

        }
        public virtual void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(IPause).IsAssignableFrom(type))
                _IPause = (IPause)o;
            if (typeof(ITouchSensor).IsAssignableFrom(type))
                _ITouchSensor = (ITouchSensor)o;
        }

        public void Swipe(SwipingInfo swipingInfo)
        {   
        }
    }
}