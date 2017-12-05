using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public abstract class Attack:IAttack
    {
        public abstract Enums.Attacks AttackType { get; }
        public abstract float Range { get; }
        public Attack()
        {

        }
        public abstract void Launch();
    }
}