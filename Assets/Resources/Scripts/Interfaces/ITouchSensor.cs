using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ITouchSensor :ISetTouchHandled
    {
        void AddTouchable(TouchPriority touchPriority, ITouchable iTouchable);
        void RemoveTouchable(ITouchable iTouchable);
    }
}