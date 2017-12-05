using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ICrushable: ISlaughter, IGetRect
    {
        void Crush(CrushInfo crushInfo);
        GameObject GetCrushParticleSystem();
    }
}