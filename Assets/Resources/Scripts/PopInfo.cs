using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
   public class PopInfo
    {
        private bool _IsDone = false;
        public bool IsDone
        {
            get
            {
                return _IsDone;
            }
            set
            {
                if (value)
                    _IsDone = value;
            }
        }
        private bool _Vibrate = false;
        public bool Vibrate
        {
            get
            {
                return _Vibrate;
            }
            set
            {
                    _Vibrate = value;
            }
        }
        private IPopable _IPopable;
        public IPopable IPopable
        {
            get{
                return _IPopable;
            }
        }
        public PopInfo(IPopable iPopable)
        {
            _IPopable = iPopable;
        }
    }
}