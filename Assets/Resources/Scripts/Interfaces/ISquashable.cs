using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public interface ISquashable
    {
        SquashInfo GetSquashInfo();
        void Squash();
        bool IsSquashed();
        void DoneSquash();
    }
}
