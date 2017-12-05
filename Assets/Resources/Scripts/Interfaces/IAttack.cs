using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IAttack:IViolence
    {
        Enums.Attacks AttackType { get; }
        float Range { get; }
    }
}