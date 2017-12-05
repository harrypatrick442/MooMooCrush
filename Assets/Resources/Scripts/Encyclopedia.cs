using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Encyclopedia : MonoBehaviour, ILooperFixedUpdate, ITouchable, IEventSystemHandler
    {
        public enum States { Closed, Opening, Open, Turning, Closing };
        private ILooper _ILooper;
        private IPause _IPause;
        private ITouchSensor _ITouchSensor;
        private States State = States.Closed;
        private ControlButton ControlButtonClose;
        private ControlButton ControlButtonLeft;
        private ControlButton ControlButtonRight;
        private const float FRONT_PAGE_ANGULAR_VELOCITY_OPENING = 130f;
        private const float FRONT_PAGE_ANGULAR_VELOCITY_CLOSING = 130f;
        private const float ANGULAR_VELOCITY_TURNING_PAGE = 200f;
        private bool _CloseScheduled = false;
        private bool _Swiping = false;
        public Action OnClose;
        private GameObject _Menu;
        private GameObject Menu
        {
            get
            {
                if (_Menu == null)
                {
                    _Menu = transform.Find("Menu").gameObject;
                }
                return _Menu;
            }
        }
        private GameObject _Book;
        private GameObject Book
        {
            get
            {
                if (_Book == null)
                    _Book = transform.Find("Book").gameObject;
                return _Book;
            }
        }
        private GameObject _Pivot;
        private GameObject Pivot
        {
            get
            {
                if (_Pivot == null)
                {
                    _Pivot = Book.transform.Find("pivot").gameObject;
                }
                return _Pivot;
            }
        }
        private GameObject _FrontPage;
        private GameObject FrontPage
        {
            get
            {
                if (_FrontPage == null)
                    _FrontPage = Book.transform.Find("FrontPage").gameObject;
                return _FrontPage;
            }
        }
        private Page[] _Pages;
        private Page[] Pages
        {
            get
            {
                if (_Pages == null)
                {
                    _Pages = Book.GetComponentsInChildren<Page>(true);
                }
                return _Pages;
            }
        }
        private int _PageNumber = 0;
        private int PageNumber
        {
            get
            {
                return _PageNumber;
            }
            set
            {
                if (value >= Pages.Length)
                    value = Pages.Length - 1;
                else if (value < 0)
                    value = 0;
                _ILooper.AddFixedUpdate(this);
                State = States.Turning;
                foreach (Page page in Pages)
                {
                    page.DoneAngle = false;
                }
                _PageNumber = value;
            }
        }
        private Rect? _Bounds;
        private Rect Bounds
        {
            get
            {
                if (_Bounds == null)
                {
                    Vector3 size = Pages[0].GetComponentInChildren<MeshRenderer>().bounds.size;
                    _Bounds = new UnityEngine.Rect(Book.transform.position.x - size.x, Book.transform.position.y - (size.y), 2 * size.x, 2 * size.y);
                }
                return (Rect)_Bounds;
            }
        }
        private bool _HideWhenClosed = false;
        private void TurnToPage(int pageNumber)
        {
            int a = (pageNumber - 1) / 2;
            PageNumber = a > 0 ? (a < Pages.Length ? a : Pages.Length - 1) : 0;
        }
        public void Open()
        {
            Open(1);
        }
        public void Open(int pageNumber)
        { 
            
            gameObject.SetActive(true);
            _ITouchSensor.AddTouchable(TouchPriority.UI0, this);
            if (State.Equals(States.Closed) || State.Equals(States.Closing))
            {
                int a = (pageNumber - 1) / 2;
                _PageNumber = a > 0 ? (a < Pages.Length ? a : Pages.Length - 1) : 0;
                bool first = true;
                int i = PageNumber + 1;
                int count = Pages.Length;
                Pages[PageNumber].ShowContent();
                Pages[PageNumber].transform.localPosition = new Vector3(-0.01f, 0, 0);
                Pages[PageNumber].Angle = 1;
                if (PageNumber > 0)
                {
                    Pages[PageNumber - 1].ShowContent();
                    Pages[PageNumber - 1].transform.localPosition = new Vector3(-0.01f, 0, 0);
                    Pages[PageNumber - 1].Angle = 1;
                }
                while (i < count)
                {
                    Page page = Pages[i];
                    page.HideContent();
                    page.Angle = 0.1f;
                    page.transform.localPosition = new Vector3(0.01f, 0, 0);
                    i++;
                }
                i = 0;
                count = PageNumber - 1;
                while (i < count)
                {
                    Page page = Pages[i];
                    page.HideContent();
                    page.Angle = 179f;
                    page.transform.localPosition = new Vector3(0.01f, 0, 0);
                    i++;
                }
                State = States.Opening;
                _CloseScheduled = false;
                _ILooper.AddFixedUpdate(this);
            }
            _IPause.Pause();
        }
        private void _Close()
        {
            State = States.Closing;
            _ILooper.AddFixedUpdate(this);
            int i = 0;
            int count = PageNumber - 1;
            while (i < count)
            {
                Page page = Pages[i];
                page.HideContent();
                i++;
            }
            while (i <= PageNumber)
            {
                Pages[i].transform.localPosition = new Vector3(-0.01f, 0, 0);
                i++;
            }
            count = Pages.Length;
            while (i < count)
            {
                Page page = Pages[i]; 
                page.HideContent();
                i++;
            }
        }
        public void Close()
        {
            if (State.Equals(States.Open) || State.Equals(States.Opening))
            {
                _Close();
            }
            else
            {
                if (!State.Equals(States.Closed) && !State.Equals(States.Closing))
                {
                    _CloseScheduled = true;
                }
            }
        }
        public void Hide()
        {
                _HideWhenClosed = true;
                Close();
        }
        private void _ClosingSetLeftAngles(float angle)
        {
            int i = 0;
            while (i <= PageNumber)
            {
                Page page = Pages[i];
                if (page.Angle > angle)
                    page.Angle = angle;
                i++;
            }
        }
        private void _Closing()
        {
            Menu.SetActive(false);
            if (FrontPage.transform.localEulerAngles.y > 0)
            {
                float a = FrontPage.transform.localEulerAngles.y - (FRONT_PAGE_ANGULAR_VELOCITY_CLOSING * Time.fixedDeltaTime);
                if (a < 0)
                {
                    a = 0;
                    _ClosingSetLeftAngles(0);
                    State = States.Closed;
                }
                FrontPage.transform.localEulerAngles = new Vector3(0, a, 0);
                _ClosingSetLeftAngles(a);
            }
            else
            {
                _ClosingSetLeftAngles(-5);
                State = States.Closed;
            }
        }
        private void _Opening()
        {
            if (FrontPage.transform.localEulerAngles.y < 180)
            {
                float a = FrontPage.transform.localEulerAngles.y + (FRONT_PAGE_ANGULAR_VELOCITY_OPENING * Time.fixedDeltaTime);
                if (a > 180)
                {
                    a = 180;
                    State = States.Open;
                    Menu.SetActive(true);
                    foreach (Page page in Pages)
                    {
                        page.DoneAngle = true;
                    }
                }
                if (PageNumber > 0)
                {
                    Page page = Pages[PageNumber - 1];
                    page.Angle = a - 1;
                }
                FrontPage.transform.localEulerAngles = new Vector3(0, a, 0);
            }
            else
                State = States.Open;
        }
        private float DegreesDifference(float to, float from)
        {
            to = to % 360;
            from = from % 360;
            return to = from;
        }
        private void HomeInOnAngle(Page page, float angleTo)
        {
            bool increasing = angleTo > page.StartAngle;
            float plus = page.Angle;
            while (plus < 0)
                plus += 360;
            plus = (plus % 360);
            float aP = angleTo;
            while (aP < 0)
                aP += 360;
            aP = (aP % 360);
            if (plus == aP)
            {
                page.DoneAngle = true;
                return;
            }
            float difference = aP - plus;
            float differenceMag = difference * Mathf.Sign(difference);
            float a;
            if ((difference > 0 && differenceMag >= 180) || (difference < 0 && differenceMag <= 180))
                a = plus - (ANGULAR_VELOCITY_TURNING_PAGE * Time.fixedDeltaTime);
            else
                a = plus + (ANGULAR_VELOCITY_TURNING_PAGE * Time.fixedDeltaTime);

            if ((a >= aP && increasing) || (a <= aP && !increasing))
            {
                page.DoneAngle = true;
                a = aP;
            }
            page.Angle = a;
        }
        private bool _Turning()
        {
            int i = 0;
            int count = Pages.Length;
            bool doAnother = true;
            bool doneOne = false;
            while (doAnother && i < _PageNumber)
            {
                Page page = Pages[i];
                if (!page.DoneAngle)
                {
                    page.transform.localPosition = new Vector3(-0.04f, 0, 0);
                    doneOne = true;
                    bool isNPage = !(i < _PageNumber - 1);
                    float angleTo = isNPage ? 178 : 179;
                    HomeInOnAngle(page, angleTo);
                    if (page.DoneAngle)
                    {
                        if (i >= _PageNumber - 1)
                            page.ShowContent();
                        if (i != _PageNumber - 1)
                        {
                            page.transform.localPosition = new Vector3(-0.01f, 0, 0);
                            if (i > 0)
                                Pages[i - 1].HideContent();
                        }
                    }
                }
                i++;
            }
            i = count - 1;
            while (i >= _PageNumber)
            {
                Page page = Pages[i];
                bool isNPage = !(i > _PageNumber);
                if (isNPage)
                    page.ShowContent();
                if (!page.DoneAngle)
                {
                    doneOne = true;
                    float angleTo = isNPage ? 1 : 0.1f;
                    HomeInOnAngle(page, angleTo);
                    if (page.DoneAngle)
                    {
                        int previousIndex = i + 1;
                        if (count > previousIndex)
                        {
                            Pages[previousIndex].HideContent();
                        }
                        if (!isNPage)
                            page.transform.localPosition = new Vector3(-0.01f, 0, 0);
                    }
                }
                i--;
            }
            if (!doneOne)
            {
                State = States.Open;
            }
            return !doneOne;
        }
        public bool LooperFixedUpdate()
        {
            switch (State)
            {
                case States.Closed:
                    OnClose();
                    Debug.Log("removed");
                    _ITouchSensor.RemoveTouchable(this);
                    gameObject.SetActive(false);
                    _IPause.Unpause();
                    _HideWhenClosed = false;
                    return true;
                case States.Open:
                    return false;
                case States.Opening:
                    _Opening();
                    return false;
                case States.Turning:
                    return _Turning();
                case States.Closing:
                    _Closing();
                    return false;
            }
            return true;
        }
        private void Start()
        {
            foreach (ControlButton controlButton in Menu.GetComponentsInChildren<ControlButton>())
            {
                switch (controlButton.name)
                {
                    case Strings.BUTTON_ENCYCLOPEDIA_CLOSE:
                        ControlButtonClose = controlButton;
                        ControlButtonClose.SetDelegates(null, () => { Hide(); });
                        break;
                    case Strings.BUTTON_ENCYCLOPEDIA_LEFT:
                        ControlButtonLeft = controlButton;
                        ControlButtonLeft.SetDelegates(null, () => { PageNumber--; });
                        break;
                    case Strings.BUTTON_ENCYCLOPEDIA_RIGHT:
                        ControlButtonRight = controlButton;
                        ControlButtonRight.SetDelegates(null, () => { PageNumber++; });
                        break;
                }
            }
        }
        public void SetInterface(object o)
        {
            if (typeof(IPause).IsAssignableFrom(o.GetType()))
                _IPause = (IPause)o;
            if (typeof(ILooper).IsAssignableFrom(o.GetType()))
                _ILooper = (ILooper)o;
            if (typeof(ITouchSensor).IsAssignableFrom(o.GetType()))
                _ITouchSensor = (ITouchSensor)o;
        }
        public void Swipe(SwipingInfo swipingInfo)
        {
            if (_Swiping)
            {
                if (State.Equals(States.Open) || State.Equals(States.Turning))
                {
                    Vector2 distance = swipingInfo.TotalDistance;
                    if (distance.magnitude > 0.2)
                    {
                        if (swipingInfo.TotalSwipeDirection.Equals(SwipingInfo.SwipeDirections.Right))
                            PageNumber--;
                        else
                            PageNumber++;
                        _Swiping = false;
                    }
                }
            }
        }
        public void Touch(TouchInfo touchInfo)
        {
            _Swiping = Bounds.Contains(touchInfo.Position);
        }
        public void TouchEnded(Vector2 positoin)
        {
            _Swiping = false;
        }
    }
}
