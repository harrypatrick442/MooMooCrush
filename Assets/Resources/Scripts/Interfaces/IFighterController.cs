using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IFighterController
    {
        bool IsBeingAttacked { get; }
        void TakeFighting(FightAction owner);
        void ReleaseFighting(FightAction owner);
        FightAction Attack(Enums.Attacks attack);
        void TakeCurrentlyEngaging(IFightable currentlyEngaging);
        void ReleaseCurrentlyEngaging();
        bool IsActing { get; }
        IFightable CurrentlyEngaging { get; }
    }
}
