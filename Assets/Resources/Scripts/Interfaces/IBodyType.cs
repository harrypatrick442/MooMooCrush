using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IBodyType
    {
        void TakeKinematic(object owner);
        void ReleaseKinematic(object owner);
    }
}