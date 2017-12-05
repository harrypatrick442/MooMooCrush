using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class ShiftedCameraBounds
    {
        public float Top;
        public float Bottom;
        public float Left;
        public float Right;
        public Rect Rect;
        public ShiftedCameraBounds(Rect bounds, float halfCameraWidth, float halfCameraHeight)
        {
            if(halfCameraHeight>bounds.height/2)
            {
                Top = bounds.y - (bounds.height / 2);
                Bottom = Top;
            }
            else
            {
                Top = bounds.y - halfCameraHeight;
                Bottom = bounds.y + halfCameraHeight - bounds.height;
            }
            if(halfCameraWidth>bounds.width/2)
            {
                Left = bounds.x + (bounds.width / 2);
                Right = Left;
            }
            else
            {
                Left = bounds.x + halfCameraWidth;
                Right = bounds.x - halfCameraWidth + bounds.width;
            }
            Rect = new Rect(Left, Top, (Right -Left),(Top-Bottom));
        }
    }
}