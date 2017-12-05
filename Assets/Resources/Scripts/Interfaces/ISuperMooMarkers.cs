using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ISuperMooMarkers
    {
        Vector2 SuperMooFlyToStartPosition { get; }
        Rect[] AllowedRegionsSuperMoo { get; }
    }
}
