using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IMotionControllerRadual : ILooperFixedUpdate, IDisposable
    {
        void Orbit(Vector2 point, float angularVelocityDegreesPerSecond, float toAngleDegrees, bool clockwise, float endRadius, Action<Angle> callback, Action reachedTarget);
        void Cancel();
        ILooper ILooper { set; }
    }
}
