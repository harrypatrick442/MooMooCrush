using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IMachineGunable:IGhost, ISlaughter
    {
        void MachineGun(Vector2 position);
        void DoneMachineGun();
    }
}
