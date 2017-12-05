using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IFightController : IPausible
    {//clocked internally
        void Start();
        void Tick();
        void Stop();
        bool IsDone { get; }
    }
}
