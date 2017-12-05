using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour, ILooperFixedUpdate, ICameraController
    {
        private RotationHandler _RotationHandler;
        public float Speed = 1;
        public float SizeChangeSpeed = 0.001f;
        private ILooper _ILooper;
        private ICameraConfiguration _ICameraConfiguration;
        private Vector2 _Direction;
        private float _DSize;
        private CameraSwiper _CameraSwiper;
        private ShiftedCameraBounds _ShiftedCameraBounds;
        private Vector2 _BoundsCrossingPoint;
        private Func<float, bool> XIsDone;
        private Func<float, bool> YIsDone;
        private Func<float, bool> SizeIsDone;
        private bool _ReachedTarget = true;
        public float UnitsPerPixel
        {
            get
            {
                return MyGeometry.UnitsPerPixel(Camera);
            }
        }
        private float _ToSize;
        private Camera _Camera;
        private Camera Camera
        {
            get
            {
                if (_Camera == null) _Camera = GetComponent<Camera>();
                return _Camera;
            }
        }
        private float CameraAspectRatio
        {
            get
            {
                return Camera.aspect;
            }
        }
        private float MaxSize
        {
            get
            {
                return GetSizeToZoomRatio() / _ICameraConfiguration.ZoomMin;
            }
        }
        private float MinSize
        {
            get
            {
                return GetSizeToZoomRatio() / _ICameraConfiguration.ZoomMax;
            }
        }
        private void Awake()
        {
            _CameraSwiper = new CameraSwiper(this);

        }
        ShiftedCameraBounds CalculateShiftedBounds(float size)
        {
            if (_ICameraConfiguration.Bounds != null)
            {
                float halfCameraHeight = size;
                float halfCameraWidth = halfCameraHeight * Camera.main.aspect;
                return new ShiftedCameraBounds((Rect)_ICameraConfiguration.Bounds, halfCameraWidth, halfCameraHeight);
            }
            else
                return null;

        }
        public bool LooperFixedUpdate()
        {
            if (_ICameraConfiguration != null && !_ReachedTarget)
            {
                float newSize = Camera.orthographicSize;
                bool doneSize = (_DSize > 0 ? newSize >= _ToSize : newSize <= _ToSize) || MyMaths.Approximately(newSize, _ToSize, 0.01f);
                if(!doneSize)
                {
                    newSize +=(_DSize * Time.fixedDeltaTime);
                    Camera.orthographicSize = newSize;
                }
                else
                    Camera.orthographicSize = _ToSize;

                float newX = gameObject.transform.position.x;
                bool doneX = XIsDone(newX);
                if(!doneX)
                {
                    newX += _Direction.x * Speed * Time.fixedDeltaTime;
                }
                else
                {
                     ConstrainX(ref newX);
                }
                float newY = gameObject.transform.position.y;
                bool doneY = YIsDone(newY);
                if (!doneY)
                {
                    newY += _Direction.y * Speed * Time.fixedDeltaTime;
                    Debug.Log(newY);
                }
                else
                {
                    ConstrainY(ref newY);
                }
                gameObject.transform.position = new Vector3(newX, newY, gameObject.transform.position.z);
                _ReachedTarget = doneX && doneY & doneSize;
                if (_ReachedTarget)
                {
                    if (_ICameraConfiguration.PositioningMode.Equals(CameraPositioningMode.ExactPoint))
                        newY = _ICameraConfiguration.Y;
                    if (_ICameraConfiguration.PositioningMode.Equals(CameraPositioningMode.ExactPoint))
                        newX = _ICameraConfiguration.X;
                    newSize = _ToSize;
                    if (_ICameraConfiguration.AllowSwipe)
                    {
                        _CameraSwiper.SetBounds(_ICameraConfiguration.Bounds);
                        _CameraSwiper.Direction = _ICameraConfiguration.Directions;
                        _CameraSwiper.Enabled = true;
                    }
                }
                return _ReachedTarget;

            }
            return true;
        }
        private void CalculateBoundsCrossing()
        {
            _BoundsCrossingPoint = MyGeometry.GetClosestLineIntersectRectanglePoints(_ShiftedCameraBounds.Rect, gameObject.transform.position);
        }
        public float GetSizeToZoomRatio()
        {
            bool fromWidth = true;
            if (_ICameraConfiguration.CameraBoundsFit.Equals(CameraBoundsFit.Best))
            {
                float boundsAspectRatio = _ICameraConfiguration.Bounds.width / _ICameraConfiguration.Bounds.height;
                fromWidth =(CameraAspectRatio < boundsAspectRatio) ;
            }
            else
            {
                if (_ICameraConfiguration.CameraBoundsFit.Equals(CameraBoundsFit.Vertically))
                {
                    fromWidth = false;
                }
            }
            return fromWidth?_ICameraConfiguration.Bounds.width / (Camera.aspect * 2):_ICameraConfiguration.Bounds.height / 2;
        }
        private void ConstrainY(ref float y)
        {
            if (_ShiftedCameraBounds != null)
            {
                if (y < _ShiftedCameraBounds.Bottom)
                {
                    y = _ShiftedCameraBounds.Bottom;
                }
                else
                {
                    if (y > _ShiftedCameraBounds.Top)
                    {
                        y = _ShiftedCameraBounds.Top;
                    }
                }
            }
        }
        private void ConstrainX(ref float x)
        {
            if (_ShiftedCameraBounds != null)
            {
                if (x < _ShiftedCameraBounds.Left)
                {
                    x = _ShiftedCameraBounds.Left;
                }
                else
                {
                    if (x > _ShiftedCameraBounds.Right)
                    {
                        x = _ShiftedCameraBounds.Right;
                    }
                }
            }
        }
        private void ConstrainSize(ref float size)
        {
            if (_ICameraConfiguration.AllowZoom)
            {
                if (size < MinSize)
                {
                    size = MinSize;
                }
                else
                {
                    if (size > MaxSize)
                    {
                        size = MaxSize;
                    }
                }
            }
        }
        public void SetConfiguration(ICameraConfiguration iCameraConfiguration)
        {
            _CameraSwiper.ZoomEnabled = false;
            _CameraSwiper.Enabled = false;
            _ReachedTarget = false;
            _ICameraConfiguration = iCameraConfiguration;
            _ToSize = Camera.orthographicSize > MaxSize ? MaxSize : (Camera.orthographicSize < MinSize ? MinSize : Camera.orthographicSize);
            _ShiftedCameraBounds = CalculateShiftedBounds(_ToSize);
            if (iCameraConfiguration.PositioningMode.Equals(CameraPositioningMode.ExactPoint))
            {
                _Direction = new Vector2(iCameraConfiguration.X - gameObject.transform.position.x, iCameraConfiguration.Y - gameObject.transform.position.y).normalized;
                XIsDone = new Func<float, bool>((x) =>
                {
                    return (_Direction.x > 0 ? x > _ICameraConfiguration.X : x < _ICameraConfiguration.X);
                });
                YIsDone = new Func<float, bool>((y) =>
                {
                    return (_Direction.y > 0 ? y > _ICameraConfiguration.Y : y < _ICameraConfiguration.Y);
                });
            }
            else
            {
                CalculateBoundsCrossing();
                _Direction = new Vector2(_BoundsCrossingPoint.x - gameObject.transform.position.x, _BoundsCrossingPoint.y - gameObject.transform.position.y).normalized;
                if (gameObject.transform.position.x < _BoundsCrossingPoint.x)
                {
                    XIsDone = new Func<float, bool>((x) =>
                    {
                        return (x >= _BoundsCrossingPoint.x || MyMaths.Approximately(x, _BoundsCrossingPoint.x, 0.01f));
                    });
                }
                else
                {

                    XIsDone = new Func<float, bool>((x) =>
                    {
                        return (x <= _BoundsCrossingPoint.x || MyMaths.Approximately(x, _BoundsCrossingPoint.x, 0.01f));
                    });
                }

                if (gameObject.transform.position.y < _BoundsCrossingPoint.y)
                {
                    YIsDone = new Func<float, bool>((y) =>
                    {
                        return (y >= _BoundsCrossingPoint.y || MyMaths.Approximately(y, _BoundsCrossingPoint.y, 0.01f));
                    });
                }
                else
                {
                    YIsDone = new Func<float, bool>((y) =>
                    {
                        return (y <= _BoundsCrossingPoint.y|| MyMaths.Approximately(y , _BoundsCrossingPoint.y, 0.01f));
                    });

                }
                if (iCameraConfiguration.AllowZoom)
                {
                    _CameraSwiper.ZoomEnabled = true;
                }
            }
            _DSize = _ToSize > Camera.orthographicSize ? (SizeChangeSpeed > 0 ? SizeChangeSpeed : -SizeChangeSpeed) : (SizeChangeSpeed > 0 ? -SizeChangeSpeed : SizeChangeSpeed);
            _ILooper.AddFixedUpdate(this);
            Debug.Log("added");
        }
        private void ReConstrain()
        {
            float halfCameraHeight = Camera.orthographicSize;
            float halfCameraWidth = halfCameraHeight * Camera.main.aspect;
            SetSize(Camera.orthographicSize);
            SetX(gameObject.transform.position.x);
            SetY(gameObject.transform.position.y);
        }
        public void SetSize(float size)
        {
            ConstrainSize(ref size);
            Camera.orthographicSize = size;
            _ShiftedCameraBounds = CalculateShiftedBounds(size);
        }
        public void SetX(float x)
        {
            ConstrainX(ref x);
            gameObject.transform.position = new Vector3(x, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        public void SetY(float y)
        {
            ConstrainY(ref y);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z);
        }
        public Vector3 GetPosition()
        {
            return gameObject.transform.position;

        }
        public float GetSize()
        {
            return Camera.orthographicSize;
        }
        public ICameraConfiguration GetConfiguration()
        {
            return _ICameraConfiguration;
        }
        public void RotationEventHandler(RotatedEventArgs rotatedEventArgs)
        {
            ReConstrain();
            if (_ICameraConfiguration != null)
                SetConfiguration(_ICameraConfiguration);
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
            if (typeof(ITouchSensor).IsAssignableFrom(type))
                _CameraSwiper.SetInterface(o);
            if (typeof(IRotationSensor).IsAssignableFrom(type))
            {
                _RotationHandler = new RotationHandler(RotationEventHandler);
                ((IRotationSensor)o).AddEventHandler(_RotationHandler);
            }
        }
    }
}