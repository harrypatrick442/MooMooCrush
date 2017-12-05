using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class SonicMoo: Attack, IDefenceControllable, ISonicMooControllable
    {
        public override Enums.Attacks AttackType { get { return Enums.Attacks.SonicMoo; } }
        private float _Range=4f;
        public override  float Range { get { return _Range; } }
        public ISonicMoo ISonicMoo
        { get {
                return _ISonicMoo;
            } }

        public Enums.Attacks[] AttackTypes
        {
            get
            {
                return new Enums.Attacks[] { Enums.Attacks.SonicMoo };
            }
        }

        public IDefencable IDefencable
        {
            get
            {
                return _IDefencable;
            }
        }

        private ISonicMoo _ISonicMoo;
        private IDefencable _IDefencable;
        public SonicMoo(float range, ISonicMoo iSonicMoo, IDefencable iDefencable)
        {
            _Range = range;
            _ISonicMoo = (ISonicMoo)iSonicMoo;
            _IDefencable = (IDefencable)iDefencable;
        }
        public override void Launch()
        {

        }
    }
}