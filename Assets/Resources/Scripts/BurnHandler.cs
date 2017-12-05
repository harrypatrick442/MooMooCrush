using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BurnHandler : ILooperFixedUpdate, IBurnHandler
    {
        private ILooper _ILooper;
        private class BurnInfo
        {
            public float StartTime;
            public enum Modes { Start, Shooting, Done }
            public Modes Mode = Modes.Start;
             public  IBurnable IBurnable;
            public BurnInfo(IBurnable iBurnable)
            {
                IBurnable = iBurnable;
            }
            public void Burn()
            {
                IBurnable.OnSlaughterStart();
                IBurnable.Burn();
            }
            public void DoneBurn()
            {
                IBurnable.DoneBurn();
            }
            public bool IsBurnt()
            {
                return IBurnable.IsBurnt();
            }
        }
        private List<BurnInfo> _BurnInfos = new List<BurnInfo>();
        private bool _Handled = false;
        public BurnHandler(ILooper iLooper)
        {
            _ILooper = iLooper;
        }
        public bool LooperFixedUpdate()
        {
            int i = 0;
            int count = _BurnInfos.Count;
            while (i < count)
            {
                BurnInfo burnInfo = _BurnInfos[i];
                switch (burnInfo.Mode)
                {
                    case BurnInfo.Modes.Start:
                        burnInfo.Burn();
                        burnInfo.StartTime = Time.time;
                        burnInfo.Mode = BurnInfo.Modes.Shooting;
                        i++;
                        break;
                    case BurnInfo.Modes.Shooting:
                        if (burnInfo.IsBurnt() || (Time.time - burnInfo.StartTime > 3))
                        {
                            burnInfo.DoneBurn();
                            _BurnInfos.Remove(burnInfo);
                            count--;
                        }
                        else
                            i++;
                        break;
                    default:
                        i++;
                        break;
                }
            }
            return _BurnInfos.Count<1;
        }
        public void Burn(IBurnable iBurnable)
        {
            _BurnInfos.Add(new BurnInfo(iBurnable));
            _ILooper.AddFixedUpdate(this);
        }
    }
}
