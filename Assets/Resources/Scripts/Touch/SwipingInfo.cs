using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class SwipingInfo
    {
        public enum Types { Traveled, Static}
        public Types Type;
        public enum SwipeDirections
        {
            Up,
            Down,
            Right,
            Left
        }
        private SwipeDirections? _SwipeDirection;
        public SwipeDirections SwipeDirection
        {
            get
            {
                if (_SwipeDirection == null) _SwipeDirection = Mathf.Abs(Deltas.x) > Mathf.Abs(Deltas.y) ? (Deltas.x > 0 ? SwipeDirections.Right : SwipeDirections.Left) : (Deltas.y > 0 ? SwipeDirections.Up : SwipeDirections.Down); return(SwipeDirections) _SwipeDirection;
            }
        }
        private SwipeDirections? _TotalSwipeDirection;
        public SwipeDirections TotalSwipeDirection
        {
            get
            {
                if(_TotalSwipeDirection==null)
                {
                    Vector2 deltas = Position - StartPosition;
                _TotalSwipeDirection = Mathf.Abs(deltas.x) > Mathf.Abs(deltas.y) ? (deltas.x > 0 ? SwipeDirections.Right : SwipeDirections.Left) : (Deltas.y > 0 ? SwipeDirections.Up : SwipeDirections.Down);
                }
                return (SwipeDirections)_TotalSwipeDirection;
            }
        }
        private Vector2? _TotalDistance=null;
        public Vector2 TotalDistance
        {get
            {
                if(_TotalDistance==null)
                    _TotalDistance= Position - StartPosition;
                return(Vector2) _TotalDistance;
            }
        }
        private float? _TotalSpeedMagnitude= null;
        public float TotalSpeedMagnitude
        {
            get
            {
                if(_TotalSpeedMagnitude==null)
                    _TotalSpeedMagnitude= TotalDistance.magnitude / (TimeNow - TimeStarted);
                return (float)_TotalSpeedMagnitude;
            }
        }
        private float? _SpeedMagnitude;
        public float SpeedMagnitude
        {
            get
            {
                if(_SpeedMagnitude==null)
                    _SpeedMagnitude= Deltas.magnitude / DT;
                return (float)_SpeedMagnitude;
            }
        }
        public Vector2 Deltas;
        public Vector2 Position;
        public Vector2 StartPosition;
        public Vector2 PositionScreen;
        public Vector2 StartPositionScreen;
        public float DT;
        public float TimeStarted;
        public float TimeNow;
        public Action SetHandled;
        private bool _Handled;
        public bool Handled
        {
            get
            {
                return _Handled;
            }
        }
        public SwipingInfo(Action setHandled, bool handled, Vector2 startPositionScreen, Vector2 startPosition, Vector2 positionScreen, Vector2 position, float dT, float timeStarted, float timeNow)
        {
            SetHandled = setHandled;
            _Handled= handled;
            Type = Types.Static;
            Position = position;
            StartPosition = startPosition;
            PositionScreen = positionScreen;
            StartPositionScreen = startPositionScreen;
            DT = dT;
            TimeNow = timeNow;
        }
        public SwipingInfo(Action setHandled, bool handled, Vector2 deltas, Vector2 startPositionScreen, Vector2 startPosition, Vector2 positionScreen, Vector2 position, float dT, float timeStarted, float timeNow)
        {
            SetHandled = setHandled;
            _Handled = handled;
            Type = Types.Traveled;
            Deltas = deltas;
            StartPosition = startPosition;
            StartPositionScreen = startPositionScreen;
            PositionScreen = positionScreen;
            DT = dT;
            TimeStarted = timeStarted;
            Position = position;
            TimeNow = timeNow;
        }

    }
}
