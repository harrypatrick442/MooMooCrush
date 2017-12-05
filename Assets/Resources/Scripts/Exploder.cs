using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Exploder:MonoBehaviour, IExploder
    {
        private ILooper _ILooper;
        private IExploderEvents _IExploderEvents;
        public void Explode(Vector2 position, ExplosionInfo explosionInfo)
        {
            if (_IExploderEvents != null)
                _IExploderEvents.BeforeExplosion();
            new Explosion(transform, position, _ILooper, explosionInfo);
            if (_IExploderEvents != null)
                _IExploderEvents.AfterExplosion();
        }
        public void SetInterfaces(object o)
        {
            if (typeof(ILooper).IsAssignableFrom(o.GetType()))
                _ILooper = (ILooper)o;
            if (typeof(IExploderEvents).IsAssignableFrom(o.GetType()))
                _IExploderEvents = (IExploderEvents)o;
        }
    }
}
