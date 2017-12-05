using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ITractable: IGetTransform, IIsAlive, IDisposeCallbackHandler, IBodyType, IGetDefaultBodyType, IDisposable, IMassless
    {
        void TakeTractable(WeakReference o);
        void ReleaseTractable();
        bool TractableIsTaken { get; }
        bool TractableEnabled { get; }
        Rigidbody2D Rigidbody2D { get; }
    }
}