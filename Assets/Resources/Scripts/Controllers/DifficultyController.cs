using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

namespace Assets.Scripts
{
    public class DifficultyController : ILooper1Hz
    {
        private List<IDifficultyable> list = new List<IDifficultyable>();
        private int difficulty = Difficulty.EASY0;
        private const int MAX = 8;
        private ILooper _ILooper;
        private bool _Done = false;
        private float _CurrentDelay = 30000;
        private float[] _Delays = { 30000, 30000, 30000, 30000, 30000, 30000, 30000, 30000, };
        private StopWatch _StopWatch = new StopWatch();
        public DifficultyController(ILooper iLooper)
        {
            iLooper.Add1Hz(this);
        }
        private void Next()
        {
            if (difficulty < MAX)
                difficulty++;
            else
                _Done = true;
            _StopWatch.Reset();
            _CurrentDelay = difficulty>=_Delays.Length?_Delays[_Delays.Length-1]:_Delays[difficulty];
            foreach (IDifficultyable iDifficultyable in list)
            {
                iDifficultyable.SetDifficulty(difficulty);
            }
        }
        public void Add(IDifficultyable iDifficultyable)
        {
            if (!list.Contains(iDifficultyable))
            {
                list.Add(iDifficultyable);
                iDifficultyable.SetDifficulty(difficulty);
            }
        }
        public void AddRange(params IDifficultyable[] iDifficultyables)
        {
            foreach (IDifficultyable iDifficultyable in iDifficultyables)
                if (!list.Contains(iDifficultyable))
                {
                    list.Add(iDifficultyable);
                    iDifficultyable.SetDifficulty(difficulty);
                }
        }
        public bool Looper1Hz()
        {
            if (_StopWatch.GetMilliseconds() > _CurrentDelay)
            {
                Next();
            }
            return _Done;
        }
    }
}