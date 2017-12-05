using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class SonicMooController : IFightController, ILooperUpdate, ILooper5Hz, IPausible
    {
        private float MAX_CHANGE_HEALTH = 0.35F;
        private float MIN_CHANGE_HEALTH = 0.2f;
        private float DURATION = 1.5f;
        public enum MooType { OneWay, Reply}
        private ISonicMooControllable _ISonicMooControllableA;
        private ISonicMooControllable _ISonicMooControllableB;
        private IDefenceControllable _IDefenceControllableB;
        private IResourceHelper _IResourceHelper;
        private ILooper _ILooper;
        private IPopHandler _IPopHandler;
        private IMoosics _IMoosics;
        private IVibrator _IVibrator;
        private bool _IsDone = false;
        private bool _Disposed = false;
        private bool _Paused = false;
        private Action Update;
        private Action Update5Hz;
        private Moooooosic _Moooooosic;
        private enum State { MusicFirst, HealthFirst, MusicSecond, HealthSecond }
        private State _State = State.MusicFirst;
        private float _StartStateTime;
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
                if(_Moooooosic!=null)
                    _Moooooosic.Dispose();
                _Disposed = true;
            }
        }
        public SonicMooController(FightAction a, FightAction b, IResourceHelper interfacesHelper, MooType type)
        {
            _IResourceHelper = interfacesHelper;
            _ILooper = _IResourceHelper.Get<ILooper>();
            _IPopHandler = _IResourceHelper.Get<IPopHandler>();
            _IMoosics = _IResourceHelper.Get<IMoosics>();
            _IVibrator = _IResourceHelper.Get<IVibrator>();
            _ILooper.AddUpdate(this);
            _ILooper.Add5Hz(this);
            _ISonicMooControllableA = (ISonicMooControllable)a.IViolence;
            switch (type)
            {
                case MooType.OneWay:
                    _IDefenceControllableB = (IDefenceControllable)b.IViolence;
                    _IVibrator.Vibrate(new Vibratable(_IDefenceControllableB.IDefencable, 10f, DURATION) { EndAmplitude = 0.05f, StartAmplitude = 0.00f });
                    Update = new Action(() => {
                            Vector2? positionA = _ISonicMooControllableA.ISonicMoo.GetMouthPosition();
                            Vector2? positionB = _IDefenceControllableB.IDefencable.GetPosition2();
                            if (positionA != null && positionB != null)
                            {
                                _Moooooosic.SetPositions((Vector2)positionA, (Vector2)positionB);
                            }
                    });
                    Update5Hz = new Action(() => {
                            switch (_State)
                            {
                                case State.MusicFirst:
                                    if (Time.time - _StartStateTime > 1.5)
                                    {
                                        _State = State.HealthFirst;
                                        _StartStateTime = Time.time;
                                    }
                                    break;
                                case State.HealthFirst:
                                    if (_IDefenceControllableB.IDefencable.Health != null)
                                    {
                                        _IDefenceControllableB.IDefencable.Health.Proportion -= GetChangeInHealth();
                                    if (_IDefenceControllableB.IDefencable.Health.Percentage <= 0)
                                        _IDefenceControllableB.IDefencable.Pop(new PopInfo(_IDefenceControllableB.IDefencable) { Vibrate =false});
                                    }
                                    Done = true;
                                    break;
                            }
                        
                    });
                    break;
             default:
                    _ISonicMooControllableB = (ISonicMooControllable)b.IViolence;
                    Update = new Action(() => {
                            Vector2? positionA = _ISonicMooControllableA.ISonicMoo.GetMouthPosition();
                            Vector2? positionB = _ISonicMooControllableB.ISonicMoo.GetMouthPosition();
                            if (positionA != null && positionB != null)
                            {
                                _Moooooosic.SetPositions((Vector2)positionA, (Vector2)positionB);
                            
                            }
                    });
                    Update5Hz = new Action(() => {
                            switch(_State)
                            {
                                case State.MusicFirst:
                                    if(Time.time-_StartStateTime>1.5)
                                    {
                                        _State = State.HealthFirst;
                                        _StartStateTime = Time.time;
                                    }
                                    break;
                                case State.HealthFirst:

                                    break;
                                case State.MusicSecond:

                                    break;
                                default:

                                    break;
                            }
                        
                    });
                    break;
            }
            _StartStateTime = Time.time;
        }
        public void Start()
        {
            Vector2? positionA = _ISonicMooControllableA.ISonicMoo.GetMouthPosition();
            Vector2? positionB = _ISonicMooControllableB!=null? _ISonicMooControllableB.ISonicMoo.GetMouthPosition():_IDefenceControllableB.IDefencable.GetPosition2();
            if (positionA != null && positionB != null)
            {
                _Moooooosic = _IMoosics.Spawn((Vector2)positionA, (Vector2)positionB, 2.5f);
                _ILooper.AddUpdate(this);
                _ILooper.Add5Hz(this);
            }
            else
                Done = true;
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
                float divisor = _ISonicMooControllableB.ISonicMoo.Health.Proportion + _ISonicMooControllableA.ISonicMoo.Health.Proportion;
                return divisor <= 0 ? 0 : _ISonicMooControllableA.ISonicMoo.Health.Proportion / (divisor);
        }
        private float GetChangeInHealth()
        {

            return Random.Range(MIN_CHANGE_HEALTH, MAX_CHANGE_HEALTH);
        }
        public bool LooperUpdate()
        {
            if (!_Paused)
            {
                if (!Done)
                {
                    Update();
                }
            }
            return Done;
        }

        public bool Looper5Hz()
        {
            if (!_Paused)
            {
                if (!Done)
                {
                    Update5Hz();
                }
            }
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

        public static FightControllerUseInfo[] UserInfo {
            get
            {
                return new FightControllerUseInfo[] {  //new FightControllerUseInfo(new Func<FightAction, FightAction, IResourceHelper, IFightController>((a, b, interfacesHelper) => { return new SonicMooController(a, b, interfacesHelper, MooType.Reply); }), typeof(SonicMoo), typeof(SonicMoo)),
               new FightControllerUseInfo(new Func<FightAction, FightAction, IResourceHelper, IFightController>((a, b, interfacesHelper) => { return new SonicMooController(a, b, interfacesHelper, MooType.OneWay); }), typeof(SonicMoo), typeof(Defenceless))
                };
            }
        }
    }
}