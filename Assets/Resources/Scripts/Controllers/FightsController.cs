using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace Assets.Scripts
{
    public class FightsController: IFightsController, ILooper2Hz, IPausible
    {
        private ControllerPicker _ControllerPicker;
        private ILooper _ILooper;
        private class FightInstance :IDisposable, IPausible{
            private FightAction _fightActionA;
            private FightAction _fightActionB;
            private IFightController _IFightController;
            public FightInstance(IFightController iFightController, FightAction fightActionA, FightAction fightActionB)
            {
                _IFightController = iFightController;
                _fightActionA = fightActionA;
                _fightActionB = fightActionB;
                iFightController.Start();
            }

            public void Dispose()
            {
                _fightActionA.MakeDone();
                _fightActionB.MakeDone();
                _IFightController.Stop();
            }

            public void Pause()
            {
                _IFightController.Pause();
            }

            public bool Tick()
            {
                _IFightController.Tick();
                return _IFightController.IsDone;
            }

            public void Unpause()
            {
                _IFightController.Unpause();
            }
        }
        private List<FightInstance> _Fights = new List<FightInstance>();
        public FightsController(ControllerPicker controllerPicker, ILooper iLooper)
        {
            _ControllerPicker = controllerPicker;
            _ILooper = iLooper;
        }
        public bool Looper2Hz()
        {
            List<FightInstance> toRemove = new List<FightInstance>();
            foreach(FightInstance fightInstance in _Fights)
            {
                if(fightInstance.Tick())
                {
                    toRemove.Add(fightInstance);
                }
            }
            foreach (FightInstance fightInstance in toRemove)
            {
                fightInstance.Dispose();
                _Fights.Remove(fightInstance);
            }
            return _Fights.Count<1;
        }
        public void Fight(FightAction fightActionA, FightAction fightActionB, IResourceHelper interfacesHelper)
        {
            IFightController establishedController = _ControllerPicker.Pick(fightActionA, fightActionB);
            FightInstance fightInstance = new FightInstance(establishedController, fightActionA, fightActionB);
            _Fights.Add(fightInstance); 
            _ILooper.Add2Hz(this);
        }

        public void Pause()
        {
            foreach(IPausible iPausible in _Fights)
            {
                iPausible.Pause();
            }
        }

        public void Unpause()
        {
            foreach (IPausible iPausible in _Fights)
            {
                iPausible.Unpause();
            }
        }
    }
}