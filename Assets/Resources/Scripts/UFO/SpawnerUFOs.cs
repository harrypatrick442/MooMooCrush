using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    class SpawnerUFOs: IPausible
    {
        private UFOs UFOs;
        private const float POSITION_Z = -0.5f;
        private const int MAX_UFOS = 0;
        private List<float> _PauseDelays = new List<float>();
        private bool _Paused = false;
        private float _PausedTime;
        private ManualResetEvent _ManualResetEvent = new ManualResetEvent(true);
        private RouletteWheel<Vector3> _RouletteWheelSpawnSite;
        private bool _Run = true;
        private Enums.SpawnerMode _CurrentMode = Enums.SpawnerMode.Normal;
        private StopWatch _StopWatch = new StopWatch();
        private int _MinDelay = 10000;
        public int MinDelay
        {
            get
            {
                return _MinDelay;
            }
            set
            {
                _MinDelay = value;
            }
        }
        private int _MaxDelay = 40000;
        public int MaxDelay
        {
            get
            {
                return _MaxDelay;
            }
            set
            {
                _MaxDelay = value;
            }
        }
        private Thread _Thread;
        private System.Random _Random = new System.Random();
        public SpawnerUFOs(UFOs ufos)
        {
            List<RouletteSlot<Vector3>> rouletteSlots = new List<RouletteSlot<Vector3>>();
            UFOSpawnSite[] ufoSpawnSites = ufos.SpawnSites;
            float ufoSpawnSiteProbability = 1 / ufoSpawnSites.Length;
            foreach (UFOSpawnSite ufoSpawnSite in ufoSpawnSites)
            {
                rouletteSlots.Add(new RouletteSlot<Vector3>(ufoSpawnSiteProbability, ufoSpawnSite.Position));
            }
            _RouletteWheelSpawnSite = new RouletteWheel<Vector3>(rouletteSlots.ToArray());
            UFOs = ufos;
            Pause();
            _Thread = new Thread(Run);
            _Thread.Start();
        }
        private int GetDelay()
        {
            return _Random.Next(MinDelay, MaxDelay);
        }
        private void UpdateMode()
        {
            switch (_CurrentMode)
            {
                case Enums.SpawnerMode.Normal:
                    break;
            }
        }
        private Vector3 GetSpawnSite()
        {
            return _RouletteWheelSpawnSite.Spin();
        }
        private void Spawn()
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (UFOs.Count < MAX_UFOS)
                {
                    UFOs.Spawn(new UFOInfo(GetSpawnSite()));
                }
            });

        }
        public void Pause()
        {
            if (!_Paused)
            {
                _ManualResetEvent.Reset();
                _PausedTime = Time.time;
            }
            _Paused = true;
        }
        public void Unpause()
        {
            if (_Paused)
            {
                _ManualResetEvent.Set();
            }
            _Paused = false;
        }
        private void Run()
        {
            while (_Run)
            {
                Thread.Sleep(GetDelay());
                if (_Paused)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        _PauseDelays.Add(Time.time - _PausedTime);
                    });
                    _ManualResetEvent.WaitOne();
                }
                if (_PauseDelays.Count > 0)
                {
                    float total = 0;
                    foreach (float pauseDelay in _PauseDelays)
                    {
                        total += pauseDelay;
                    }
                    _PauseDelays.Clear();
                    Debug.Log((int)(1000 * total));
                    Thread.Sleep((int)(1000 * total));
                }
                UpdateMode();
                Spawn();
            }
        }
    }
}
