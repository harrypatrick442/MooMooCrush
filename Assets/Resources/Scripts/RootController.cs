using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class RootController : MonoBehaviour, ICrusher, IDoorway, IMenuShowing, IMenuHidden, IShowEncyclopedia, IDoneOpen<INonVolatileData>, ISetControlButtonConfigurable
    {
        private CameraConfiguration _CameraConfigurationNormal;
        private CameraConfiguration CameraConfigurationNormal
        {
            get
            {
                if (_CameraConfigurationNormal == null)
                {
                    _CameraConfigurationNormal = new CameraConfiguration(MonoBehaviourHelper.GetRectFromMarkers(gameObject.transform, new string[] { "Markers", "Normal" }), CameraBoundsFit.Best, true, 1.000f, CameraSwiperDirections.All, 1.6f);
                }
                return _CameraConfigurationNormal;
            }
        }
        private CameraConfiguration _CameraConfigurationEncyclopedia;
        private CameraConfiguration CameraConfigurationEncyclopedia
        {
            get
            {
                if (_CameraConfigurationEncyclopedia == null)
                {
                    _CameraConfigurationEncyclopedia = new CameraConfiguration(MonoBehaviourHelper.GetRectFromMarkers(gameObject.transform, new string[] { "Markers", "Encyclopedia" }), CameraBoundsFit.Best, true, 1f, CameraSwiperDirections.All);
                }
                return _CameraConfigurationEncyclopedia;
            }
        }
        private TeleportationSimulationHandler _TeleportationSimulationHandler;
        private DifficultyController _DifficultyController;
        private Vibrator _Vibrator;
        private Display _Display;
        private BurnHandler _BurnHandler;
        private PopHandler _PopHandler;
        private IFightsController _FightsController;
        private Markers _Markers;
        private Camera MainCamera;
        private CameraController CameraController;
        private ControlsLayoutController _ControlsLayoutController;
        private RotationSensor RotationSensor;
        private TouchSensor TouchSensor;
        private ResourcesHelper _ResourcesHelper = new ResourcesHelper();
        private RootLooper _RootLooper = new RootLooper();
        private RelocationHandler _RelocationHandler = new RelocationHandler();
        private LaseredHandler _LaseredHandler;
        private GamePlayMenu GamePlayMenu;
        private MenuSwipeUpDown MenuSwipeUpDown;
        private AlertFeedSuperMooCow AlertFeedSuperMooCow;
        private Pauser _Pauser = new Pauser();
        private Pauser _PauserAll = new Pauser();
        private ControlButtonsConfigurableHandler _ControlButtonConfigurableHandler;
        private Shove<Cow> Shove;
        private Trolly Trolly;
        private Cows Cows;
        private UFOs UFOs;
        private Crusher Crusher;
        private SlaughterPlatform SlaughterPlatform;
        private Doorway[] Doorways;
        private SquashSourceSink[] SquashSourceSinks;
        private SquashHandler SquashHandler;
        private Doorway DoorwayHeaven;
        private Doorway DoorwayHell;
        private ControlButton ControlButtonLeft;
        private ControlButton ControlButtonRight;
        private ControlButton ControlButtonCrush;
        private ControlButton ControlButtonPhase;
        private ControlButtonConfigurable ControlButtonConfigurableChoice0;
        private ControlButtonConfigurable ControlButtonConfigurableChoice1;
        private ControlButtonConfigurable ControlButtonConfigurableChoice2;
        private ControlButtonConfigurable ControlButtonConfigurableChoice3;
        private ControlButtonConfiguration ControlButtonConfigurationCrusher;
        private ControlButtonConfiguration ControlButtonConfigurationBurner;
        private ControlButtonConfiguration ControlButtonConfigurationMachineGun;
        private ControlButtonConfiguration ControlButtonConfigurationPhaser;
        private ControlButtonConfiguration ControlButtonConfigurationRitualKnife;
        private ControlButton ControlButtonBurn;
        private ControlButton ControlButtonConveyorSwitchRight;
        private ControlButton ControlButtonConveyorSwitchLeft;
        private ControlButton ControlButtonMenu;
        private ControlButton ControlButtonStrawberry;
        private ControlButton ControlButtonBanana;
        private ControlButton ControlButtonChocolate;
        private ControlButton ControlButtonBurger;
        private Encyclopedia Encyclopedia;
        private Conveyor ConveyorLhsMiddle;
        private Conveyor ConveyorRhsMiddle;
        private Conveyor ConveyorLhsTop;
        private Conveyor ConveyorRhsTop;
        private Conveyor ConveyorLhsBottom;
        private Conveyor ConveyorRhsBottom;
        private GhostHandler GhostHandler;
        private Phaser Phaser;
        private Burner Burner;
        private MachineGun MachineGun;
        private RPG RPG;
        private SpawnerCows SpawnerCows;
        private SpawnerUFOs SpawnerUFOs;
        private Exploder Exploder;
        private RitualKnife RitualKnife;
        private PlayGames PlayGames;
        private ControlButton _ControlButtonCurrent;
        private EntrailsHandler _EntrailsHandler;
        private LoadingBar _LoadingBar;
        private Moosics Moosics;
		private ScoreMilkshakes _ScoreMilkshakes;
        private enum Modes { Normal, Phaser, Burner, RitualKnife, MachineGun, Menu, Encyclopedia, RPG };
        private Modes Mode = Modes.Normal;
		private AudioPlayer _AudioPlayer;
		private ConveyorSounds _ConveyorSounds;
		private Bin Bin;
		private MessageBot _MessageBot;
        private void Awake()
        {
            //Lookup.Clear();
        }
        private void SetMode(Modes newMode)
        {
            if (newMode != Mode)
            {
                ControlButton button = null;
                switch (Mode)
                {

                    case Modes.Phaser:
                        Phaser.Deactivate();
                        button = _ControlButtonConfigurableHandler.GetButton(Strings.PHASER);
                        if (button != null) button.Deactivate();
                        break;
                    case Modes.RitualKnife:
                        RitualKnife.Deactivate();
                        button = _ControlButtonConfigurableHandler.GetButton(Strings.RITUAL_KNIFE);
                        if (button != null) button.Deactivate();
                        break;
                    case Modes.Burner:
                        Burner.Deactivate();
                        button = _ControlButtonConfigurableHandler.GetButton(Strings.BURNER);
                        if (button != null) button.Deactivate();
                        break;
                    case Modes.MachineGun:
                        MachineGun.Deactivate();
                        button = _ControlButtonConfigurableHandler.GetButton(Strings.MACHINE_GUN);
                        if (button != null) button.Deactivate();
                        break;
                    case Modes.RPG:
                        RPG.Deactivate();
                        button = _ControlButtonConfigurableHandler.GetButton(Strings.RPG);
                        if (button != null) button.Deactivate();
                        break;
                    case Modes.Menu:
                        GamePlayMenu.Hide();
                        break;
                    case Modes.Encyclopedia:
                        Encyclopedia.Hide();
                        CameraController.SetConfiguration(CameraConfigurationNormal);
                        Debug.Log("set configuration normal");
                        break;
                    case Modes.Normal:
                        break;
                }
                Mode = newMode;
                switch (Mode)
                {
                    case Modes.Phaser:
                        Phaser.Activate();
                        CameraController.SetConfiguration(CameraConfigurationNormal);
                        break;
                    case Modes.RitualKnife:
                        RitualKnife.Activate();
                        CameraController.SetConfiguration(CameraConfigurationNormal);
                        break;
                    case Modes.Burner:
                        Burner.Activate();
                        CameraController.SetConfiguration(CameraConfigurationNormal);
                        break;
                    case Modes.MachineGun:
                        MachineGun.Activate();
                        CameraController.SetConfiguration(CameraConfigurationNormal);
                        break;
                    case Modes.Menu:
                        GamePlayMenu.Show();
                        break;
                    case Modes.Encyclopedia:
                        CameraController.SetConfiguration(CameraConfigurationEncyclopedia);
                        Encyclopedia.Open(3);
                        break;
                    case Modes.RPG:
                        RPG.Activate();
                        break;
                    default:
                        CameraController.SetConfiguration(CameraConfigurationNormal);
                        break;
                }
            }
        }
        private void Start()
        {
            MyGeometry.GetPointInRectForLineLengthX(new Rect(1/*1*/, 5, 1, 1), new Vector2(0, 0),4.5f);
            #region Pickup MonoBehaviours
            _Display = gameObject.GetComponentInChildren<Display>();
            _Markers = new Markers(gameObject);
            MainCamera = gameObject.GetComponentInChildren<Camera>();
            TouchSensor = new TouchSensor(TouchPriority.List, MainCamera);
            Shove = new Shove<Cow>(TouchSensor);
            _Vibrator = new Vibrator(_RootLooper);
            _PopHandler = new PopHandler(_Vibrator, _RootLooper, Exploder);
            _BurnHandler = new BurnHandler(_RootLooper);
            _TeleportationSimulationHandler = gameObject.GetComponentInChildren<TeleportationSimulationHandler>(true);
            _LaseredHandler = new LaseredHandler(_BurnHandler, _RootLooper, Exploder);
            List<FightControllerUseInfo> listFightControllerUseInfo = new List<FightControllerUseInfo>{
                LaserEyesController.UserInfo
            };
            _DifficultyController = new DifficultyController(_RootLooper);
            listFightControllerUseInfo.AddRange(SonicMooController.UserInfo);
            _FightsController = new FightsController(new ControllerPicker(listFightControllerUseInfo, _ResourcesHelper), _RootLooper);
            RotationSensor = gameObject.GetComponentInChildren<RotationSensor>(true);
            _ControlsLayoutController = gameObject.GetComponentInChildren<ControlsLayoutController>();
            CameraController = gameObject.GetComponentInChildren<CameraController>(true);
            _ControlButtonConfigurableHandler = gameObject.transform.Find("Controls").transform.GetComponentInChildren<ControlButtonsConfigurableHandler>(true);
            Trolly = gameObject.GetComponentInChildren<Trolly>(true);
            Cows = gameObject.GetComponentInChildren<Cows>(true);
            UFOs = gameObject.GetComponentInChildren<UFOs>(true);
			Bin = gameObject.GetComponentInChildren<Bin>(true);
            SlaughterPlatform = gameObject.GetComponentInChildren<SlaughterPlatform>(true);
            Crusher = gameObject.GetComponentInChildren<Crusher>(true);
            Doorways = gameObject.GetComponentsInChildren<Doorway>(true);
            GhostHandler = gameObject.GetComponentInChildren<GhostHandler>();
            Phaser = gameObject.GetComponentInChildren<Phaser>(true);
            Burner = gameObject.GetComponentInChildren<Burner>(true);
            MachineGun = gameObject.GetComponentInChildren<MachineGun>(true);
            RPG = gameObject.GetComponentInChildren<RPG>(true);
            RitualKnife = gameObject.GetComponentInChildren<RitualKnife>(true);
            Exploder = gameObject.GetComponentInChildren<Exploder>(true);
            Conveyor[] conveyors = GetComponentsInChildren<Conveyor>(true);
            Encyclopedia = GetComponentInChildren<Encyclopedia>(true);
            GamePlayMenu = GetComponentInChildren<GamePlayMenu>(true);
            MenuSwipeUpDown = GetComponentInChildren<MenuSwipeUpDown>(true);
            AlertFeedSuperMooCow = GetComponentInChildren<AlertFeedSuperMooCow>(true);
            SpawnerCows = new SpawnerCows(Cows, _Markers.SuperMooSpawnSites, _Markers.DevilCowSpawnSites);
            SpawnerUFOs = new SpawnerUFOs(UFOs);
            SquashSourceSinks = GetComponentsInChildren<SquashSourceSink>(true);
            _EntrailsHandler = GetComponentInChildren<EntrailsHandler>(true);
            Moosics = GetComponentInChildren<Moosics>(true);
            _LoadingBar = GetComponentInChildren<LoadingBar>(true);
            SquashHandler = new SquashHandler();
			_MessageBot = new MessageBot( _Display, _RootLooper);
            PlayGames = PlayGames.Instance;
            RPG.SetInterface(Exploder);
			_ScoreMilkshakes= gameObject.GetComponentInChildren<ScoreMilkshakes>(true);
			_AudioPlayer = new AudioPlayer(_RootLooper, gameObject.transform.Find("AudioSources").gameObject, 12, "Audio/Clips");
            foreach (Conveyor conveyor in conveyors)
            {
                _Pauser.Add(conveyor);
                switch (conveyor.name)
                {
                    case Strings.CONVEYOR_LHS_BOTTOM:
                        ConveyorLhsBottom = conveyor;
                        break;
                    case Strings.CONVEYOR_RHS_BOTTOM:
                        ConveyorRhsBottom = conveyor;
                        break;
                }
            }
            foreach (Doorway doorway in Doorways)
            {
                if (doorway.name.Equals(Strings.MOO_MOO_HEAVEN))
                    DoorwayHeaven = doorway;
                else
                    DoorwayHell = doorway;
            }
            #endregion
            #region prepare RelocationHandler
            _RelocationHandler.Add(ConveyorLhsBottom);
            _RelocationHandler.Add(ConveyorRhsBottom);
            _RelocationHandler.Add(ConveyorLhsTop);
            _RelocationHandler.Add(ConveyorRhsTop);
            _RelocationHandler.Add(ConveyorLhsMiddle);
            _RelocationHandler.Add(ConveyorRhsMiddle);
            _RelocationHandler.Add(Trolly);
            _RelocationHandler.Add(Crusher);
            _RelocationHandler.Add(DoorwayHeaven);
            _RelocationHandler.Add(DoorwayHell);
            #endregion
            #region Prepare ResourcesHelper
            _ResourcesHelper.Add<IExploder>(Exploder);
            _ResourcesHelper.Add<IBurnHandler>(_BurnHandler);
            _ResourcesHelper.Add<ILooper>(_RootLooper);
            _ResourcesHelper.Add<IPause>(_Pauser);
            _ResourcesHelper.Add<IRelocator>(_RelocationHandler);
            _ResourcesHelper.Add<IEntrailsHandler>(_EntrailsHandler);
            _ResourcesHelper.Add<ISuperMooMarkers>(_Markers);
            _ResourcesHelper.Add<IDevilCowMarkers>(_Markers);
            _ResourcesHelper.Add<IFightsController>(_FightsController);
            _ResourcesHelper.Add<ILaseredHandler>(_LaseredHandler);
            _ResourcesHelper.Add<IShowAlertFeedSuperMooCow>(AlertFeedSuperMooCow);
            _ResourcesHelper.Add<IPopHandler>(_PopHandler);
            _ResourcesHelper.Add<IMoosics>(Moosics);
            _ResourcesHelper.Add<IVibrator>(_Vibrator);
            _ResourcesHelper.Add<IAudioPlayer>(_AudioPlayer);
            _ResourcesHelper.Add<ITeleportationSimulationHandler>(_TeleportationSimulationHandler);
			_ResourcesHelper.Add<IMessageBot>(_MessageBot);
            #endregion
            #region Sets
            _TeleportationSimulationHandler.SetInterface<ILooper>(_RootLooper);
            Moosics.SetInterface<ILooper>(_RootLooper);
            RotationSensor.SetInterface(_RootLooper);
            CameraController.SetInterface(_RootLooper);
            CameraController.SetInterface(TouchSensor);
            CameraController.SetInterface(RotationSensor);
            _ControlsLayoutController.SetInterface<IRotationSensor>(RotationSensor);
            _ControlsLayoutController.SetInterface<TouchSensor>(TouchSensor);
            _ControlsLayoutController.SetInterface<TouchSensor>(TouchSensor);
            DoorwayHeaven.SetInterface(this);
            DoorwayHell.SetInterface(this);
            Cows.SetInterface(_ResourcesHelper);
            UFOs.SetInterface(_ResourcesHelper);
            UFOs.SetInterface(CameraController);
            GhostHandler.SetPositions(DoorwayHeaven, DoorwayHell);
            Phaser.SetInterface(this);
            Phaser.SetInterface(_AudioPlayer);
            Crusher.SetInterface(SlaughterPlatform);
            Crusher.SetInterface(_AudioPlayer);
            Crusher.SetInterface(this);
            Burner.SetInterface(this);
            Burner.SetInterface(TouchSensor);
            Burner.SetInterface(_BurnHandler);
            MachineGun.SetInterface(this);
            MachineGun.SetInterface(_AudioPlayer);
            RPG.SetInterface(_RootLooper);
            RPG.SetInterface(TouchSensor);
            GhostHandler.SetInterface(_RootLooper);
            Phaser.SetInterface(_RootLooper);
            Phaser.SetInterface(TouchSensor);
            Crusher.SetInterface(_RootLooper);
            Burner.SetInterface(_RootLooper);
            MachineGun.SetInterface(_RootLooper);
            MachineGun.SetInterface(GhostHandler);
            MachineGun.SetInterface(TouchSensor);
            RitualKnife.SetInterface(_RootLooper);
            RitualKnife.SetInterface(TouchSensor);
            GamePlayMenu.SetInterface(this);
            GamePlayMenu.SetInterface(_Pauser);
            GamePlayMenu.SetInterface(_RootLooper);
            GamePlayMenu.SetInterface(TouchSensor);
            MenuSwipeUpDown.SetInterface(_Pauser);
            MenuSwipeUpDown.SetInterface(TouchSensor);
            AlertFeedSuperMooCow.SetInterface(_Pauser);
            AlertFeedSuperMooCow.SetInterface(TouchSensor);
            UFOs.SetInterface(_ResourcesHelper);
            UFOs.SetInterface<MenuSwipeUpDown>(MenuSwipeUpDown);
            RitualKnife.SetInterface(GhostHandler);
            Exploder.SetInterfaces(_RootLooper);
            Encyclopedia.SetInterface(_RootLooper);
            Encyclopedia.SetInterface(_Pauser);
            Encyclopedia.SetInterface(TouchSensor);
            Encyclopedia.OnClose = new Action(() => { Debug.Log("doing on close"); SetMode(Modes.Normal); });
            _DifficultyController.Add(SpawnerCows);
            _EntrailsHandler.SetInterfaces(_RootLooper);
			_ScoreMilkshakes.SetInterface<TouchSensor>(TouchSensor);
			_ScoreMilkshakes.SetInterface<RootLooper>(_RootLooper);
			_ScoreMilkshakes.SetInterface<ISetTouchHandled>(TouchSensor);
			_ConveyorSounds = new ConveyorSounds(_AudioPlayer);
            foreach (SquashSourceSink squashSourceSink in SquashSourceSinks)
            {
                SquashHandler.Add(squashSourceSink);
                squashSourceSink.SetInterface(SquashHandler);
                squashSourceSink.SetInterface(_RootLooper);
            }
			Bin.SetInterface<ILooper>(_RootLooper);
            _Pauser.Add(Cows);
            _Pauser.Add(UFOs);
            _Pauser.Add(SpawnerCows);
            _Pauser.Add(SpawnerUFOs);
            _Pauser.Add(_FightsController);
			_Pauser.Add(_ConveyorSounds);
            #endregion
            #region Controls
            foreach (ControlButton controlButton in gameObject.GetComponentsInChildren<ControlButton>())
            {
                switch (controlButton.name)
                {
                    case Strings.BUTTON_CRUSH:
                        _Pauser.Add(controlButton);
                        ControlButtonCrush = controlButton;
                        controlButton.SetDelegates(() =>
                        {
                            Crusher.Crush();
                        }, null);
                        break;
                    case Strings.BUTTON_MENU:
                        _PauserAll.Add(controlButton);
                        ControlButtonMenu = controlButton;
                        controlButton.SetDelegates(() => { }, () => { SetMode(Modes.Menu); });
                        break;
                    case Strings.BUTTON_SWITCH_LHS:
                        _Pauser.Add(controlButton);

                        ControlButtonConveyorSwitchLeft = controlButton;
                        controlButton.SetDelegates(() =>
                        {
                            ConveyorLhsBottom.Direction = Conveyor.Directions.Left;
                        }, () =>
                        {
                            ConveyorLhsBottom.Direction = Conveyor.Directions.Right;
                        });
                        break;
                    case Strings.BUTTON_SWITCH_RHS:
                        _Pauser.Add(controlButton);
                        ControlButtonConveyorSwitchRight = controlButton;
                        controlButton.SetDelegates(() =>
                        {
                            ConveyorRhsBottom.Direction = Conveyor.Directions.Right;
                        }, () =>
                        {
                            ConveyorRhsBottom.Direction = Conveyor.Directions.Left;
                        });
                        break;
                    case Strings.BUTTON_STRAWBERRY:
                        _Pauser.Add(controlButton);
                        ControlButtonStrawberry = controlButton;
                        controlButton.SetDelegates(() => { Trolly.FoodType = Enums.FoodType.Strawberry; }, () => { });
                        break;
                    case Strings.BUTTON_BANANA:
                        _Pauser.Add(controlButton);
                        ControlButtonBanana = controlButton;
                        controlButton.SetDelegates(() => { Trolly.FoodType = Enums.FoodType.Banana; }, () => { });
                        break;
                    case Strings.BUTTON_CHOCOLATE:
                        _Pauser.Add(controlButton);
                        ControlButtonChocolate = controlButton;
                        controlButton.SetDelegates(() => { Trolly.FoodType = Enums.FoodType.Chocolate; }, () => { });
                        break;
                    case Strings.BUTTON_BURGER:
                        _Pauser.Add(controlButton);
                        ControlButtonBurger = controlButton;
                        controlButton.SetDelegates(() => { Trolly.FoodType = Enums.FoodType.Burger; }, () => { });
                        break;
                }
                controlButton.SetInterface<ISetTouchHandled>(TouchSensor);
            }
            foreach (ControlButtonConfigurable controlButton in gameObject.GetComponentsInChildren<ControlButtonConfigurable>())
            {
                switch (controlButton.name)
                {
                    case Strings.BUTTON_CHOICE_0:
                        _Pauser.Add(controlButton);
                        ControlButtonConfigurableChoice0 = controlButton;
                        break;
                    case Strings.BUTTON_CHOICE_1:
                        _Pauser.Add(controlButton);
                        ControlButtonConfigurableChoice1 = controlButton;
                        break;
                    case Strings.BUTTON_CHOICE_2:
                        _Pauser.Add(controlButton);
                        ControlButtonConfigurableChoice2 = controlButton;
                        break;
                    case Strings.BUTTON_CHOICE_3:
                        _Pauser.Add(controlButton);
                        ControlButtonConfigurableChoice3 = controlButton;
                        break;
                }
                controlButton.SetInterface<ISetTouchHandled>(TouchSensor);
            }
            _ControlButtonConfigurableHandler.SetCallbacks(Strings.MACHINE_GUN, () => { SetMode(Modes.MachineGun); }, () => { SetMode(Modes.Normal); });
            _ControlButtonConfigurableHandler.SetCallbacks(Strings.RITUAL_KNIFE, () => { SetMode(Modes.RitualKnife); }, () => { SetMode(Modes.Normal); });
            _ControlButtonConfigurableHandler.SetCallbacks(Strings.PHASER, () => { SetMode(Modes.Phaser); }, () => { SetMode(Modes.Normal); });
            _ControlButtonConfigurableHandler.SetCallbacks(Strings.BURNER, () => { SetMode(Modes.Burner); }, () => { SetMode(Modes.Normal); });
            _ControlButtonConfigurableHandler.SetCallbacks(Strings.RPG, () => { SetMode(Modes.RPG); }, () => { SetMode(Modes.Normal); });
            _ControlButtonConfigurableHandler.SetCallbacks(Strings.DEFAULT, () => { }, () => { Debug.Log("default"); SetMode(Modes.Menu); });
            #endregion
            _PauserAll.AddParent(_Pauser);
            _PauserAll.Pause();
            CameraController.SetConfiguration(CameraConfigurationNormal);
            PlayGames.Open("file_0", this);
            _Display.Queue(new MovingText("Welcome to MooMoo-Crush!", 0.8f, _RootLooper));
        }
        public void ICrushablesCrushed(List<ICrushable> iCrushables)
        {
            foreach (ICrushable iCrushable in iCrushables)
            {
                if (typeof(Cow).IsAssignableFrom(iCrushable.GetType()))
                {
                    Cow cow = (Cow)iCrushable;
					if (FoodItem.WillAcceptIngredient(Trolly.CurrentFoodType, cow)){
						_MessageBot.Crushed (iCrushable, true);
						Trolly.Completeness++;
					} else {
						Bin.Trash (new TrashableFoodItem (gameObject, Trolly.CurrentFoodItemPosition, Trolly.CurrentFoodType, Trolly.Completeness));
						Trolly.Completeness = 0;
						_MessageBot.Crushed (iCrushable, false);
					}
                    cow.Slaughter();
                }
            }
        }
        public void ExitedThroughDoorway(Enums.DoorwayType type, IDoorwayable iDoorwayable)
        {
            if (iDoorwayable.GetType().Equals(typeof(Cow)))
                ((Cow)iDoorwayable).Slaughter();

        }
        public void MenuHidden()
        {
            SetMode(Modes.Normal);
        }
        public void MenuShowing()
        {

        }
        public void SetControlButtonsConfigurable(string[] names)
        {
            string nam = "";
            foreach (string name in names)
            {
                nam += name;
                nam += ",";
            }
            _ControlButtonConfigurableHandler.Set(names);
        }
        public void ShowEncyclopedia()
        {
            SetMode(Modes.Encyclopedia);
        }
        public void DoneOpen(OpenDoneInfo<INonVolatileData> openDoneInfo)
        {
            if (openDoneInfo.Successful)
            {
                INonVolatileData iNonVolatileData = openDoneInfo.Data;

                string[] names = null; try { names = (string[])iNonVolatileData["weapon_names"]; }
                catch (Exception ex)
                {
                    Debug.Log("Initializing weapon_names failed: " + ex);
                }
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (names != null)
                        SetControlButtonsConfigurable(names);
                    AlertFeedSuperMooCow.SetInterface(iNonVolatileData);
                    GamePlayMenu.SetInterface(iNonVolatileData);
                    UFOs.SetInterface(iNonVolatileData);
                    _LoadingBar.Hide();
                    _PauserAll.Unpause();
                });
            }
        }
        private void Update()
        {
            _RootLooper.Update();
        }
        private void FixedUpdate()
        {
            TouchSensor.Run(Time.fixedDeltaTime);
            _RootLooper.FixedUpdate();
        }
    }
}
