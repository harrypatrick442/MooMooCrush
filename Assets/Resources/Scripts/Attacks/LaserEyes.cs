using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class LaserEyes:Attack, IDefence, ILaserEyesControllable
    {
        public override Enums.Attacks AttackType { get { return Enums.Attacks.LaserEyes; } }
        private float _Range=4f;
        public override  float Range { get { return _Range; } }
        public ILaserEyes ILaserEyes { get {
                return _ILaserEyes;
            } }

        public Enums.Attacks[] AttackTypes
        {
            get
            {
                return new Enums.Attacks[] { Enums.Attacks.LaserEyes };
            }
        }

        private ILaserEyes _ILaserEyes;
        public LaserEyes(float range, ILaserEyes iLaserEyes)
        {
            _Range = range;
            _ILaserEyes = iLaserEyes;
        }
        public override void Launch()
        {

        }
    }
}