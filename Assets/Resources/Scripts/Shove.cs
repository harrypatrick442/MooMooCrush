using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class Shove<T> :ITouchable where T : IShove
    {
        private List<T> beingShoveds = new List<T>();
        //private SwipingInfo.SwipeDirections? swipeDirection;
        public Shove(ITouchSensor iTouchSensor)
        {
            iTouchSensor.AddTouchable(TouchPriority.Cows, this);
        }
        public void Shoving(SwipingInfo swipingInfo)
        {
            RaycastHit2D hit = Physics2D.Raycast(swipingInfo.Position, Vector2.zero);
                if (hit)
            {
                T t = Lookup.GetBody<T>(hit.collider.gameObject);
                    if (t != null)
                {
                    if (t.IsShovable&&!beingShoveds.Contains(t))
                        beingShoveds.Add(t);
                    }
                }
               // if (swipeDirection != null)
            //{
               // Debug.Log("b");
               // if (!swipeDirection.Equals(swipingInfo.SwipeDirection))
               //     {// return;
               // }
                //}
               // else
               //     swipeDirection = swipingInfo.SwipeDirection;
               
            int i = 0;
                int count = beingShoveds.Count;
            bool isLeft =  swipingInfo.TotalSwipeDirection.Equals(SwipingInfo.SwipeDirections.Left);
                while (i < count)
            {
                T beingShoved = beingShoveds[i];
                    if(beingShoved.Shove(new ShoveInfo(isLeft, swipingInfo.Position.x, swipingInfo.DT)))
                {
                    beingShoveds.Remove(beingShoved);
                        count--;
                    }
                    else
                        i++;
                }

            }
        public void Reset()
        {
            beingShoveds.Clear();
        }
        private bool StartShove(RaycastHit2D[] hits)
        {
            beingShoveds.Clear();
            foreach (RaycastHit2D hit in hits)
            {
                T t = Lookup.GetBody<T>(hit.collider.gameObject);
                if (t != null)
                {
                    if (t.IsShovable && !beingShoveds.Contains(t))
                        beingShoveds.Add(t);
                }
            }
            return beingShoveds.Count > 0;
        }
        private bool _Handled = false;
        public void Swipe(SwipingInfo swipingInfo)
        {
            if (!_Handled)
            {
                Shoving(swipingInfo);
            }
        }
        public void Touch(TouchInfo touchInfo)
        {
            if (!touchInfo.Handled)
            {
                if (StartShove(touchInfo.Hits))
                {
                    touchInfo.SetHandled();
                }
                _Handled = false;
            }
            else
            {
                _Handled = true;
            }
        }

        public void TouchEnded(Vector2 positoin)
        {

        }
    }
}
