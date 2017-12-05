using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
    public class Vibrator : IVibrator, ILooperFixedUpdate
    {
        private ILooper _ILooper;
        private class Vibration
        {
            public float StartTime;
            private Vibratable _Vibratable;
            private Vector2 _StartPosition;
            private float _AngleMultiplier;
            private float _DAmplitude;
            public Vibration(Vibratable vibratable, Vector2 startPosition)
            {
                StartTime = Time.time;
                _StartPosition = startPosition;
                _Vibratable = vibratable;
                _DAmplitude = vibratable.EndAmplitude - vibratable.StartAmplitude;
                _AngleMultiplier = vibratable.Frequency * (Mathf.PI * 2);
            }
            public bool Update(float time)
            {
                float duration = time - StartTime;
                if (duration> _Vibratable.Duration)
                {
                    Vector2? q = _Vibratable.IVibratable.GetPosition2();
                    if (q != null)
                        _Vibratable.IVibratable.SetPosition(new Vector2(((Vector2)q).x, _StartPosition.y));
    
                    if (_Vibratable.Callback != null)
                        _Vibratable.Callback();
                    return true;
                }
                float amplitude = (_DAmplitude * duration / _Vibratable.Duration) + _Vibratable.StartAmplitude;
                float angle = _AngleMultiplier * duration;
                Vector2? p = _Vibratable.IVibratable.GetPosition2();
                if(p!=null)
                    _Vibratable.IVibratable.SetPosition(new Vector2(((Vector2)p).x, _StartPosition.y)+new Vector2(0, amplitude * Mathf.Sin(angle)));
                return false;
            }
        }
        private List<Vibration> _Vibrations = new List<Vibration>();
        public void Vibrate(Vibratable vibratable)
        {
            Vector2? startPosition = vibratable.IVibratable.GetPosition2();
            if (startPosition != null)
            {
                _Vibrations.Add(new Scripts.Vibrator.Vibration(vibratable, (Vector2)startPosition));
                _ILooper.AddFixedUpdate(this);
            }
        }
        public Vibrator(ILooper iLooper)
        {
            _ILooper = iLooper;
        }
        public bool LooperFixedUpdate()
        {
            int i = 0;
            int count = _Vibrations.Count;
            while (i < count)
            {
                Vibration vibration = _Vibrations[i];
                if (!vibration.Update(Time.time))
                {
                    i++;
                }
                else
                {
                    count--;
                    _Vibrations.Remove(vibration);
                }
            }
            return _Vibrations.Count < 1;
        }
    }
}