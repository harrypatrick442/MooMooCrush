using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

namespace Assets.Scripts
{
    public class ControlsLayoutController:MonoBehaviour, ITouchable
    {
        private RotationHandler _RotationHandler;
        private ITouchSensor _ITouchSensor;
        private bool _IsLandscape;
        private bool HideLeftRightButtons
        {
            get
            {
                return true;
            }
        }
        private AspectRatioFitter _AspectRatioFitter;
        private AspectRatioFitter AspectRatioFitter
        {
       get
            {
                if(_AspectRatioFitter==null)
                {
                    _AspectRatioFitter = transform.Find("Bottom").GetComponent<AspectRatioFitter>();
                }
                return _AspectRatioFitter;
            }
        }
        private GameObject _ButtonShowHideGameObject;
        private GameObject ButtonShowHideGameObject
        {
            get
            {
                if (_ButtonShowHideGameObject==null)
                {
                    _ButtonShowHideGameObject = transform.Find("Bottom").transform.Find("button_show_hide").gameObject;
                }
                return _ButtonShowHideGameObject;
            }
        }
        private ControlButton _ButtonShowHide;
        private ControlButton ButtonShowHide
        {
            get
            {
                if(_ButtonShowHide==null)
                {
                    _ButtonShowHide = ButtonShowHideGameObject.GetComponent<ControlButton>();
                }
                return _ButtonShowHide;
            }
        }
        private GameObject _GameObjectControlsLeft;
        private GameObject GameObjectControlsLeft
        {
            get
            {
                if (_GameObjectControlsLeft == null)
                    _GameObjectControlsLeft = transform.Find("Bottom").transform.Find("controls_left").gameObject;//.GetComponent<RectTransform>();
                return _GameObjectControlsLeft;
            }
        }
        private RectTransform _RectTransformControlsLeft;
        private RectTransform RectTransformControlsLeft
        {
            get
            {
                if (_RectTransformControlsLeft == null)
                    _RectTransformControlsLeft = GameObjectControlsLeft.GetComponent<RectTransform>();
                return _RectTransformControlsLeft;
            }
        }
        private GameObject _GameObjectControlsRight;
        private GameObject GameObjectControlsRight
        {
            get
            {
                if (_GameObjectControlsRight == null)
                    _GameObjectControlsRight = transform.Find("Bottom").transform.Find("controls_right").gameObject;//.GetComponent<RectTransform>();
                return _GameObjectControlsRight;
            }
        }
        private RectTransform _RectTransformControlsRight;
        private RectTransform RectTransformControlsRight
        {
            get
            {
                if (_RectTransformControlsRight == null)
                    _RectTransformControlsRight =GameObjectControlsRight.GetComponent<RectTransform>();
                return _RectTransformControlsRight;
            }
        }
        private RectTransform _RectTransformControlsButtonMenu;
        private RectTransform RectTransformControlsButtonMenu
        {
            get
            {
                if (_RectTransformControlsButtonMenu == null)
                    _RectTransformControlsButtonMenu = transform.Find("Bottom").transform.Find("button_menu").gameObject.GetComponent<RectTransform>();
                return _RectTransformControlsButtonMenu;
            }
        }
        private RectTransform _RectTransformControlsTrolly;
        private RectTransform RectTransformControlsTrolly
        {
            get
            {
                if (_RectTransformControlsTrolly == null)
                    _RectTransformControlsTrolly = transform.Find("Bottom").transform.Find("controls_trolly").gameObject.GetComponent<RectTransform>();
                return _RectTransformControlsTrolly;
            }
        }
        private RectTransform _RectTransformControlsButtonShowHide;
        private RectTransform RectTransformControlsButtonShowHide
        {
            get
            {
                if (_RectTransformControlsButtonShowHide == null)
                    _RectTransformControlsButtonShowHide = ButtonShowHideGameObject.GetComponent<RectTransform>();
                return _RectTransformControlsButtonShowHide;
            }
        }
        private void SetPortrait(bool hideLeftRightButtons)
        {
            _IsLandscape = false;
            float offset = -0.13f;
            if (hideLeftRightButtons)
            {
                GameObjectControlsRight.SetActive(false);
                GameObjectControlsLeft.SetActive(false);
                ButtonShowHideGameObject.SetActive(true);
            }
            else
            {
                GameObjectControlsRight.SetActive(true);
                GameObjectControlsLeft.SetActive(true);
                ButtonShowHideGameObject.SetActive(false);
                if (_ITouchSensor != null)
                    _ITouchSensor.AddTouchable(TouchPriority.Camera, this);
            }
            RectTransformControlsRight.anchorMin = new Vector2(0f, 0.34f+offset);
            RectTransformControlsLeft.anchorMax = new Vector2(1f, 1f+offset);
            RectTransformControlsRight.anchorMax = new Vector2(1f, 0.67f+offset);
            RectTransformControlsLeft.anchorMin = new Vector2(0f, 0.68f+offset);
            RectTransformControlsButtonMenu.anchorMax = new Vector2(1f, 0.2000f);
            RectTransformControlsButtonMenu.anchorMin = new Vector2(0.76f, 0f);
            RectTransformControlsTrolly.anchorMax = new Vector2(0.7f, 0.2f);
            RectTransformControlsTrolly.anchorMin = new Vector2(0.2f, 0f);
            AspectRatioFitter.aspectRatio = 1.4f;
        }
        private void SetLandscape()
        {
            _IsLandscape = true;
            RectTransformControlsRight.anchorMin = new Vector2(0.52f, 0.4f);
            RectTransformControlsLeft.anchorMax = new Vector2(0.47f, 1f);
            RectTransformControlsRight.anchorMax = new Vector2(1f, 1f);
            RectTransformControlsLeft.anchorMin = new Vector2(0f, 0.4f);
            RectTransformControlsButtonMenu.anchorMax = new Vector2(1f, 0.35f);
            RectTransformControlsButtonMenu.anchorMin = new Vector2(0.83f, 0f);
            RectTransformControlsTrolly.anchorMax = new Vector2(0.76f, 0.39f);
            RectTransformControlsTrolly.anchorMin = new Vector2(0.24f, 0f);
            GameObjectControlsRight.SetActive(true);
            GameObjectControlsLeft.SetActive(true);
            ButtonShowHideGameObject.SetActive(false);
            AspectRatioFitter.aspectRatio = 5.2f;
        }
        public void SetInterface<T>(T o)
        {
            if(typeof(IRotationSensor).IsAssignableFrom(typeof(T)))
            {
                _RotationHandler = new RotationHandler(Rotated);
                ((IRotationSensor)o).AddEventHandler(_RotationHandler);
            }
            if (typeof(ITouchSensor).IsAssignableFrom(typeof(T)))
            {
                _ITouchSensor = ((ITouchSensor)o);
            }
        }
        public void Rotated(RotatedEventArgs rotatedEventArgs)
        {
            if (rotatedEventArgs.IsLandscape)
                SetLandscape();
            else
                SetPortrait(true);
        }
        public void Start()
        {
            ButtonShowHide.SetDelegates(()=> {
            }, () => {
                    SetPortrait(false);
            });
            SetPortrait(true);
        }

        public void Swipe(SwipingInfo swipingInfo)
        {

        }

        public void Touch(TouchInfo touchInfo)
        {

        }

        public void TouchEnded(Vector2 positoin)
        {
            if (!_IsLandscape)
            {
                new Thread(() => {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => {
                        SetPortrait(true);
                    });
                }).Start();
            }
            _ITouchSensor.RemoveTouchable(this);
        }
    }
}