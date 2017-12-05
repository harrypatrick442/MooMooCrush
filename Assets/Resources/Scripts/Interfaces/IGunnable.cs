using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IGunnable:IGetPosition2D,ISlaughter
    {
        void Gun();
        void Done();
        bool IsGunned();
    }
}
