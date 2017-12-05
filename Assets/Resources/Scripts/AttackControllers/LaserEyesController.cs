using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class LaserEyesController : IFightController, ILooperUpdate, ILooper5Hz, IPausible
    {
        private ILaserEyesControllable _ILaserEyesControllableA;
        private ILaserEyesControllable _ILaserEyesControllableB;
        private IResourceHelper _IResourceHelper;
        private ILooper _ILooper;
        private bool _IsDone = false;
        private bool _Disposed = false;
        private bool _Paused = false;
        private bool _DoingLaseredA = false;
        private bool _DoingLaseredB = false;
        public bool IsDone
        {
            get
            {
                return _IsDone;
            }
        }
        private bool Done
        {
            get
            {
                return _IsDone;
            }
            set
            {
                if (value)
                {
                    _Dispose();
                }
                _IsDone = value;
            }
        }
        private void _Dispose()
        {
            if (!_Disposed)
            {
                _Disposed = true;
                if (_ILaserEyesControllableA.ILaserEyes != null)
                {
                    _ILaserEyesControllableA.ILaserEyes.HideLaserEyesLasers();
                }
                if (_ILaserEyesControllableB.ILaserEyes != null)
                {
                    _ILaserEyesControllableB.ILaserEyes.HideLaserEyesLasers();
                }
            }
        }
        public LaserEyesController(FightAction a, FightAction b, IResourceHelper interfacesHelper)
        {
            _IResourceHelper = interfacesHelper;
            _ILooper = _IResourceHelper.Get<ILooper>();
            _ILooper.AddUpdate(this);
            _ILooper.Add5Hz(this);
            _ILaserEyesControllableA = (ILaserEyesControllable)a.IViolence;
            _ILaserEyesControllableB = (ILaserEyesControllable)b.IViolence;
        }
        public void Start()
        {
            _ILooper.AddUpdate(this);
            _ILaserEyesControllableA.ILaserEyes.ShowLaserEyesLasers();
            _ILaserEyesControllableB.ILaserEyes.ShowLaserEyesLasers();
        }
        public void Tick()
        {

        }
        public void Stop()
        {
            _Dispose();
        }
        private float getProportionA()
        {
                float divisor = _ILaserEyesControllableB.ILaserEyes.Health.Proportion + _ILaserEyesControllableA.ILaserEyes.Health.Proportion;
                return divisor <= 0 ? 0 : _ILaserEyesControllableA.ILaserEyes.Health.Proportion / (divisor);
        }
        private void UpdateProportion()
        {
            if (!_Paused)
            {
                if (!Done)
                {
                    if (_ILaserEyesControllableA.ILaserEyes == null
                        || _ILaserEyesControllableB.ILaserEyes == null
                        || _ILaserEyesControllableB.ILaserEyes.Health == null
                        || _ILaserEyesControllableA.ILaserEyes.Health == null)
                    {
                        Done = true;
                        return;
                    }
                    float proportionA = getProportionA();
                    bool doUpdateProportion = !_DoingLaseredA && !_DoingLaseredB;
                    if (_ILaserEyesControllableA.ILaserEyes.Health.Proportion <= 0)
                    {
                        if (!_DoingLaseredA)
                        {
                            _DoingLaseredA = true;
                            _ILaserEyesControllableA.ILaserEyes.Lasered();
                        }
                        else
                        {
                            if (_ILaserEyesControllableA.ILaserEyes.IsLasered)
                            {
                                Done = true;
                            }
                        }
                    }
                    else
                    {
                        if (_ILaserEyesControllableB.ILaserEyes.Health.Proportion <= 0)
                        {
                            if (!_DoingLaseredB)
                            {
                                _DoingLaseredB = true;
                                _ILaserEyesControllableB.ILaserEyes.Lasered();
                            }
                            else
                            {
                                if (_ILaserEyesControllableB.ILaserEyes.IsLasered)
                                {
                                    Done = true;
                                }
                            }
                        }
                        else
                        {
                            if (doUpdateProportion)
                            {
                                _ILaserEyesControllableB.ILaserEyes.Health.Proportion -= GetChangeInHealth(proportionA);

                                if (_ILaserEyesControllableB.ILaserEyes.Health.Proportion > 0)
                                {
                                    _ILaserEyesControllableA.ILaserEyes.Health.Proportion -= GetChangeInHealth(1 - proportionA);
                                }
                            }
                        }
                    }
                }
            }
        }
        private float MAX_CHANGE_HEALTH_PER_SECOND = 0.2F;
        private float MIN_CHANGE_HEALTH_PER_SECOND = 0.05F;
        private float GetChangeInHealth(float proportionHealth)
        {
            proportionHealth = proportionHealth * Random.Range(0f, 2f);
            proportionHealth = proportionHealth > 1 ? 1 : proportionHealth;
            return(MIN_CHANGE_HEALTH_PER_SECOND+((MAX_CHANGE_HEALTH_PER_SECOND- MIN_CHANGE_HEALTH_PER_SECOND)* proportionHealth)) / 5;
        }
        public bool LooperUpdate()
        {
            if (!_Paused)
            {
                if (!Done)
                {
                    if (_ILaserEyesControllableA.ILaserEyes == null
                        || _ILaserEyesControllableB.ILaserEyes == null
                        || _ILaserEyesControllableB.ILaserEyes.Health == null
                        || _ILaserEyesControllableA.ILaserEyes.Health == null)
                    {
                        Done = true;
                        return true;
                    }
                    float proportionA = getProportionA();
                    Vector2? positionALeft = _ILaserEyesControllableA.ILaserEyes.GetLeftEyePosition();
                    Vector2? positionBLeft = _ILaserEyesControllableB.ILaserEyes.GetLeftEyePosition();
                    Vector2? positionARight = _ILaserEyesControllableA.ILaserEyes.GetRightEyePosition();
                    Vector2? positionBRight = _ILaserEyesControllableB.ILaserEyes.GetRightEyePosition();
                    if (positionALeft != null && positionBLeft != null && positionARight != null && positionARight != null)
                    {
                        Vector2 meetPointLeft = GetLaserMeetPoint((Vector2)positionALeft, (Vector2)positionBLeft, proportionA);
                        Vector2 meetPointRight = GetLaserMeetPoint((Vector2)positionARight, (Vector2)positionBRight, proportionA);
                        _ILaserEyesControllableA.ILaserEyes.SetLasersPositions(meetPointLeft, meetPointRight);
                        _ILaserEyesControllableB.ILaserEyes.SetLasersPositions(meetPointLeft, meetPointRight);
                    }
                    else
                        Done = true;
                }
            }
            return Done;
        }
        private Vector2 GetLaserMeetPoint(Vector2 a, Vector2 b, float proportionA)
        {
            Vector2 distance = b - a;
            float sf = proportionA;
            return a + (new Vector2(distance.x * sf, distance.y * sf));
        }

        public bool Looper5Hz()
        {
            UpdateProportion();
            return Done;
        }

        public void Pause()
        {
            _Paused = true;
        }

        public void Unpause()
        {
            _Paused = false;
        }

        public static FightControllerUseInfo UserInfo {
            get
            {
                return new FightControllerUseInfo(new Func<FightAction, FightAction, IResourceHelper, IFightController>((a, b, interfacesHelper) => { return new LaserEyesController(a, b, interfacesHelper); }), typeof(LaserEyes), typeof(LaserEyes));
            }
        }/*
        private class Decisions
        {
            public static bool StopFighting() {//higher after hit. never for supermoo. caused by teleporation of 
                return false;
            }
            public static bool DoHit(float proportionStrengthA, float proportionStrengthB)
            {
                float divisor = proportionStrengthA + proportionStrengthB;
                float difference = proportionStrengthA - proportionStrengthB;
                float differenceMag = difference * Mathf.Sign(difference);
                float proportionDifference = differenceMag / divisor;
                const float MIN_PERCENT = 10;
                const float MAX_PERCENT = 90;
                float finalPercent = (MIN_PERCENT + ((MAX_PERCENT - MIN_PERCENT) * proportionDifference));
                return new RouletteWheel<bool>(new RouletteSlot<bool>[] { new RouletteSlot<bool>(finalPercent, true), new RouletteSlot<bool>(100 - finalPercent, false) }).Spin();
            }
            public static bool AScoresHit(float proportionStrengthA, float proportionStrengthB)
            {
                float divisor = (proportionStrengthA + proportionStrengthB) / 100;
                return new RouletteWheel<bool>(new RouletteSlot<bool>[] { new RouletteSlot<bool>(proportionStrengthA / divisor, true), new RouletteSlot<bool>(proportionStrengthB / divisor, false) }).Spin();
            }
            public static bool ProportionStrength()
            {
                return false;
            }
            public static bool MoveTowardsA()
            {
                return false;
            }
        }*/
    }
}