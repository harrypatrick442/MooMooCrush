using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class FightAction
    {
        private IViolence _IViolence;
        public IViolence IViolence
        {
            get
            {
                return _IViolence;
            }
        }
        private Action _CallbackEnded;
        public FightAction(IFighterController iFighterController, IViolence iViolence, Action callbackEnded)
        {
            _IViolence = iViolence;
            _CallbackEnded = callbackEnded;
            iFighterController.TakeFighting(this);
        }
        public void MakeDone()
        {
            _CallbackEnded();
        }
    }
}