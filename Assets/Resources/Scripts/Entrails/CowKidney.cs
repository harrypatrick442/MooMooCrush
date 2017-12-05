using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class CowKidney : EntrailsItem
    {
        private string[] _Prefabs = new string[] { "Prefabs/Entrails/kidney_0", "Prefabs/Entrails/kidney_1", "Prefabs/Entrails/kidney_2" };
        public override string[] Prefabs { get { return _Prefabs; } }
        public CowKidney():base()
        {

        }
    }
}
