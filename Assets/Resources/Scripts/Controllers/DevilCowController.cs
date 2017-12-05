using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class DevilCowController:ILooper2Hz
    {
        private const float MIN_ENGAGE_TIME = 3;
        private const float IDEAL_DISTANCE_MIN = 1f;
        private const float IDEAL_DISTANCE_MAX = 2f;
        private const float SWITCH_FIGHTING_WITH_PROBABILITY = 10;
        private TelekinesisController _TelekinesisController;
        private IDevilCowMarkers _IDevilCowMarkers;
        private IGetRect _IGetRect;
        private IFightable _IFightable;
        private   float[] _SteppedDistances;
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
                        float distance = IDEAL_DISTANCE_MIN + ((i * (IDEAL_DISTANCE_MAX - IDEAL_DISTANCE_MIN)) / 10);
                        _SteppedDistances[i] = distance;
                        i++;
                    }
                }
                return new List<float>((float[])_SteppedDistances.Clone());
            }
        }
        private bool _Done = false;
        private bool Done
        {
            get
            {
                return _Done;
            }
            set
            {
                if (value)
                {
                    _Dispose();
                }
                _Done = value;
            }
        }
        private bool _Disposed = false;
        private void _Dispose()
        {
            if (!_Disposed)
            {
                _Disposed = true;
                _TelekinesisController.Dispose();
                _FighterController.Dispose();
            }
        }
        private FighterController _FighterController;
        public FighterController FighterController
        {
            get
            {
                return _FighterController;
            }
        }
        private ILooper _ILooper;
        private IIsRepositioning _IIsRepositioning;
        public DevilCowController(IIsRepositioning iIsRepositioning, HealthBar healthBar, IFightsController iFightsController, Rect[] allowedRegions, IResourceHelper iResourceHelper, IFightable iFightable, Transform transform, IGetRect iGetRect, IBodyType iBodyType)
        {
            _IIsRepositioning = iIsRepositioning;
               _ILooper = iResourceHelper.Get<ILooper>();
            _IGetRect = iGetRect;
            _IDevilCowMarkers = iResourceHelper.Get<IDevilCowMarkers>();
            _IFightable = iFightable;
            _TelekinesisController = new TelekinesisController(iResourceHelper, transform, allowedRegions, iGetRect) { ReachedTarget = (value) => { _IIsRepositioning.IsRepositioning = !value; } };
            //_TelekinesisController.TeleportTo(iSuperMooMarkers.SuperMooFlyToStartPosition);
            _FighterController = new FighterController(healthBar, iFightsController, iFightable, _TelekinesisController, iResourceHelper, transform, MIN_ENGAGE_TIME, SWITCH_FIGHTING_WITH_PROBABILITY);
            _ILooper.Add2Hz(this);
        }
        public void Dispose()
        {
            Done = true;
        }
        public bool Looper2Hz()
        {
            if (Decisions.Teleport(_FighterController, IDEAL_DISTANCE_MIN, _IIsRepositioning))
                Reposition();
            return Done;
        }
        private void Reposition()
        {
            Vector2? a = _IFightable.GetPosition2();
            if (a != null)
            {
                Vector2? b = MyGeometry.GetCloseTo((Vector2)a, _IDevilCowMarkers.AllowedRegionsDevilCow, IDEAL_DISTANCE_MIN, IDEAL_DISTANCE_MAX, _IGetRect, SteppedDistances);
                if (b != null)
                {
                    _TelekinesisController.TeleportTo((Vector2)b);
                }
            }
        }
        private class Decisions
        {
            public static bool Teleport(FighterController fighterController, float idealDistance, IIsRepositioningGet iIsRepositioningEnemyGet)
            {
                if (fighterController.IsActing)
                {
                    if (iIsRepositioningEnemyGet != null && !iIsRepositioningEnemyGet.IsRepositioning)
                    {
                        float? distanceFromEnemey = fighterController.DistanceFromEnemy;
                        if (distanceFromEnemey != null)
                        {
                            if ((float)distanceFromEnemey < idealDistance)
                            {
                                return new RouletteWheel<bool>(new RouletteSlot<bool>[] { new RouletteSlot<bool>(100f, true), new RouletteSlot<bool>(10f, false) }).Spin();
                            }
                            else
                                return new RouletteWheel<bool>(new RouletteSlot<bool>[] { new RouletteSlot<bool>(5f, true), new RouletteSlot<bool>(95f, false) }).Spin();
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return new RouletteWheel<bool>(new RouletteSlot<bool>[] { new RouletteSlot<bool>(1f, true), new RouletteSlot<bool>(100f, false) }).Spin();
                }
                return false;
            }
        }
    }
}