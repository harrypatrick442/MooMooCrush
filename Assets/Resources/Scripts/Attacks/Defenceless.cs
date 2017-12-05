using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class Defenceless: IDefence, IViolence,  IDefenceControllable
    {
        public Enums.Attacks[] AttackTypes
        {
            get
            {
                return new Enums.Attacks[] { Enums.Attacks.SonicMoo };
            }
        }
        private IDefencable _IDefencable;
        public IDefencable IDefencable
        {
            get
            {
                return _IDefencable;
            }
        }

        public void Launch()
        {

        }
        public Defenceless(IDefencable iDefencable)
        {
            _IDefencable = (IDefencable)iDefencable;
        }
    }
}