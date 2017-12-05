using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraSwiper : ITouchableTwoFingers
    {
        private ITouchSensor _ITouchSensor;
        private const float MIN_FINGER_SCREEN_DISTANCE = 10;
        private ICameraController _ICameraController;
        private bool _Paused = false;
        public bool Enabled = true;
        private bool _SwipingTwoFingers = false;
        private float _ZoomRate = 2;
        private Vector2 _CentresDistanceAppart;
        public float ZoomRate
        {
            get
            {
                return _ZoomRate;
            }
            set
            {
                _ZoomRate = value;
            }
        }
        private bool _ZoomEnabled = false;
        public bool ZoomEnabled
        {
            get
            {
                return _ZoomEnabled;
            }
            set
            {
                _ZoomEnabled = value;
            }
        }
        private Vector2? _Start;
        private Vector2 _StartPosition;
        private Camera _Camera;
        private CameraSwiperDirections _Direction;
        private float _UnitPerPixel;
        private Rect? _Bounds = null;
        private float _StartProportion;
        private float _Divisor;
        private float _DivisorCentre;
        private float _StartSize;
        private Vector2 _StartProportionCentre;
        private Vector2 _StartZoomCentre;
        private class ShiftedBounds
        {
            public float Top;
            public float Bottom;
            public float Left;
            public float Right;
            public ShiftedBounds(Rect bounds, float halfCameraWidth, float halfCameraHeight)
            {
                Top = bounds.y - halfCameraHeight;
                Bottom = bounds.y + halfCameraHeight - bounds.height;
                Left = bounds.x + halfCameraWidth - bounds.width;
                Right = bounds.x - halfCameraWidth;
            }
        }
        private ShiftedBounds _ShiftedBounds;
        public CameraSwiperDirections Direction
        {
            set
            {
                switch (value)
                {
                    case CameraSwiperDirections.Horizontally:
                        _Swipe = new Action<SwipingInfo>((swipingInfo) =>
                        {
                            float x = (((Vector2)_Start).x - swipingInfo.PositionScreen.x) * _UnitPerPixel;
                            _ICameraController.SetX(x);
                        });
                        break;
                    case CameraSwiperDirections.Vertically:
                        _Swipe = new Action<SwipingInfo>((swipingInfo) =>
                        {
                            float y = (((Vector2)_Start).y - swipingInfo.PositionScreen.y) * _UnitPerPixel;
                            _ICameraController.SetY(y);
                        });
                        break;
                    default:
                        _Swipe = new Action<SwipingInfo>((swipingInfo) =>
                        {
                            float y = (((Vector2)_Start).y - swipingInfo.PositionScreen.y) * _UnitPerPixel;
                            float x = (((Vector2)_Start).x - swipingInfo.PositionScreen.x) * _UnitPerPixel;
                            _ICameraController.SetX(x);
                            _ICameraController.SetY(y);

                        });
                        break;
                }
                _Direction = value;
            }
            get
            {
                return _Direction;
            }
        }
        private Action<SwipingInfo> _Swipe;
        public CameraSwiper(ICameraController iCameraController)
        {
            _ICameraController = iCameraController;
        }
        public void SetBounds(Rect rect)
        {
            _Bounds = rect;
        }
        public void Pause()
        {
            _ITouchSensor.RemoveTouchable(this);
            _Paused = true;
        }
        public void Unpause()
        {
            _ITouchSensor.AddTouchable(TouchPriority.Camera, this);
            _Paused = false;
        }
        private void _SwipeTwoFingers(SwipingInfoTwoFingers swipingInfoTwoFingers)
        {
            float fingerDistanceScreen = swipingInfoTwoFingers.ScreenDistance.magnitude;
             fingerDistanceScreen= fingerDistanceScreen < MIN_FINGER_SCREEN_DISTANCE ? MIN_FINGER_SCREEN_DISTANCE  
                : fingerDistanceScreen;
            float size = _Divisor * _StartSize/ fingerDistanceScreen;
            float proportionCent = swipingInfoTwoFingers.ScreenDistance.magnitude / _DivisorCentre;
            _ICameraController.SetSize(size);
            _ICameraController.SetX(
           _StartZoomCentre.x);
            _ICameraController.SetY(
          _StartZoomCentre.y);
        }
        public void Swipe(SwipingInfo swipingInfo)
        {
            if (!_Handled)
            {
                if (_Start != null && Enabled)
                    _Swipe(swipingInfo);
            }
        }
        private bool _Handled = false;
        public void Touch(TouchInfo touchInfo)
        {
            if (!_Paused)
            {
                if (!touchInfo.Handled)
                {
                    touchInfo.SetHandled();
                    _Handled = false;
                    if (Enabled)
                    {
                        _UnitPerPixel = _ICameraController.UnitsPerPixel;
                        _StartPosition = _ICameraController.GetPosition();
                        _Start = new Vector2(touchInfo.ScreenPosition.x + (_StartPosition.x / _UnitPerPixel), touchInfo.ScreenPosition.y + (_StartPosition.y / _UnitPerPixel));
                    }
                }
                else
                    _Handled = true;
            }
        }

        public void TouchEnded(Vector2 positoin)
        {
            _Start = null;
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ITouchSensor).IsAssignableFrom(type))
            {
                _ITouchSensor = (ITouchSensor)o;
                _ITouchSensor.AddTouchable(TouchPriority.Camera, this);
            }
        }

        public void SwipeTwoFingers(SwipingInfoTwoFingers swipingInfo)
        {
            if (_SwipingTwoFingers && Enabled&&ZoomEnabled)
                _SwipeTwoFingers(swipingInfo);
        }

        public void TouchTwoFingers(TouchInfoTwoFingers touchInfo)
        {
                if (!_Paused)
                {
                    if (Enabled)
                    {
                    _StartSize = _ICameraController.GetSize();
                    _StartZoomCentre = touchInfo.FirstFinger.Position + (touchInfo.Distance / 2);
                    _Divisor = touchInfo.ScreenDistance.magnitude;
                    _Divisor = _Divisor < MIN_FINGER_SCREEN_DISTANCE ? MIN_FINGER_SCREEN_DISTANCE : _Divisor;
                    ICameraConfiguration ic = _ICameraController.GetConfiguration();
                    _DivisorCentre = _Divisor;// * _ICameraController.GetSize()/ (ic.SizeMax - ic.Size);
                    _CentresDistanceAppart= (Vector2)_ICameraController.GetPosition() - (Vector2)_StartZoomCentre;
                    _SwipingTwoFingers = true;
                    }
                }
        }

        public void TouchEndedTwoFingers(Vector2 position, Vector2 position2)
        {
            _SwipingTwoFingers = false;
        }
    }
}