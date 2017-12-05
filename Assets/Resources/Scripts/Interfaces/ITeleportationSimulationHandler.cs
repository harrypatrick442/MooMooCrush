using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ITeleportationSimulationHandler
    {
        void SimulateTeleportation(Vector2 from, Vector2 to, float radius, Action callbackSwitch);
    }
}