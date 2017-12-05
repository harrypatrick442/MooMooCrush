using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class ExplosionInfo
    {
        public virtual bool IsPhysical { get { return true; } }
        public virtual float? Radius { get { return null; } }
        public virtual string Prefab { get { return "Prefabs/explosion"; } }
    }
}
