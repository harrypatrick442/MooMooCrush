using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraConfiguration:ICameraConfiguration
    {
        private float _X;
        public float X { get { return _X; } }
        public float _Y;
        public float Y { get { return _Y; } }
        private float _ZoomMax;
        public float ZoomMax { get { return _ZoomMax; } }
        public float _ZoomMin;
        public float ZoomMin { get { return _ZoomMin; } }
        public bool _AllowSwipe = false;
        public bool AllowSwipe { get { return _AllowSwipe; } }
        public bool _AllowZoom = false;
        public bool AllowZoom { get { return _AllowZoom; } }
        private CameraBoundsFit _CameraBoundsFit;
        public CameraBoundsFit CameraBoundsFit
        {
            get
            {
                return _CameraBoundsFit;
            }
        }
        private CameraPositioningMode _PositioningMode;
        public CameraPositioningMode PositioningMode
        {
            get
            {
                return _PositioningMode;
            }
        }
        private CameraSwiperDirections _Directions;
        public CameraSwiperDirections Directions
        {
            get {
                return _Directions;
            }
        }private Rect _Bounds;
        public Rect Bounds
        {
            get
            {
                return _Bounds;
            }
        }
        /*public CameraConfiguration(float x, float y, float size)
        {
            _X = x;
            _Y = y;
            _ZoomMin = size;
            _PositioningMode = CameraPositioningMode.ExactPoint;
        }*/
        public CameraConfiguration(Rect bounds, CameraBoundsFit cameraBoundsFit, bool allowSwipe,  float?zoomMin=null, CameraSwiperDirections directions = CameraSwiperDirections.All, float? sizeMax = null)
        {
            _CameraBoundsFit = cameraBoundsFit;
            _PositioningMode = CameraPositioningMode.WithinBounds;
            _Directions = directions;
            if (allowSwipe)
                _Bounds = bounds;
            _AllowSwipe = allowSwipe;
            _X = bounds.x + (bounds.width / 2);
            _Y = bounds.y - (bounds.height / 2f);
            if (zoomMin != null)
            {
                _ZoomMin =(float)zoomMin;
                //_ZoomMin = (float)zoomMin < 1 ? 1 : (float)zoomMin;
                if (sizeMax != null)
                {
                    _AllowZoom = true;
                    _ZoomMax = (float)sizeMax;
                }
                else
                    _ZoomMax = _ZoomMin;
            }
            else
            {
                _ZoomMin = 1;
                _ZoomMax = _ZoomMin;
            }
        }
        public static implicit operator Vector2(CameraConfiguration cameraConfiguration)
        {
            return new Vector2(cameraConfiguration.X, cameraConfiguration.Y);

        }
    }
}