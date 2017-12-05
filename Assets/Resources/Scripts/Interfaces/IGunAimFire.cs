using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IGunAimFire
    {
        void Shooting(float requiredAngle, Vector3 position, Vector3 nozelPosition);
        void DoneProjectile();
        List<IGunnable> GetTargets();
    }
}
