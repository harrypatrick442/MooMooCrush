using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class TouchSensor: ITouchSensor
    {
        private DictionaryOfListsWithKeyMemory<TouchPriority, ITouchable> _ITouchables = new DictionaryOfListsWithKeyMemory<TouchPriority, ITouchable>();
        private DictionaryOfListsWithKeyMemory<TouchPriority, ITouchableTwoFingers> _ITouchablesTwoFingers = new DictionaryOfListsWithKeyMemory<TouchPriority, ITouchableTwoFingers>();
        private List<ITouchable> _ToRemove = new List<ITouchable>();
        private List<Tuple<TouchPriority, ITouchable>> _ToAdd = new List<Tuple<TouchPriority, ITouchable>>();
        private List<Tuple<TouchPriority, ITouchableTwoFingers>> _ToAddTwoFingers = new List<Tuple<TouchPriority, ITouchableTwoFingers>>();
        private List<ITouchableTwoFingers> _ToRemoveTwoFingers = new List<ITouchableTwoFingers>();
        private Vector2 lastPosition;
        private Vector2 lastPositionSecond;
        private Vector2 lastPositionScreen;
        private Vector2 lastPositionScreenSecond;
        private Func<Vector2> GetScreenPosition;
        private Func<Vector2> GetSecondScreenPosition;
        private Func<Vector2> GetDeltaPosition;
        private Func<Vector2> GetDeltaPositionSecondFinger;
        private Func<bool> IsTouching;
        private Func<bool> IsTwoFingerTouching;
        private bool mouseWasDown = false;
        private bool mouseWasDownSecondFinger = false;
        private Vector2? previousMousePosition;
        private Vector2? previousMousePositionSecondFinger;
        private Vector2 deltaMousePosition;
        private Vector2 deltaMousePositionSecondFinger;
        private Vector2 startPosition;
        private Vector2 startPositionSecond;
        private Vector2 startPositionScreen;
        private Vector2 startPositionScreenSecond;
        private bool sentTouch = false;
        private bool sentTouchSecond = false;
        private bool sentStatic = true;
        private bool sentStaticSecond = false;
        private bool _TouchHandled;
        private bool _WDown = false;
        private bool _TDown = false;
        private float timeStarted;
        private Vector2 _TwoFingerDesktopFirstScreenPosition;
        private Camera Camera;
        private List<TouchPriority> _TouchPriorities;
        public TouchSensor(List<TouchPriority> touchPriorities, Camera camera)
        {
            _TouchPriorities = touchPriorities;
            Camera = camera;
            bool desktop = Application.platform == RuntimePlatform.WindowsEditor;
            GetScreenPosition = desktop ? new Func<Vector2>(() =>
            {
                if(!_TDown)
                    return Input.mousePosition;
                return _TwoFingerDesktopFirstScreenPosition;
            }) : new Func<Vector2>(() =>
            {
                return Input.GetTouch(0).position;
            });
            GetSecondScreenPosition = desktop ? new Func<Vector2>(() =>
            {
                    return Input.mousePosition;
            }) : new Func<Vector2>(() =>
            {
                return Input.GetTouch(1).position;
            });
            GetDeltaPosition = desktop ? new Func<Vector2>(() =>
            {
                return deltaMousePosition;
            }) : new Func<Vector2>(() =>
            {
                return Camera.ScreenToWorldPoint(Input.GetTouch(0).deltaPosition);
            });
            GetDeltaPositionSecondFinger = desktop ? new Func<Vector2>(() =>
            {
                return deltaMousePositionSecondFinger;
            }) : new Func<Vector2>(() =>
            {
                return Camera.ScreenToWorldPoint(Input.GetTouch(1).deltaPosition);
            });
            IsTouching = desktop ? new Func<bool>(() =>
            {
                if (!_WDown)
                {
                    bool down = Input.GetMouseButton(0);
                    if (down)
                    {
                        Vector2 currentPosition = Camera.ScreenToWorldPoint((Vector2)Input.mousePosition);
                        if (previousMousePosition != null)
                        {
                            deltaMousePosition = (currentPosition - (Vector2)previousMousePosition);
                        }
                        previousMousePosition = currentPosition;
                    }
                    bool r = mouseWasDown && down;
                    mouseWasDown = down;
                    return r;
                }
                else
                {
                    return mouseWasDown;
                }
            }) : new Func<bool>(() =>
            {
                return Input.touchCount > 0;
            });
            IsTwoFingerTouching = desktop ? new Func<bool>(() =>
            {
                if (Input.GetKey(KeyCode.T) == true)
                {
                    if (!_TDown)
                    {
                        _TwoFingerDesktopFirstScreenPosition = GetScreenPosition();
                        _TDown = true;
                        _WDown = false;
                    }
                    else
                    {
                            _WDown = (Input.GetKey(KeyCode.W) == true);
                        if(_WDown)
                        {
                            Vector2 currentPosition = Camera.ScreenToWorldPoint((Vector2)Input.mousePosition);
                            if (previousMousePositionSecondFinger != null)
                            {
                                deltaMousePositionSecondFinger = (currentPosition - (Vector2)previousMousePositionSecondFinger);
                            }
                            previousMousePositionSecondFinger = currentPosition;
                        }
                        bool r = mouseWasDownSecondFinger && _WDown;
                        mouseWasDownSecondFinger = _WDown;
                        return r;
                    }
                }
                else
                    _WDown = false;
                return false;
            }) :
            new Func<bool>(() => {
                return Input.touchCount > 1;
            });

        }
        private void _TouchEnded()
        {
            sentTouch = false;
            foreach (TouchPriority i in _TouchPriorities)
                foreach (ITouchable iTouchable in _ITouchables[i])
                iTouchable.TouchEnded(lastPosition);
        }
        private void _TouchEndedTwoFingers()
        {
            sentTouchSecond = false;
            _WDown = false;
            _TDown = false;
            foreach (TouchPriority i in _TouchPriorities)
            {
                foreach (ITouchableTwoFingers iTouchableTwoFingers in _ITouchablesTwoFingers[i])
                    iTouchableTwoFingers.TouchEndedTwoFingers(lastPosition, lastPositionSecond);
            }
        }
        private void _Touch(Vector2 screenPosition, Vector2 p, float dT)
        {

            Tuple<List<ITouchable>, RaycastHit2D[]> tuples = GetHits(p);
            bool hitITouchables = tuples.A.Count > 0;
            foreach (TouchPriority i in _TouchPriorities)
            foreach (ITouchable iTouchable in _ITouchables[i])
            {
                iTouchable.Touch(new TouchInfo(SetTouchHandled, _TouchHandled, screenPosition, p, tuples.B, tuples.A.Contains(iTouchable), hitITouchables));
            }
            sentTouch = true;
            lastPosition = p;
            lastPositionScreen = screenPosition;
            startPosition = lastPosition;
            startPositionScreen = lastPositionScreen;
            timeStarted = Time.time;
        }
        private Tuple<List<ITouchable>, RaycastHit2D[]>GetHits(Vector2 p)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(p, Vector2.zero);
            List<ITouchable> iTouchables = new List<ITouchable>();
            foreach (RaycastHit2D hit in hits)
            {
                ITouchable iTouchable = Lookup.GetBody<ITouchable>(hit.collider.gameObject);
                //Debug.Log(hit.collider.gameObject.name);
                if(iTouchable!=null)
                {
                    iTouchables.Add(iTouchable);
                }
            }
            return new Tuple<List<ITouchable>, RaycastHit2D[]>(iTouchables, hits);
        }
        private void _TouchTwoFingers(Vector2 screenPosition, Vector2 p, Vector2 secondScreenPosition, Vector2 secondPosition, float dT)
        {
            Tuple<List<ITouchable>, RaycastHit2D[]> tuplesFirstFinger = GetHits(p);
            Tuple<List<ITouchable>, RaycastHit2D[]> tuplesSecondFinger = GetHits(p);
            foreach(TouchPriority i in _TouchPriorities)
            foreach (ITouchableTwoFingers iTouchableTwoFingers in _ITouchablesTwoFingers[i])
            {
                ITouchable iTouchable = (ITouchable)iTouchableTwoFingers;
                TouchInfo touchInfoFirstFinger = new TouchInfo(SetTouchHandled, _TouchHandled, screenPosition, p, tuplesFirstFinger.B, tuplesFirstFinger.A.Contains(iTouchable), tuplesFirstFinger.A.Count>0);
                TouchInfo touchInfoSecondFinger = new TouchInfo(SetTouchHandled, _TouchHandled, secondScreenPosition, secondPosition, tuplesSecondFinger.B, tuplesSecondFinger.A.Contains(iTouchable), tuplesSecondFinger.A.Count > 0);
                iTouchableTwoFingers.TouchTwoFingers(      new TouchInfoTwoFingers(touchInfoFirstFinger, touchInfoSecondFinger));
            }
            sentTouchSecond = true;
            lastPosition = p;
            lastPositionSecond = secondPosition;
            lastPositionScreen = screenPosition;
            lastPositionScreenSecond = secondScreenPosition;
            startPosition = lastPosition;
            startPositionSecond = lastPositionSecond;
            startPositionScreen = lastPositionScreen;
            startPositionScreenSecond = lastPositionScreenSecond;
            timeStarted = Time.time;
        }
        private void _Swiping(Vector2 screenPosition, Vector2 p, float dT)
        {
            sentStatic = false;
            Vector2 delta = p - lastPosition;
            lastPosition = p;
            lastPositionScreen = screenPosition;
            SwipingInfo swipingInfo = new SwipingInfo(SetTouchHandled, _TouchHandled, delta, startPositionScreen, startPosition, screenPosition, p, dT, timeStarted, Time.time);
            foreach (TouchPriority i in _TouchPriorities)
                foreach (ITouchable iTouchable in _ITouchables[i])
                iTouchable.Swipe(swipingInfo);
        }
        private void _SwipingTwoFingers(Vector2 screenPosition, Vector2 p, Vector2 secondScreenPosition, Vector2 secondPosition, float dT)
        {
            SwipingInfo swipingInfoFirstFinger;
            SwipingInfo swipingInfoSecondFinger;
            bool firstIsStatic = GetDeltaPosition().sqrMagnitude != 0;
            bool secondIsStatic = GetDeltaPositionSecondFinger().sqrMagnitude != 0;
            if (firstIsStatic)
            {
                Vector2 delta = p - lastPosition;
                lastPosition = p;
                lastPositionScreen = screenPosition;
                swipingInfoFirstFinger = new SwipingInfo(SetTouchHandled, _TouchHandled, delta, startPositionScreen, startPosition, screenPosition, p, dT, timeStarted, Time.time);

            }
            else
            {
                swipingInfoFirstFinger = new SwipingInfo(SetTouchHandled, _TouchHandled, startPositionScreen, startPosition, screenPosition, p, dT, timeStarted, Time.time);
            }
            if (secondIsStatic)
            {
                Vector2 deltaSecond = secondPosition - lastPositionSecond;
                lastPositionSecond = secondPosition;
                lastPositionScreenSecond = secondScreenPosition;
                swipingInfoSecondFinger = new SwipingInfo(SetTouchHandled, _TouchHandled, deltaSecond, startPositionScreenSecond, startPositionSecond, secondScreenPosition, secondPosition, dT, timeStarted, Time.time);
            }
            else
            {
                swipingInfoSecondFinger = new SwipingInfo(SetTouchHandled, _TouchHandled, startPositionScreenSecond, startPositionSecond, secondScreenPosition, secondPosition, dT, timeStarted, Time.time);
            }
            SwipingInfoTwoFingers swipingInfoTwoFingers = new SwipingInfoTwoFingers(swipingInfoFirstFinger, swipingInfoSecondFinger);
            if (firstIsStatic && secondIsStatic)
            {
                if (!sentStaticSecond)
                {
                    sentStatic = true;
                    foreach (TouchPriority i in _TouchPriorities)
                        foreach (ITouchableTwoFingers iTouchableTwoFingers in _ITouchablesTwoFingers[i])
                    {
                        iTouchableTwoFingers.SwipeTwoFingers(swipingInfoTwoFingers);
                    }
                }
            }
            else
            {
                foreach (TouchPriority i in _TouchPriorities)
                    foreach (ITouchableTwoFingers iTouchableTwoFingers in _ITouchablesTwoFingers[i])
                {
                    iTouchableTwoFingers.SwipeTwoFingers(swipingInfoTwoFingers);
                }
            }
        }
        private void _StaticSwiping(Vector2 screenPosition, Vector2 p, float dT)
        {
            sentStatic = true;
            SwipingInfo swipingInfo = new SwipingInfo(SetTouchHandled, _TouchHandled, startPositionScreen, startPosition, screenPosition, p, dT, timeStarted, Time.time);
            foreach (TouchPriority i in _TouchPriorities)
                foreach (ITouchable iTouchable in _ITouchables[i])
                iTouchable.Swipe(swipingInfo);
        }
        public void SetTouchHandled()
        {
            _TouchHandled = true;
        }
        public void Run(float dT)
        {
            if (!IsTouching())
            {
                if (sentTouch)
                {
                    _TouchHandled = false;
                    _TouchEnded();
                }
                if (sentTouchSecond)
                {
                    _TouchHandled = false;
                    _TouchEndedTwoFingers();
                }
            }
            else
            {
                Vector2 screenPosition = GetScreenPosition();
                Vector2 p = Camera.ScreenToWorldPoint(screenPosition);
                
                if (!IsTwoFingerTouching())
                {
                    if (!sentTouch)
                    {
                        _Touch(screenPosition, p, dT);
                    }
                    else
                    {
                        if (GetDeltaPosition().sqrMagnitude != 0)
                        {
                            _Swiping(screenPosition, p, dT);
                        }
                        else
                        {
                            if (!sentStatic)
                            {
                                _StaticSwiping(screenPosition, p, dT);
                            }
                        }
                    }
                }
                else
                {
                    Vector2 screenPositionSecond = GetSecondScreenPosition();
                    Vector2 positionSecond = Camera.ScreenToWorldPoint(screenPositionSecond);
                    if (!sentTouchSecond)
                    {
                        _TouchTwoFingers(screenPosition, p, screenPositionSecond, positionSecond, dT);
                    }
                    else
                    {
                        _SwipingTwoFingers(screenPosition, p, screenPositionSecond, positionSecond, dT);
                    }
                }
            }
                foreach (Tuple<TouchPriority, ITouchable> tuple in _ToAdd)
            {
                    _ITouchables.Add(tuple.A, tuple.B);
            }
            _ToAdd.Clear();
            foreach (ITouchable iTouchable in _ToRemove)
            {
                    _ITouchables.Remove(iTouchable);
            }
            _ToRemove.Clear();
            foreach (Tuple<TouchPriority, ITouchableTwoFingers> tuple in _ToAddTwoFingers)
            {
                    _ITouchablesTwoFingers.Add(tuple.A, tuple.B);
            }
            _ToAddTwoFingers.Clear();
            foreach (ITouchableTwoFingers iTouchable in _ToRemoveTwoFingers)
            {
                    _ITouchablesTwoFingers.Remove(iTouchable);
            }
            _ToRemoveTwoFingers.Clear();
        }
        public void AddTouchable(TouchPriority touchPriority, ITouchable iTouchable)
        {
            if (typeof(ITouchableTwoFingers).IsAssignableFrom(iTouchable.GetType()))
                _ToAddTwoFingers.Add(new Tuple<TouchPriority, ITouchableTwoFingers>(touchPriority, (ITouchableTwoFingers)iTouchable));
            _ToAdd.Add(new Tuple<TouchPriority, ITouchable>(touchPriority, iTouchable));
        }
        public void RemoveTouchable(ITouchable iTouchable)
        {
            if (typeof(ITouchableTwoFingers).IsAssignableFrom(iTouchable.GetType()))
                _ToRemoveTwoFingers.Add((ITouchableTwoFingers)iTouchable);
            _ToRemove.Add(iTouchable);
        }
    }
}
