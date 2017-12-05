using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ITouchableTwoFingers:ITouchable
    {
        void SwipeTwoFingers(SwipingInfoTwoFingers swipingInfo);
        void TouchTwoFingers(TouchInfoTwoFingers touchInfo);
        void TouchEndedTwoFingers(Vector2 position, Vector2 position2);
    }
}