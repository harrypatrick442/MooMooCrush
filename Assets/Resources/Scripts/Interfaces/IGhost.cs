using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IGhost: IGetPosition2D, IPosition2D
    {
        GameObject GetGhostPrefab();
    }
}