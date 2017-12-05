using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TelekinesisController : IGetCloseTo, ILooperFixedUpdate
    {

        private const float MIN_DISTANCE_FROM_TARGET = 1f;
        private const float MAX_DISTANCE_FROM_TARGET = 2f;
        private float[] _SteppedDistances;
        private Action<bool> _ReachedTarget;
        public Action<bool> ReachedTarget
        {
            set
            {
                _ReachedTarget = value;
            }
        }
        private List<float> SteppedDistances
        {
            get
            {
                if (_SteppedDistances == null)
                {
                    _SteppedDistances = new float[11];
                    int i = 0;
                    while (i <= 10)
                    {
                        float distance = MIN_DISTANCE_FROM_TARGET + ((i * (MAX_DISTANCE_FROM_TARGET - MIN_DISTANCE_FROM_TARGET)) / 10);
                        _SteppedDistances[i] = distance;
                        i++;
                    }
                }
                return new List<float>((float[])_SteppedDistances.Clone());
            }
        }
        private Transform _Transform;
        private IGetRect _IGetRect;
        private ILooper _ILooper;
        private ITeleportationSimulationHandler _ITeleportationSimulationHandler;
        private Rect[] _AllowedRegions;
        public TelekinesisController(IResourceHelper iResourceHelper, Transform transform, Rect[] allowedRegions, IGetRect iGetRect)
        {
            _ILooper = iResourceHelper.Get<ILooper>();
            _ITeleportationSimulationHandler = iResourceHelper.Get<ITeleportationSimulationHandler>();
            _IGetRect = iGetRect;
            _Transform = transform;
            _AllowedRegions = allowedRegions;
        }
        public bool LooperFixedUpdate()
        {
            if (_ReachedTarget != null)
                _ReachedTarget(true);
            return true;
        }
        public void TeleportTo(Vector2 position)
        {
            if (_Transform != null)
            {
                _ITeleportationSimulationHandler.SimulateTeleportation(_Transform.position, position, 0.4f, () =>
                {
                    if (_Transform != null)
                    {
                        _Transform.position = position;
                    }
                    if (_ReachedTarget != null)
                    {
                        _ReachedTarget(false);
                        _ILooper.AddFixedUpdate(this);
                    }
                });
            }
        }
        public void GetCloseTo(Vector2 position)
        {
            Vector2? close = MyGeometry.GetCloseTo(position, _AllowedRegions, MIN_DISTANCE_FROM_TARGET, MAX_DISTANCE_FROM_TARGET, _IGetRect, SteppedDistances);
            if (close != null)
                TeleportTo((Vector2)close);
        }
        public void TeleportCloseTo(Vector2 position, float dist)
        {
            GetCloseTo(position);
        }
        public void Dispose()
        {

        }
    }
}