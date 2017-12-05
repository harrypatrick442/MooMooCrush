using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class GamePlayMenu : MonoBehaviour, ITouchable, IMenuPickWeaponSlot
    {
        private ControlButton ControlButtonTest;
        private IMenuShowing _IMenuShowing;
        private IMenuHidden _IMenuHidden;
        private ILooper _ILooper;
        private IPause _IPause;
        private Pauser _Pauser = new Pauser();
        private INonVolatileData _INonVolatileData;
        private IShowEncyclopedia _IShowEncyclopedia;
        private bool _Showing = false;
        private bool _GotButtons = false;
        private enum States { Normal, WeaponPosition }
        private States _State = States.Normal;
        private States State
        {
            get
            {
                return _State;
            }
            set
            {
                switch (_State)
                {
                    case States.Normal:
                        break;
                    case States.WeaponPosition:
                        break;
                }
                _State = value;
            }
        }
        private ControlButton _ControlButtonClose;
        private ControlButton ControlButtonClose
        {
            get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonClose;
            }
        }
        private ControlButton _ControlButtonEncyclopedia;
        private ControlButton ControlButtonEncyclopedia
        {
            get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonEncyclopedia;
            }
        }
        private ControlButton _ControlButtonCrusher;
        private ControlButton ControlButtonCrusher { get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonCrusher;
            }
        }
        private ControlButton _ControlButtonBurner;
        private ControlButton ControlButtonBurner
        {
            get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonBurner;
            }
        }
        private ControlButton _ControlButtonRitualKnife;
        private ControlButton ControlButtonRitualKnife
        {
            get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonRitualKnife;
            }
        }
        private ControlButton _ControlButtonPhaser;
        private ControlButton ControlButtonPhaser
        {
            get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonPhaser;
            }
        }
        private ControlButton _ControlButtonMachineGun;
        private ControlButton ControlButtonMachineGun
        {
            get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonMachineGun;
            }
        }
        private ControlButton _ControlButtonRPG;
        private ControlButton ControlButtonRPG
        {
            get
            {
                if (!_GotButtons) GetControlButtons();
                return _ControlButtonRPG;
            }
        }
        private void GetControlButtons()
        {
            foreach (ControlButton controlButton in GetComponentsInChildren<ControlButton>())
            {
                _Pauser.Add(controlButton);
                switch (controlButton.name)
                {
                    case "button_menu_close":
                        _ControlButtonClose = controlButton;
                        _ControlButtonClose.SetDelegates(() => { }, () => { Hide(); });
                        break;
                    case "button_encyclopedia":
                        _ControlButtonEncyclopedia = controlButton;
                        _ControlButtonEncyclopedia.SetDelegates(() => { }, () => { _IShowEncyclopedia.ShowEncyclopedia(); });
                        break;
                    case "toggle_crusher":
                        _ControlButtonCrusher = controlButton;
                        _ControlButtonCrusher.SetDelegates(() => { }, ()=> { });
                        break;
                    case "toggle_phaser":
                        _ControlButtonPhaser = controlButton;
                        _ControlButtonPhaser.SetDelegates(() => { WeaponsSelectionControllerInstance.SetWeapon(Strings.PHASER); }, () => { WeaponsSelectionControllerInstance.RemoveWeapon(Strings.PHASER); });
                        break;
                    case "toggle_ritual_knife":
                        _ControlButtonRitualKnife = controlButton;
                        _ControlButtonRitualKnife.SetDelegates(() => { WeaponsSelectionControllerInstance.SetWeapon(Strings.RITUAL_KNIFE); }, () => { WeaponsSelectionControllerInstance.RemoveWeapon(Strings.RITUAL_KNIFE); });
                        break;
                    case "toggle_machine_gun":
                        _ControlButtonMachineGun = controlButton;
                        _ControlButtonMachineGun.SetDelegates(() => { WeaponsSelectionControllerInstance.SetWeapon(Strings.MACHINE_GUN); }, () => { WeaponsSelectionControllerInstance.RemoveWeapon(Strings.MACHINE_GUN); });
                        break;
                    case "toggle_burner":
                        _ControlButtonBurner = controlButton;
                        _ControlButtonBurner.SetDelegates(() => { WeaponsSelectionControllerInstance.SetWeapon(Strings.BURNER); }, () => { WeaponsSelectionControllerInstance.RemoveWeapon(Strings.BURNER); });
                        break;
                    case "toggle_rpg":
                        _ControlButtonRPG = controlButton;
                        _ControlButtonRPG.SetDelegates(() => { WeaponsSelectionControllerInstance.SetWeapon(Strings.RPG); }, () => { WeaponsSelectionControllerInstance.RemoveWeapon(Strings.RPG); });
                        break;
                }
            }
            _GotButtons = true;
        }
        private MenuPickWeaponSlot _MenuPickWeaponSlot;
        private MenuPickWeaponSlot MenuPickWeaponSlot
        {
            get
            {
                if (_MenuPickWeaponSlot == null)
                {
                    _MenuPickWeaponSlot = GetComponentInChildren<MenuPickWeaponSlot>(true);
                    _MenuPickWeaponSlot.SetInterface(this);
                }

                return _MenuPickWeaponSlot;
            }
        }
        private WeaponsSelectionController _WeaponsSelectionController;
        private WeaponsSelectionController WeaponsSelectionControllerInstance
        {
            get
            {
                if (_WeaponsSelectionController == null)
                    _WeaponsSelectionController = new WeaponsSelectionController(_Pauser, MenuPickWeaponSlot, new Tuple<string, ControlButton>[] { new Tuple<string, ControlButton>(Strings.RITUAL_KNIFE, ControlButtonRitualKnife ), new Tuple<string, ControlButton>(Strings.MACHINE_GUN, ControlButtonMachineGun), new Tuple<string, ControlButton>(Strings.BURNER, ControlButtonBurner), new Tuple<string, ControlButton>(Strings.PHASER, ControlButtonPhaser) });
                return _WeaponsSelectionController;
            }
        }

        public void Show()
        {
            if (!_Showing)
            {
                gameObject.SetActive(true);
                if (_IMenuShowing != null)
                    _IMenuShowing.MenuShowing();
                _Showing = true;
                _IPause.Pause();
            }
        }
        public void Hide()
        {
            if (_Showing)
            {
                gameObject.SetActive(false);
                _Showing = false;
                if (_IMenuHidden != null)
                    _IMenuHidden.MenuHidden();
                _IPause.Unpause();
            }
        }
        private void Start()
        {

        }
        public void Swipe(SwipingInfo swipingInfo)
        {

        }
        public void Touch(TouchInfo touchInfo)
        {
            if(State.Equals(States.WeaponPosition))
                MenuPickWeaponSlot.Touch(touchInfo);
        }
        public void TouchEnded(Vector2 positoin)
        {

        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ITouchSensor).IsAssignableFrom(type))
                ((ITouchSensor)o).AddTouchable(TouchPriority.UI1, this);
            if (typeof(IMenuShowing).IsAssignableFrom(type))
                _IMenuShowing = (IMenuShowing)o;
            if (typeof(IMenuHidden).IsAssignableFrom(type))
                _IMenuHidden = (IMenuHidden)o;
            if (typeof(IPause).IsAssignableFrom(type))
                _IPause = (IPause)o;
            if (typeof(IShowEncyclopedia).IsAssignableFrom(type))
                _IShowEncyclopedia = (IShowEncyclopedia)o;
            if (typeof(INonVolatileData).IsAssignableFrom(type))
            {
                _INonVolatileData = (INonVolatileData)o;
                WeaponsSelectionControllerInstance.SetInterface(o);
            }
            if (typeof(ISetControlButtonConfigurable).IsAssignableFrom(type))
                WeaponsSelectionControllerInstance.SetInterface(o);
        }
        public void MenuPickWeaponSlotHidden()
        {
            State = States.Normal;
        }
        private class WeaponsSelectionController
        {
            private Pauser _Pauser;
            private MenuPickWeaponSlot _MenuPickWeaponSlot;
            private Dictionary<string, ControlButton> _NameToToggle = new Dictionary<string, ControlButton>();
            private ISetControlButtonConfigurable _ISetControlButtonConfigurable;
            private INonVolatileData _INonVolatileData;
            private string[] names = new string[4];
            public void SetNames(string[] names)
            {
                if(names!=null&&names.Length==4)
                this.names = names;
                foreach (string name in names)
                {
                    if (name != null)
                        if (_NameToToggle.ContainsKey(name))
                        {
                            _NameToToggle[name].Activate();
                        }
                }
            }
            public void SetWeapon(string name)
            {
                _Pauser.Pause();
                _MenuPickWeaponSlot.SetWeapons(names);
                _MenuPickWeaponSlot.Show((int index) =>
                {
                    if (_ISetControlButtonConfigurable != null)
                    {
                        string currentName = names[index];
                        if (currentName != null)
                        {
                            if (_NameToToggle.ContainsKey(currentName))
                                _NameToToggle[currentName].Deactivate();
                        }
                        names[index] = name;
                        _ISetControlButtonConfigurable.SetControlButtonsConfigurable(names);
                        _INonVolatileData["weapon_names"] = names;
                    }
                    _Pauser.Unpause();
                }, () =>
                {
                    if (_NameToToggle.ContainsKey(name))
                        _NameToToggle[name].Deactivate();
                    _Pauser.Unpause();
                });
            }
            public void RemoveWeapon(string name)
            {
                Debug.Log("RemoveWeapon: " + name);
                    for(int i=0; i<names.Length; i++)
                {
                    string nameString = names[i];
                    if (name.Equals(nameString)) names[i] = null;
                }
                    if (_ISetControlButtonConfigurable != null)
                    {
                        _ISetControlButtonConfigurable.SetControlButtonsConfigurable(names);
                }
                _INonVolatileData["weapon_names"] = names;
            }
            public WeaponsSelectionController(Pauser pauser, MenuPickWeaponSlot menuPickWeaponSlot, params Tuple<string, ControlButton>[] controlButtons)
            {

                _Pauser = pauser;
                _MenuPickWeaponSlot = menuPickWeaponSlot;
                foreach (Tuple<string, ControlButton> controlButtonTuple in controlButtons)
                {
                    if(!_NameToToggle.ContainsKey(controlButtonTuple.A))
                        _NameToToggle[controlButtonTuple.A] = controlButtonTuple.B;
                }
            }
            public void SetInterface(object o)
            {
                Type type = o.GetType();
                if (typeof(ISetControlButtonConfigurable).IsAssignableFrom(type))
                    _ISetControlButtonConfigurable = (ISetControlButtonConfigurable)o;
                if (typeof(INonVolatileData).IsAssignableFrom(type))
                {
                    _INonVolatileData = (INonVolatileData)o;
                    string[] names = (string[])_INonVolatileData["weapon_names"];
                    if (names != null)
                    {
                        _MenuPickWeaponSlot.SetWeapons(names);
                        SetNames(names);
                    }
                }
            }
        }
    }
}