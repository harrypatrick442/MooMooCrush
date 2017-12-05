using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class TouchInfo
    {
        public Vector2 ScreenPosition;
        public Vector2 Position;
        public RaycastHit2D Hit;
        public RaycastHit2D[] Hits;
        public bool WasHit;
        public bool HitITouchables;
        public Action SetHandled;
        private bool _Handled;
        public bool Handled
        {
            get
            {
                return _Handled;
            }
        }
        public TouchInfo(Action setHandled, bool touchHandled, Vector2 screenPosition, Vector2 position, RaycastHit2D[] hits, bool wasHit, bool hitITouchables)
        {
            SetHandled = setHandled;
            _Handled = touchHandled;
            ScreenPosition = screenPosition;
            Position = position;
            if(hits.Length>0)
                Hit = hits[0];
            Hits = hits;
            WasHit = wasHit;
            HitITouchables = hitITouchables;
        }
    }
}
