using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
    public delegate void HealthEventHandler(HealthEventArgs e);
    public class HealthEventArgs : EventArgs {
        public enum Types { Purged, Restored, Changed}
        private Types _Type;
        public Types Type{
            get{
                return _Type;
            }
            }
        public HealthEventArgs(Types type)
        {
            _Type = type;
        }
    }
    public class Health
    {
        public void AddEventHandler(HealthEventHandler h)
        {
            _Events.Add(h);
        }
        public void RemoveEventHandler(HealthEventHandler h)
        {
            _Events.Remove(h);
        }
        private WeakCollection<HealthEventHandler> _Events = new WeakCollection<HealthEventHandler>();
        private float _Proportion = 1f;
        public float Proportion
        {
            get
            {
                return _Proportion;
            }
            set
            {
                if (value >= 1)
                {
                    _Proportion = 1;
                    FireRestoredEvent();
                }
                else
                    if (value <= 0)
                    {

                        _Proportion = 0;
                        FirePurgedEvent();
                    }
                    else
                    {
                        _Proportion = value;
                        FireChangedEvent();
                    }
            }
        }
        private void FireRestoredEvent()
        {
            HealthEventArgs e = new HealthEventArgs(HealthEventArgs.Types.Restored);
            foreach(HealthEventHandler h in _Events)
            {
                h(e);
            }
        }
        private void FirePurgedEvent()
        {
            HealthEventArgs e = new HealthEventArgs(HealthEventArgs.Types.Purged);
            foreach (HealthEventHandler h in _Events)
            {
                h(e);
            }
        }
        private void FireChangedEvent()
        {
            HealthEventArgs e = new HealthEventArgs(HealthEventArgs.Types.Changed);
            foreach (HealthEventHandler h in _Events)
            {
                h(e);
            }
        }
        public float Percentage
        {
            get
            {
                return 100 * Proportion;
            }
            set
            {
                Proportion = value / 100;
            }
        }
        public void Purge()
        {
            Proportion = 0;
        }
        public void Restore()
        {
            Proportion = 1;
        }
        public Health()
        {

        }
    }
}