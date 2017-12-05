using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class CowHeart : EntrailsItem
    {
        private string[] _Prefabs = new string[]{ "Prefabs/Entrails/heart"};
        public override string[] Prefabs { get { return _Prefabs; } }
        public CowHeart():base()
        {

        }
    }
}
