using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class CowEntrails:Entrails
    {
        public override int MinNComponents { get { return 6; } }
        public override int MaxNComponents { get { return 9; } }
        public override int Duration { get { return 2; } }
        private static List<EntrailsComponent> _EntrailsComponents = new List<EntrailsComponent>()
        {
            new EntrailsComponent(typeof(CowGuts), 3),
            new EntrailsComponent(typeof(CowHeart), 1),
            new EntrailsComponent(typeof(CowLiver), 1),
            new EntrailsComponent(typeof(CowKidney), 2),
            new EntrailsComponent(typeof(CowSkin), 3),
            new EntrailsComponent(typeof(CowLeg), 2)
        };
        public override List<EntrailsComponent> EntrailsComponents
        {
            get{ return _EntrailsComponents; }
        }
        public CowEntrails():base()
        {
        }
    }

}