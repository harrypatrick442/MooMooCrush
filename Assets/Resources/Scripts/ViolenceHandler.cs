using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
    public class ViolenceHandler : IViolenceHandler
    {
        private Dictionary<Enums.Attacks, IAttack> _MapAttackTypeToIAttack = new Dictionary<Enums.Attacks, IAttack>();
        private Dictionary<Enums.Attacks, List<IDefence>> _MapAttackTypeToIDefences = new Dictionary<Enums.Attacks, List<IDefence>>();
        public List<Enums.Attacks> ReceiveAttacks
        {
            get
            {
                return new List<Enums.Attacks>(_MapAttackTypeToIDefences.Keys);
            }
        }

        public List<Enums.Attacks> SendAttacks
        {
            get
            {
                return new List<Enums.Attacks>(_MapAttackTypeToIAttack.Keys);
            }
        }
        public ViolenceHandler(List<IViolence> list)
        {
            foreach (IViolence iViolence in list)
            {
                if (typeof(IAttack).IsAssignableFrom(iViolence.GetType()))
                {
                    IAttack iAttack = (IAttack)iViolence;
                    _MapAttackTypeToIAttack[iAttack.AttackType] = iAttack;
                }
                if (typeof(IDefence).IsAssignableFrom(iViolence.GetType()))
                {
                    IDefence iDefence= (IDefence)iViolence;
                    foreach (Enums.Attacks attackType in iDefence.AttackTypes)
                    {
                        List<IDefence> iDefences;
                        if (_MapAttackTypeToIDefences.ContainsKey(attackType))
                        {
                             iDefences = _MapAttackTypeToIDefences[attackType];
                            if (!iDefences.Contains(iDefence))
                                iDefences.Add(iDefence);
                        }
                        else
                        {
                            iDefences = new List<IDefence>();
                            iDefences.Add(iDefence);
                            _MapAttackTypeToIDefences[attackType] = iDefences;
                        }
                    }
                }
            }
        }
        public IAttack GetIAttack(Enums.Attacks attack)
        {
            if (_MapAttackTypeToIAttack.ContainsKey(attack))
            {
                    return _MapAttackTypeToIAttack[attack];
            }
            return null;
        }
        public List<IDefence> GetIDefences(Enums.Attacks attack)
        {
            if (_MapAttackTypeToIDefences.ContainsKey(attack))
            {
                return _MapAttackTypeToIDefences[attack];
            }
            return new List<IDefence>();
        }
    }
}