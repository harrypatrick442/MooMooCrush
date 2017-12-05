using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class CowGuts:EntrailsItem
    {
        private string[] _Prefabs = new string[] { "Prefabs/Entrails/guts_0", "Prefabs/Entrails/guts_1", "Prefabs/Entrails/guts_2", "Prefabs/Entrails/guts_3", "Prefabs/Entrails/guts_4", "Prefabs/Entrails/guts_5" };
        public override string[] Prefabs { get { return _Prefabs; } }
        public CowGuts():base()
        {

        }
    }
}
