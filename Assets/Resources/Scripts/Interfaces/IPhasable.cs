using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    interface IPhasable:IGetPosition2D,ISlaughter
    {
        void Phase();
        void DonePhase();
        bool IsPhased();
		bool IsPhasable{ get;}
    }
}
