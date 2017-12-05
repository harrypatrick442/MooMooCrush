using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IFightsController:IPausible
    {
        void Fight(FightAction fightActionA, FightAction fightActionB, IResourceHelper interfacesHelper);
    }
}
