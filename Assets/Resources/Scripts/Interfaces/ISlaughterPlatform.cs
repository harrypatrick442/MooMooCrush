using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ISlaughterPlatform : IGetTransform
    {
        List<T> GetInstances<T>();
        Transform GetTransform();
    }
}