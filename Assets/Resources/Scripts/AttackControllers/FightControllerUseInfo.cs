using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
namespace Assets.Scripts
{
    public class FightControllerUseInfo
    {
        private Func<FightAction, FightAction, IResourceHelper, IFightController> _GetInstance;
        private Type _IViolenceTypeA;
        private Type _IViolenceTypeB;
        public Func<FightAction, FightAction, IResourceHelper, IFightController> GetInstance { get { return _GetInstance; } }
        public Type IViolenceTypeA { get { return _IViolenceTypeA; } }
        public Type IViolenceTypeB { get { return _IViolenceTypeB; } }
        public FightControllerUseInfo(Func<FightAction, FightAction, IResourceHelper, IFightController> getInstance, Type iViolenceTypeA, Type iViolenceTypeB)
        {
            if (!typeof(IViolence).IsAssignableFrom(iViolenceTypeA))
                throw new Exception("The type of iViolenceTypeA(" + iViolenceTypeA + ") is not assignable from IViolence");
            if (!typeof(IViolence).IsAssignableFrom(iViolenceTypeB))
                throw new Exception("The type of iViolenceTypeB(" + iViolenceTypeB + ") is not assignable from IViolence");
            _IViolenceTypeA = iViolenceTypeA;
            _IViolenceTypeB = iViolenceTypeB;
            _GetInstance = getInstance;
        }
    }
}