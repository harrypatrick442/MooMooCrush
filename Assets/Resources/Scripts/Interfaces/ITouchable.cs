using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ITouchable
    {
        void Swipe(SwipingInfo swipingInfo);
        void Touch(TouchInfo touchInfo);
        void TouchEnded(Vector2 positoin);
    }
}