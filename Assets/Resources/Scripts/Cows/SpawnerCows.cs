using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
namespace Assets.Scripts
{
    public class SpawnerCows : IPausible, IDifficultyable
    {
        private Cows Cows;
        private const float POSITION_Z = -0.5f;
        private const int MAX_COWS = 25;
        private List<float> _PauseDelays = new List<float>();
        private bool _Paused = false;
        private float _PausedTime;
        private ManualResetEvent _ManualResetEvent = new ManualResetEvent(true);
        private Enums.FoodType[] _FoodTypes = { Enums.FoodType.Strawberry, Enums.FoodType.Banana, Enums.FoodType.Chocolate, Enums.FoodType.Burger };
        private RouletteWheel<Enums.FoodType> _RouletteWheelNormalFoodType = new RouletteWheel<Enums.FoodType>(new RouletteSlot<Enums.FoodType>[]{
            new RouletteSlot<Enums.FoodType>(25f,Enums.FoodType.Strawberry),
            new RouletteSlot<Enums.FoodType>(25f,Enums.FoodType.Banana),
            new RouletteSlot<Enums.FoodType>(25f, Enums.FoodType.Chocolate),
            new RouletteSlot<Enums.FoodType>(25f, Enums.FoodType.Burger)
            });
        private bool _DevilCowWaitingForSuperMoo = false;
        private RouletteWheel<Enums.CowType> _RouletteWheelNormalCowType;
        private RouletteSlot<Enums.CowType> _RouletteSlotSuperMoo = new RouletteSlot<Enums.CowType>(50f, Enums.CowType.SuperMoo);
        private RouletteWheel<Enums.CowType> _RouletteWheelReligiousCowType;
        private RouletteWheel<Enums.Health> _RouletteWheelNormalHealth = new RouletteWheel<Enums.Health>(new RouletteSlot<Enums.Health>[]{
            new RouletteSlot<Enums.Health>(80f, Enums.Health.Well),
            new RouletteSlot<Enums.Health>(20f, Enums.Health.Unwell)
        });
        private RouletteWheel<Enums.Health> _RouletteWheelSickHealth = new RouletteWheel<Enums.Health>(new RouletteSlot<Enums.Health>[]{
            new RouletteSlot<Enums.Health>(20f, Enums.Health.Well),
            new RouletteSlot<Enums.Health>(80f, Enums.Health.Unwell)
        });
        private RouletteWheel<Vector2> _RouletteWheelSuperMooSpawnSite;
        private RouletteWheel<Vector2> _RouletteWheelDevilCowSpawnSite;
        private RouletteWheel<Enums.ExplosiveType> _RouletteWheelTerrorExplosiveType = new RouletteWheel<Enums.ExplosiveType>(new RouletteSlot<Enums.ExplosiveType>[]{
            new RouletteSlot<Enums.ExplosiveType>(20f, Enums.ExplosiveType.TNT),
            new RouletteSlot<Enums.ExplosiveType>(80f, Enums.ExplosiveType.C4)
        });
        private RouletteWheel<Vector3> _RouletteWheelSpawnSite = new RouletteWheel<Vector3>(new RouletteSlot<Vector3>[] { new RouletteSlot<Vector3>(25f, new Vector3(-3.38f, 1.31f, POSITION_Z)), new RouletteSlot<Vector3>(25f, new Vector3(3.38f, 1.31f, POSITION_Z)), new RouletteSlot<Vector3>(25f, new Vector3(-3.38f, 0.56f, POSITION_Z)), new RouletteSlot<Vector3>(25f, new Vector3(3.38f, 0.56f, POSITION_Z)) });
        private RouletteWheel<Enums.CowType> _CurrentRouletteWheelCowType;
        private RouletteWheel<Enums.FoodType> _CurrentRouletteWheelFoodType;
        private RouletteWheel<Enums.Health> _CurrentRouletteWheelHealth;
        private bool _Run = true;
        private Enums.SpawnerMode _CurrentMode = Enums.SpawnerMode.Normal;
        private StopWatch _StopWatch = new StopWatch();
        private int _MinDelay = 300;
        private int MinDelay
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
        private int _MaxDelay = 1000;
        private int MaxDelay
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
        public void SetDifficulty(int difficulty )
        {
            switch (difficulty)
            {
                case Difficulty.EASY0:
                    MinDelay = 3000;
                    MaxDelay = 4000;
                    break;
                case Difficulty.EASY1:
                    MinDelay = 2500;
                    MaxDelay = 3500;
                    break;
                case Difficulty.EASY2:
                    MinDelay = 2200;
                    MaxDelay = 3300;
                    break;
                case Difficulty.MEDIUM0:
                    MinDelay = 1800;
                    MaxDelay = 3100;
                    break;
                case Difficulty.MEDIUM1:
                    MinDelay = 1600;
                    MaxDelay = 3000;
                    break;
                case Difficulty.MEDIUM2:
                    MinDelay = 1400;
                    MaxDelay = 2500;
                    break;
                case Difficulty.HARD0:
                    MinDelay = 1100;
                    MaxDelay = 2000;
                    break;
                case Difficulty.HARD1:
                    MinDelay = 800;
                    MaxDelay = 1000;
                    break;
                default:
                    break;
            }
        }
        private Thread _Thread;
        private System.Random _Random = new System.Random();
        public SpawnerCows(Cows cows, Vector2[] superMooSpawnSites, Vector2[] devilCowSpawnSites)
        {
            Cows = cows;
            InitializeCowTypes();
            InitializeSpawnSites(superMooSpawnSites, devilCowSpawnSites);
            _CurrentRouletteWheelCowType = _RouletteWheelNormalCowType;
            _CurrentRouletteWheelFoodType = _RouletteWheelNormalFoodType;
            _CurrentRouletteWheelHealth = _RouletteWheelNormalHealth;
            _Thread = new Thread(Run);
            _Thread.Start();
        }
        private void InitializeCowTypes()
        {
            _RouletteWheelNormalCowType = new RouletteWheel<Enums.CowType>(new RouletteSlot<Enums.CowType>[]{
            new RouletteSlot<Enums.CowType>(5f, Enums.CowType.Devil),
            _RouletteSlotSuperMoo,
            new RouletteSlot<Enums.CowType>(5f, Enums.CowType.Religious),
            new RouletteSlot<Enums.CowType>(10f, Enums.CowType.Terror),
            new RouletteSlot<Enums.CowType>(100f, Enums.CowType.Edible)
        });
            _RouletteWheelReligiousCowType = new RouletteWheel<Enums.CowType>(new RouletteSlot<Enums.CowType>[]{
            new RouletteSlot<Enums.CowType>(1f, Enums.CowType.Devil),
            _RouletteSlotSuperMoo,
            new RouletteSlot<Enums.CowType>(5f, Enums.CowType.Terror),
            new RouletteSlot<Enums.CowType>(95f, Enums.CowType.Religious),
            new RouletteSlot<Enums.CowType>(100f, Enums.CowType.Edible)
        });
        }
        private void InitializeSpawnSites(Vector2[] superMooSpawnSites, Vector2[] devilCowSpawnSites)
        {
            int count = superMooSpawnSites.Count();
            RouletteSlot<Vector2>[] rouletteSlots = new RouletteSlot<Vector2>[count];
            float probability = 100f / count;
            int i = 0;
            foreach (Vector2 site in superMooSpawnSites)
            {
                rouletteSlots[i] = new RouletteSlot<Vector2>(probability, site);
                i++;
            }
            _RouletteWheelSuperMooSpawnSite = new RouletteWheel<Vector2>(rouletteSlots);
            rouletteSlots = new RouletteSlot<Vector2>[devilCowSpawnSites.Count()];
            i = 0;
            count = devilCowSpawnSites.Count();
            probability = 100f / count;
            foreach (Vector2 site in devilCowSpawnSites)
            {
                rouletteSlots[i] = new RouletteSlot<Vector2>(probability, site);
                i++;
            }
            _RouletteWheelDevilCowSpawnSite = new RouletteWheel<Vector2>(rouletteSlots);
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
                case Enums.SpawnerMode.Religious:
                    if (_StopWatch.GetS() > 10000)
                    {
                        _CurrentMode = Enums.SpawnerMode.Normal;
                        _CurrentRouletteWheelCowType = _RouletteWheelNormalCowType;
                    }
                    break;
                case Enums.SpawnerMode.Sick:
                    if (_StopWatch.GetS() > 10000)
                    {
                        _CurrentMode = Enums.SpawnerMode.Normal;
                        _CurrentRouletteWheelHealth = _RouletteWheelNormalHealth;
                    }
                    break;
            }
        }
        private void Sick()
        {
            _StopWatch.Reset();
            _CurrentRouletteWheelHealth = _RouletteWheelSickHealth;
            _CurrentMode = Enums.SpawnerMode.Sick;
        }
        private void Religious()
        {
            _StopWatch.Reset();
            _CurrentRouletteWheelCowType = _RouletteWheelReligiousCowType;
            _CurrentMode = Enums.SpawnerMode.Religious;
        }
        private Vector3 GetSpawnSite()
        {
            return _RouletteWheelSpawnSite.Spin();
        }
        private Vector2 GetSuperMooSpawnSite()
        {
            Vector2 a = _RouletteWheelSuperMooSpawnSite.Spin();
            return new Vector3(a.x, a.y, POSITION_Z);
        }
        private void Spawn()
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (Cows.Count < MAX_COWS)
                {
                    CowInfo cowInfo = new Scripts.CowInfo();
                    cowInfo.CowType = _CurrentRouletteWheelCowType.Spin();
                    Vector3 spawnSite;
                    switch (cowInfo.CowType)
                    {
                        case Enums.CowType.Edible:
                            Enums.Health health = _CurrentRouletteWheelHealth.Spin();
                            cowInfo.Health = health;
                            spawnSite = GetSpawnSite();
                            cowInfo.FoodType = _CurrentRouletteWheelFoodType.Spin();
                            break;
                        case Enums.CowType.Terror:
                            cowInfo.FoodType = Enums.FoodType.Inedible;
                            cowInfo.ExplositeType = _RouletteWheelTerrorExplosiveType.Spin();
                            spawnSite = GetSpawnSite();
                            break;
                        case Enums.CowType.Devil:
                            _DevilCowWaitingForSuperMoo = true;
                            spawnSite = _RouletteWheelDevilCowSpawnSite.Spin();
                            cowInfo.FoodType = Enums.FoodType.Inedible;
                            break;
                        case Enums.CowType.SuperMoo:
                            _DevilCowWaitingForSuperMoo = false;
                            _RouletteSlotSuperMoo.Probability = 1;
                            spawnSite = GetSuperMooSpawnSite();
                            cowInfo.FoodType = Enums.FoodType.Inedible;
                            break;
                        default:
                            spawnSite = GetSpawnSite();
                            cowInfo.FoodType = Enums.FoodType.Inedible;
                            break;

                    }
                    if (_DevilCowWaitingForSuperMoo)
                    {
                        if (_RouletteSlotSuperMoo.Probability < 90)
                        {
                            _RouletteSlotSuperMoo.Probability += 90;
                        }
                    }
                    Cows.Spawn(spawnSite, cowInfo);
                }
            });

        }
        public void Pause()
        {
            if (!_Paused)
            {
                _ManualResetEvent.Reset();
                _Paused = true;
                _PausedTime = Time.time;
            }
        }
        public void Unpause()
        {
            if (_Paused)
            {
                _Paused = false;
                _ManualResetEvent.Set();
            }
        }
        private void Run()
        {
            while (_Run)
            {
                UpdateMode();
                Spawn();
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

            }
        }
    }
}
