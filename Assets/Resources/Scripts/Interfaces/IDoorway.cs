using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IDoorway
    {
        void ExitedThroughDoorway(Enums.DoorwayType type, IDoorwayable iDoorwayable);
    }
}