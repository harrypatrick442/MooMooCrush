using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class ControlButton : MonoBehaviour, ITouchControls, IRelocatible, IPausible, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler
    {
        private ISetTouchHandled _ISetTouchHandled;
        public enum Types { SinglePush, Toggle }
        public Types Type;
        public Sprite SpriteUp;
        public Sprite SpriteDown;
        public Sprite SpriteActiveUp;
        public Sprite SpriteActiveDown;
        private Action _GoingDown;
        private Action _GoingUp;
        protected Action _GoneDown;
        protected Action _GoneUp;
        private bool _Paused = false;
        private bool _IsCanvas = false;
        private Sprite Sprite
        {
            set
            {
                if (!_IsCanvas)
                {
                    SpriteRenderer spriteRenderer = SpriteRenderer;
                    if (!_IsCanvas)
                    {
                        SpriteRenderer.sprite = value;
                        return;
                    }
                }
                Image.sprite=value;
            }
        }
        private SpriteRenderer _SpriteRenderer;
        private SpriteRenderer SpriteRenderer
        {
            get {
                if (_SpriteRenderer == null) _SpriteRenderer = GetComponent<SpriteRenderer>();
                if (_SpriteRenderer == null)
                    _IsCanvas = true;
                return _SpriteRenderer;
            }
        }
        private Image _Image;
        private Image Image
        {
            get
            {
                if (_Image == null) _Image = GetComponent<Image>();
                return _Image;
            }
        }
        private Rigidbody2D _Rigidbody2D;
        private Rigidbody2D Rigidbody2D
        {
            get
            {
                if (_Rigidbody2D == null)
                    _Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                return _Rigidbody2D;
            }
        }
        private bool _Active = false;
        public void Activate()
        {
            Sprite = SpriteActiveUp;
            _Active = true;
        }
        public void Deactivate()
        {
            Sprite = SpriteUp;
            _Active = false;
        }
        private void OnEnable()
        {
            Setup();
        }
        protected void Setup()
        {
            Sprite = (!_Active)?SpriteUp:SpriteActiveUp;
            if (Type.Equals(Types.SinglePush))
            {
                _GoingDown = new Action(() =>
                {
                    if (_ISetTouchHandled != null)
                    {
                        _ISetTouchHandled.SetTouchHandled();
                    }
                    Sprite = SpriteDown;
                    if (_GoneDown != null)
                    {
                        _GoneDown();
                    }
                });
                _GoingUp = new Action(() =>
                {
                    Sprite = SpriteUp;
                    if (_GoneUp != null)
                    {
                        _GoneUp();
                    }
                });
            }
            else
            {
                _GoingDown = new Action(() =>
                {
                    if(_ISetTouchHandled!=null)
                        _ISetTouchHandled.SetTouchHandled();
                    if (!_Active)
                    {
                        if (SpriteDown != null)
                        {
                            Sprite = SpriteDown;
                        }
                    }
                    else
                    {
                        if (SpriteActiveDown != null)
                        {
                            Sprite = SpriteActiveDown;
                        }
                    }
                });
                _GoingUp = new Action(() =>
                {
                    if (!_Active)
                    {
                        if (_GoneDown != null)
                        {
                            _GoneDown();
                        }
                        Activate();
                    }
                    else
                    {
                        if (_GoneUp != null)
                        {
                            _GoneUp();
                        }
                        Deactivate();
                    }
                });
            }
        }
        private void Start()
        {
            Lookup.AddBody(gameObject, this);
            Setup();
        }
        public void SetDelegates(Action goneDown, Action goneUp)
        {
            _GoneDown = goneDown;
            _GoneUp = goneUp;
        }
        public void Swipe(SwipingInfo swipingInfo)
        {

        }
        public void Touch(TouchInfo touchInfo)
        {
            Debug.Log(touchInfo.WasHit);
            if (!_Paused)
                {
                _GoingDown(); }
        }
        public void TouchEnded(Vector2 position)
        {if(!_Paused)
            _GoingUp();
        }
        public Relocator GetRelocator()
        {
            return new Scripts.Relocator(Rigidbody2D);
        }
        public void Pause() {
            _Paused = true;
        }
        public void Unpause()
        {
            _Paused = false;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_Paused)
            {
                _GoingDown();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
           if (!_Paused)
              _GoingUp();
        }
        public void SetInterface<T>(T o)
        {
            if (typeof(ISetTouchHandled).Equals(typeof(T)))
                _ISetTouchHandled = (ISetTouchHandled)o;
        }
    }
}
