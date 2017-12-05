using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Page:  MonoBehaviour
    {
        private float? _StartAngle;
        public float StartAngle
        {
            get
            {
                if (_StartAngle == null)
                    _StartAngle = Angle;
                return (float)_StartAngle;
            }
            set
            {
                _StartAngle = value;
            }
        }
        public bool  _DoneAngle = false;
        public bool DoneAngle
        {
            get
            {
                return _DoneAngle;
            }
            set
            {
                if(!value)
                {
                    _StartAngle = Angle;
                }
                _DoneAngle = value;
            }
        }
        public float Angle
        {
            set
            {
                gameObject.transform.localEulerAngles = new Vector3(0, value, 0);
            }
            get
            {
                return gameObject.transform.localEulerAngles.y;
            }
        }
        public enum States { LHS, Changing, RHS };
        public States State
        {
            get
            {
                return Angle >180 ? States.LHS : (Angle < 0 ? States.RHS : States.Changing);
            }
        }
        public void HideContent()
        {
            gameObject.SetActive(false);
        }
        public void ShowContent()
        {
            gameObject.SetActive(true);
        }
    }
}
