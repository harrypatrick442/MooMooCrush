using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class LaseredHandler : MonoBehaviour, ILaseredHandler   
    {
        private ILooper _ILooper;
        private IExploder _IExploder;
        private IBurnHandler _IBurnHandler;
        public LaseredHandler(IBurnHandler iBurnHandler, ILooper iLooper, IExploder iExploder)
        {
            _IBurnHandler = iBurnHandler;
            _ILooper = iLooper;
            _IExploder = iExploder;
        }
        public void Lasered(LaseredInfo laserInfo)
        {
            _IBurnHandler.Burn(laserInfo.ILaserable);
        }
    }
}
