using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IExploder
    {
        void Explode(Vector2 point, ExplosionInfo explosionInfo);
    }
}
