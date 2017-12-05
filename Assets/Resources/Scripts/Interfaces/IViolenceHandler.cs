using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IViolenceHandler
    {
        List<Enums.Attacks> SendAttacks { get; }
        List<Enums.Attacks> ReceiveAttacks { get; }
        IAttack GetIAttack(Enums.Attacks attack);
        List<IDefence> GetIDefences(Enums.Attacks attack);
    }
}