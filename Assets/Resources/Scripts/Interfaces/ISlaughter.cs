using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ISlaughter
    {
        void OnSlaughterStart();
        bool IsBeingSlaughtered();
    }
}
