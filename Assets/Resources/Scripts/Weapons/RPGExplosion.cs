using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class RPGExplosion : ExplosionInfo
    {
        public override bool IsPhysical { get { return false; } }
        public override float? Radius{get{return 0.7f;}}
        public override string Prefab { get { return "Prefabs/rpg_explosion"; } }
    }
}
