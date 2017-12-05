using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ICameraController
    {
        float UnitsPerPixel { get; }
        void SetConfiguration(ICameraConfiguration iCameraConfiguration);
        ICameraConfiguration GetConfiguration();
        Vector3 GetPosition();
        float GetSize();
        //float GetZoomToSizeRatio();
        void SetX(float x);
        void SetY(float y);
        void SetSize(float size);
    }
}