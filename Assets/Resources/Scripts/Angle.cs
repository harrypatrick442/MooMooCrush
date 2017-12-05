using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class Angle
    {
        private float _Radians;
        private float? _Degrees;
        public float Degrees
        {
            get
            {
                if (_Degrees == null) _Degrees = _Radians * 180 / Mathf.PI;
                return (float)_Degrees;
            }
            set
            {
                _Degrees = null;
                _Radians = ClampWithin2Pi(value * Mathf.PI / 180);
            }
        }
        public float Radians
        {
            get
            {
                return _Radians;
            }
            set
            {
                _Radians = ClampWithin2Pi(value);
                _Degrees = null;
            }
        }
        public Angle(float radians)
        {
            _Radians = ClampWithin2Pi(radians);
        }
        private static float ClampWithin2Pi(float radians)
        {
            float twoPi = 2 * Mathf.PI;
            while(radians>=twoPi)
            {
                radians = radians - twoPi;
            }
            while(radians<0)
            {
                radians = radians + twoPi;
            }
            return radians;
        }
        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(ClampWithin2Pi(left.Radians + right.Radians));
        }
        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(ClampWithin2Pi(right.Radians - left.Radians));
        }
    }
}
