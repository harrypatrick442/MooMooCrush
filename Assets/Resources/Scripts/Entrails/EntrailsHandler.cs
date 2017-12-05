using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class EntrailsHandler:MonoBehaviour, IEntrailsHandler, ILooperFixedUpdate
    {
        private ILooper _ILooper;
        private class ScatterInfo
        {
            private Entrails _Entrails;
            public float StartTime;
            public float Duration
            {
                get
                {
                    return _Entrails.Duration;
                }
            }
            public void Destroy()
            {
                _Entrails.Destroy();
            }
            public ScatterInfo(Entrails entrails)
            {
                _Entrails = entrails;
                StartTime = Time.time;
            }
        }
        private List<ScatterInfo> _ScatterInfos = new List<ScatterInfo>();
        public void Scatter(Vector2 position, Type typeEntrails)
        {
            if (!( typeof(Entrails).IsAssignableFrom(typeEntrails)))
                throw new Exception("The type " + typeEntrails.ToString() + " is not assignable from the type Entrails");
            Entrails entrails = (Entrails)Activator.CreateInstance(typeEntrails);
            entrails.Initialize(gameObject.transform, position);
            _ScatterInfos.Add(new ScatterInfo(entrails));
            _ILooper.AddFixedUpdate(this);
        }
        public void SetInterfaces(object o)
        {
            if (typeof(ILooper).IsAssignableFrom(o.GetType()))
                _ILooper = (ILooper)o;
        }
        public bool LooperFixedUpdate()
        {
            List<ScatterInfo> removes = new List<ScatterInfo>();
            foreach(ScatterInfo scatterInfo in _ScatterInfos)
            {
                if (Time.time - scatterInfo.StartTime > scatterInfo.Duration)
                {
                    scatterInfo.Destroy();
                    removes.Add(scatterInfo);
                }
            }
            foreach (ScatterInfo remove in removes)
                _ScatterInfos.Remove(remove);
            return _ScatterInfos.Count<1;
        }
    }
}
