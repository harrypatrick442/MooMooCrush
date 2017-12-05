using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    class Score:MonoBehaviour
    {
        private int _Takeouts;
        public int Takeouts
        {
            get
            {
                return _Takeouts;
            }
            set
            {
                _Takeouts = value;
            }
        }
    }
}
