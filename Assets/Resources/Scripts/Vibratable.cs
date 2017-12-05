using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
    public class Vibratable
    {
        private IVibratable _IVibratable;
        public IVibratable IVibratable
        {
            get
            {
                return _IVibratable;
            }
        }
        private float _Frequency = 10;
        public float Frequency
        {
            get
            {
                return _Frequency;
            }
        }
        private float _Duration = 1000;
        public float Duration
        {
            get
            {
                return _Duration;
            }
        }
        private Action _Callback;
        public Action Callback
        {
            get
            {
                return _Callback;
            }
            set
            {
                _Callback = value;
            }
        }
        public Vibratable(IVibratable iVibratable, float frequency, float duration)
        {
            _IVibratable = iVibratable;
            _Frequency = frequency;
            _Duration = duration;
        }
        private float _StartAmplitude = 0.05f;
        public float StartAmplitude
        {
            get
            {
                return _StartAmplitude;
            }
            set
            {
                _StartAmplitude = value;
            }
        }
        private float? _EndAmplitude;
        public float EndAmplitude
        {
            get
            {
                if (_EndAmplitude == null)
                    _EndAmplitude = StartAmplitude;
                return (float)_EndAmplitude;
            }
            set
            {
                _EndAmplitude = value;
            }
        }
    }
}