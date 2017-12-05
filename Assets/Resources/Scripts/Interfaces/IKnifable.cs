using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    interface IKnifable:IGetPosition2D, IGhost, ISlaughter
    {
        void RitualKnife();
        bool IsInRitualKnifeBounds(Vector2 position);
        Vector2 GetRitualKnifeSnapInPlacePosition();
        bool IsRitualKnifed();
        void DoneRitualKnife();
    }
}
