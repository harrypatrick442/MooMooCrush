using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    class TractorSensor: IDisposable, ILooperFixedUpdate
    {
        private float _MinWidthTargets;
        private ITractor _ITractor;
        private ILooper _ILooper;
        public ILooper ILooper
        {
            get
            {
                return _ILooper;
            }
            set
            {
                if (_ILooper == null)
                    _ILooper = value;
                _ILooper.AddFixedUpdate(this);
            }
        }
        public TractorSensor(ITractor iTractor, float minWidthTargets)
        {
            _MinWidthTargets = minWidthTargets;
            _ITractor = iTractor;
        }
        private void Scan()
        {
            Vector2 recipricolPosition = _ITractor.GetRecipricolTransform().position;
            Vector2 direction = _ITractor.GetBeamDirection();
            Vector2 directionNormalized = direction.normalized;
            float beamWidth = _ITractor.GetWidth();
            Vector2  normal = new Vector2(directionNormalized.y * beamWidth, -directionNormalized.x * beamWidth);
            Vector2 positionLeft = recipricolPosition - new Vector2(normal.x / 2, normal.y / 2);
            float step = _MinWidthTargets / beamWidth;
            if (step > 1)
                step = 1;
            float length = _ITractor.GetHeight();
            int i = 0;
            List<ITractable> hitITractibles = new List<ITractable>();
            #region Hits
            bool run = true;
            while (run)
            {
                float scalingFactor = i * step;
                if (scalingFactor > 1)
                {
                    run = false;
                    scalingFactor = 1;
                }
                Vector2 origin = positionLeft + new Vector2(normal.x * scalingFactor, normal.y * scalingFactor);
                RaycastHit2D[] hits = Physics2D.RaycastAll(origin, directionNormalized, length);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit)
                    {
                        ITractable iTractable = Lookup.GetBody<ITractable>(hit.collider.gameObject);
                        if (iTractable != null)
                        {
                            if (!hitITractibles.Contains(iTractable))
                                hitITractibles.Add(iTractable);
                        }
                    }
                }
                i++;
            }
            #endregion
            #region UpdateInterractions
            EventDictionary<ITractable, TractoringInfo> interractions = _ITractor.GetInterractions();
            List<ITractable> toAdd = new List<ITractable>();
            foreach (ITractable hit in hitITractibles)
            {
                if (!interractions.Keys.Contains(hit))
                {
                    if (!hit.TractableIsTaken&&hit.TractableEnabled)
                    {
                        toAdd.Add(hit);
                    }
                }
            }
            List<ITractable> toRemove = new List<ITractable>();
            foreach (ITractable iTractable in interractions.Keys)
            {
                if (!hitITractibles.Contains(iTractable))
                {
                    toRemove.Add(iTractable);
                }
            }
            foreach (ITractable iTractable in toRemove)
            {
                TractoringInfo tractoringInfo = interractions[iTractable];
                tractoringInfo.Removing();
                interractions.Remove(iTractable);
            }
            foreach (ITractable iTractable in toAdd)
            {
                _ITractor.AddInterraction(iTractable);
            }
            #endregion
        }
        private StopWatch _StopWatchScan = new StopWatch();
        private bool _Done = false;
        public bool LooperFixedUpdate()
        {
            if (_StopWatchScan.GetMilliseconds()  >  250)
            {
                _StopWatchScan.Reset();
                Scan();
            }
            return _Done;
        }
        public void Dispose()
        {
            _Done = true;
        }
    }
}
