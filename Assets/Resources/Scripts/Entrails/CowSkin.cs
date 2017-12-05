using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class CowSkin : EntrailsItem
    {
        private string[] _Prefabs = new string[] { "Prefabs/Entrails/skin_0", "Prefabs/Entrails/skin_1", "Prefabs/Entrails/skin_2" };
        public override string[] Prefabs { get { return _Prefabs; } }
        public CowSkin():base()
        {

        }
    }
}
