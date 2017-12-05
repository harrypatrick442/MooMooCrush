using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IMotionController : ILooperFixedUpdate, IMove, IDisposable
    {
        void MoveTo(Vector2 position);
        void MoveTo(Transform transform);
        void Cancel();
        void AddEventHandlerReachedTarget(MotionControllerEventHandler h);
        void RemoveEventHandlerReachedTarget(MotionControllerEventHandler h);
        ILooper ILooper { set; }
    }
}
