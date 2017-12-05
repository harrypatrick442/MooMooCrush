using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace Assets.Scripts
{
    public class SuperMooController:ILooper2Hz
    {
        private const float IDEAL_DISTANCE_MIN = 1f;
        private const float IDEAL_DISTANCE_MAX = 2f;
        private float MIN_ENGAGE_TIME = 3;
        private const float SWITCH_FIGHTING_WITH_PROBABILITY = 10;
        private enum States { Flying, Fighting,  Straddling}
        private States _State;
        private SuperMooFlightController _SuperMooFlightController;
        private FighterController _FighterController;
        public FighterController FighterController
        {
            get { return _FighterController; }
        }
        private ILooper _ILooper;
        private IFightable _IFightable;
        private ISuperMooMarkers _ISuperMooMarkers;
        private IGetRect _IGetRect;
        private IIsRepositioning _IIsRepositioning;
        private float[] _SteppedDistances;
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
                _SuperMooFlightController.Dispose();
                _FighterController.Dispose();
            }
        }
        public SuperMooController(IIsRepositioning iIsRepositioning, HealthBar healthBar, IFightsController iFightsController,IResourceHelper iResourceHelper, IFightable iFightable, ISuperMooMarkers iSuperMooMarkers, Transform transform, IGetRect iGetRect, IBodyType iBodyType)
        {
            _ILooper = iResourceHelper.Get<ILooper>();
            _ILooper.Add2Hz(this);
            _IIsRepositioning = iIsRepositioning;
            _ISuperMooMarkers = iResourceHelper.Get<ISuperMooMarkers>();
            _IFightable = iFightable;
            _IGetRect = iGetRect;
            _SuperMooFlightController = new SuperMooFlightController((value)=> { iIsRepositioning.IsRepositioning = value; }, _ILooper, transform, iSuperMooMarkers.AllowedRegionsSuperMoo, iGetRect, iBodyType) { MinDistanceFromTarget = 1F, MaxDistanceFromTarget=2 };
            _SuperMooFlightController.FlyTo(iSuperMooMarkers.SuperMooFlyToStartPosition);
            _FighterController = new FighterController(healthBar, iFightsController, iFightable, _SuperMooFlightController, iResourceHelper, transform, MIN_ENGAGE_TIME, SWITCH_FIGHTING_WITH_PROBABILITY);
        }
        public void Dispose()
        {
            Done = true;
        }
        public bool Looper2Hz()
        {
            if (Decisions.Teleport(_FighterController, IDEAL_DISTANCE_MIN, _IFightable.FighterController.CurrentlyEngaging))
                Reposition();
            return Done;
        }
        private void Reposition()
        {
            Vector2? a = _IFightable.GetPosition2();
            if (a != null)
            {
                Vector2? b = MyGeometry.GetCloseTo((Vector2)a, _ISuperMooMarkers.AllowedRegionsSuperMoo, IDEAL_DISTANCE_MIN, IDEAL_DISTANCE_MAX, _IGetRect, SteppedDistances);
                if (b != null)
                {
                    _SuperMooFlightController.FlyTo((Vector2)b);
                }
            }
        }
        private class Decisions
        {
            public static bool Teleport(FighterController fighterController, float idealDistance, IIsRepositioningGet iIsRepositioningEnemyGet )
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