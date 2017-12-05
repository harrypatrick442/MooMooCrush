using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Assets.Scripts
{
    public class MenuPickWeaponSlot : MonoBehaviour, ITouchable
    {
        private IMenuPickWeaponSlot _IMenuPickWeaponSlot;
        private Action<int> _Set;
        private Action _Cancel;
        private ControlButtonConfigurable[] _ControlButtons = new ControlButtonConfigurable[5];
        private ControlButton _ControlButtonClose;
        private Renderer _Renderer;
        private Renderer Renderer
        {
            get { if (_Renderer == null) _Renderer = GetComponent<Renderer>();
                return _Renderer;
            }
        }
        private Rect? _Bounds;
        private Rect Bounds
        {
            get
            {
                if (_Bounds == null)
                {
                    Vector3 size = Renderer.bounds.size;
                    _Bounds = new UnityEngine.Rect(gameObject.transform.position.x - size.x, gameObject.transform.position.y - (size.y), 2 * size.x, 2 * size.y);
                }
                return (Rect)_Bounds;
            }
        }
        private ControlButtonsConfigurableHandler  _ControlButtonsConfigurableHandler;
        private ControlButtonsConfigurableHandler ControlButtonsConfigurableHandler
        {
            get
            {
                if (_ControlButtonsConfigurableHandler == null)
                    _ControlButtonsConfigurableHandler = GetComponent<ControlButtonsConfigurableHandler>();
                return _ControlButtonsConfigurableHandler;
            }
        }
        private void Start()
        {
            foreach (ControlButton controlButton in GetComponentsInChildren<ControlButton>(true))
            {
                Debug.Log(controlButton.name);
                switch (controlButton.name)
                {
                    case "button_pick_weapon_slot_close":
                        controlButton.SetDelegates(null, () => {
                            _Cancel();
                            Hide();
                        });
                        _ControlButtonClose = controlButton;
                        break;
                }
            }
            foreach (ControlButtonConfigurable controlButton in GetComponentsInChildren<ControlButtonConfigurable>(true))
            {
                switch (controlButton.name)
                {
                    case "button_pick_weapon_slot_0":
                        controlButton.SetDelegates(null, () => {
                            _Set(0);
                            Hide();
                        });
                        _ControlButtons[0] = controlButton;
                        break;
                    case "button_pick_weapon_slot_3":
                        controlButton.SetDelegates(null, () => {
                            _Set(3);
                            Hide();
                        });
                        _ControlButtons[3] = controlButton;
                        break;
                    case "button_pick_weapon_slot_2":
                        controlButton.SetDelegates(null, () => {
                            _Set(2);
                            Hide();
                        });
                        _ControlButtons[2] = controlButton;
                        break;
                    case "button_pick_weapon_slot_1":
                        controlButton.SetDelegates(null, () => {
                            _Set(1);
                            Hide();
                        });
                        _ControlButtons[1] = controlButton;
                        break;
                }
            }
        }
        public void SetWeapons(string[] names)
        {
            if(names!=null)
            ControlButtonsConfigurableHandler.Set(names);
        }
        public void Show(Action<int> set, Action cancel)
        {
            _Set = set;
            _Cancel = cancel;
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
            if (_IMenuPickWeaponSlot != null)
                _IMenuPickWeaponSlot.MenuPickWeaponSlotHidden();
        }
        public void Swipe(SwipingInfo swipingInfo)
        {
        }
        public void Touch(TouchInfo touchInfo)
        {
            if(!Bounds.Contains(touchInfo.Position))
            {
                _Cancel();
                Hide();
            }
        }
        public void TouchEnded(Vector2 positoin)
        {

        }
        public void SetInterface(object o)
        {
            if (typeof(IMenuPickWeaponSlot).IsAssignableFrom(o.GetType()))
                _IMenuPickWeaponSlot = (IMenuPickWeaponSlot)o;
        }
    }
}