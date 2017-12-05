using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ILaserable:IBurnable, IGetPosition2D
    {
        void Lasered();
        bool IsLasered { get; }
    }
}