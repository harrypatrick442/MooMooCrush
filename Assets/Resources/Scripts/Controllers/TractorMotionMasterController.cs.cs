using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    class TractorMotionMasterController

        : ITractorMotionMasterController, ILooperFixedUpdate
    {

        private ILooper _ILooper;
        private ITractor _ITractor;
        public ILooper ILooper
        {
            get
            {
                return _ILooper;
            }
            set
            {
                if (value != null)
                {
                    _ILooper = value;
                }
            }
        }
        private bool _Paused = false;
        public TractorMotionMasterController(ITractor iTractor)
        {
            _ITractor = iTractor;
        }
        public void TractablesChanged()
        {
            EventDictionary<ITractable, TractoringInfo> interractions = _ITractor.GetInterractions();
            if(interractions.Count>0)
                if(_ILooper!=null)
                    _ILooper.AddFixedUpdate(this);
        }
        public bool LooperFixedUpdate()
        {
            EventDictionary<ITractable, TractoringInfo> interractions = _ITractor.GetInterractions();
            return true;// interractions.Count <= 0;
        }
        public void Pause()
        {
            if (!_Paused)
            {
            }
            _Paused = true;
        }
        public void Unpause()
        {
            if (_Paused)
            {
            }
            _Paused = false;
        }
        public void Dispose()
        {
            
        }

        public Transform GetRecipricolTransform()
        {
            return _ITractor.GetRecipricolTransform();
        }
    }
}
