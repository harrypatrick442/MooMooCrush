using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ICameraConfiguration
    {
        float X { get; }
        float Y { get; }
        float ZoomMin { get; }
        float ZoomMax { get; }
        bool AllowSwipe { get; }
        bool AllowZoom { get; }
        CameraSwiperDirections Directions { get; }
        Rect Bounds { get; }
        CameraPositioningMode PositioningMode { get; }
        CameraBoundsFit CameraBoundsFit{get;}
    }
}