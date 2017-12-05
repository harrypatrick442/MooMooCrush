using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IFightable : IGetPosition2D, IIsRepositioningGet
    {
        IViolenceHandler ViolenceHandler { get; }
        IFighterController FighterController { get; }
    }
}