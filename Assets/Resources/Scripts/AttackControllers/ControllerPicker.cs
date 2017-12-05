using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Assets.Scripts
{
    public class ControllerPicker:IAttackController
    {
        private IResourceHelper _IResouceHelper;
        private Dictionary<Type, Dictionary<Type, List<Func<FightAction, FightAction, IResourceHelper, IFightController>>>> _MapIViolenceTypeToControllerType = new Dictionary<Type, Dictionary<Type, List<Func<FightAction, FightAction, IResourceHelper, IFightController>>>>();
        public IFightController Pick(FightAction fightActionA, FightAction fightActionB)
        {
            if (_MapIViolenceTypeToControllerType.ContainsKey(fightActionA.IViolence.GetType()))
            {
                Dictionary<Type, List<Func<FightAction, FightAction, IResourceHelper, IFightController>>> secondaryMap = _MapIViolenceTypeToControllerType[fightActionA.IViolence.GetType()];
                if (secondaryMap.ContainsKey(fightActionB.IViolence.GetType()))
                {
                    List<Func<FightAction, FightAction, IResourceHelper, IFightController>> list = secondaryMap[fightActionB.IViolence.GetType()];
                    IFightController iFightController = list[Random.Range((int)0, list.Count)](fightActionA, fightActionB, _IResouceHelper);
                    return iFightController;
                }
            }
            Exception exception = new Exception("The required controller could not be found :(! Please add one!");
            Debug.Log(exception);
            throw exception;
        }
        public ControllerPicker(List<FightControllerUseInfo> fightControllerUseInfos, IResourceHelper iResourceHelper){
            _IResouceHelper = iResourceHelper;
            foreach(FightControllerUseInfo fightControllerUseInfo in fightControllerUseInfos)
            {
                foreach(Tuple<Type, Type> tuple in new Tuple<Type, Type>[] { new Tuple<Type, Type>(fightControllerUseInfo.IViolenceTypeA, fightControllerUseInfo.IViolenceTypeB), new Tuple<Type, Type>(fightControllerUseInfo.IViolenceTypeB, fightControllerUseInfo.IViolenceTypeA) })
                {
                    Dictionary<Type, List<Func<FightAction, FightAction, IResourceHelper, IFightController>>> secondMap;
                    if (!_MapIViolenceTypeToControllerType.ContainsKey(tuple.A))
                    {
                        secondMap = new Dictionary<Type, List<Func<FightAction, FightAction, IResourceHelper, IFightController>>>
                        {
                            {tuple.B, new List<Func<FightAction, FightAction, IResourceHelper, IFightController>> { fightControllerUseInfo.GetInstance }}
                        };
                        _MapIViolenceTypeToControllerType[tuple.A] = secondMap;
                    }
                    else
                    {
                        secondMap = _MapIViolenceTypeToControllerType[tuple.A];
                        if(!secondMap.ContainsKey(tuple.B))
                        {
                            secondMap[tuple.B] = new List<Func<FightAction, FightAction, IResourceHelper, IFightController>> { fightControllerUseInfo.GetInstance };
                        }
                        else
                        {
                            List<Func<FightAction, FightAction, IResourceHelper, IFightController>> list = secondMap[tuple.B];
                            //if (!list.Contains(fightControllerUseInfo.GetInstance))
                            //{
                            //not needed
                                list.Add(fightControllerUseInfo.GetInstance);
                            //}
                        }
                    }
                }
            }
            //foreach(var a in _MapIViolenceTypeToControllerType.Keys)
            //{
                //foreach(var b in _MapIViolenceTypeToControllerType[a].Keys)
                //{
                    //Debug.Log(a + "_" + b);
                //}
            //}
        }
    }
}