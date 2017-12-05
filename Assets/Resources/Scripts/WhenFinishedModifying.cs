using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.ComponentModel;
using System.Threading;
namespace Assets.Scripts
{
    class WhenFinishedModifying
    {
        public delegate void DelegateFinished();
        private DelegateFinished delegateFinished;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private volatile Boolean _reset = true;
        private int delay = 500;
        public WhenFinishedModifying(DelegateFinished delegateFinished)
        {
            this.delegateFinished = delegateFinished;
            backgroundWorker.DoWork += doWork;
        }
        public WhenFinishedModifying(DelegateFinished delegateFinished, int delay)
        {
            this.delay = delay;
            this.delegateFinished = delegateFinished;
            backgroundWorker.DoWork += doWork;
        }
        public void Reset()
        {
            _reset = true;
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }
        private void doWork(object o, EventArgs e)
        {
            while (_reset)
            {
                _reset = false;
                Thread.Sleep(delay);
            }
            this.delegateFinished();
        }
    }
}