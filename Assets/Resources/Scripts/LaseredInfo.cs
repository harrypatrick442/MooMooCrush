using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
   public class LaseredInfo
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
        private ILaserable _ILaserable;
        public ILaserable ILaserable{
            get{
                return _ILaserable;
            }
        }
        public LaseredInfo(ILaserable iLaserable)
        {
            _ILaserable = iLaserable;
        }
    }
}