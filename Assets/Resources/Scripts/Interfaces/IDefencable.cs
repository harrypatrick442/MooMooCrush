using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IDefencable:IHealth, IGetPosition2D, IPopable, IVibratable
    {

    }
}