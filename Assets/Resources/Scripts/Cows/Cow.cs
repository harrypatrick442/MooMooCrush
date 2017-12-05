using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class Cow : ITouchable, IShove, ICowMonoBehaviou, IPhasable, IBurnable, IMachineGunable, IGhost, IExplodible, ICrushable, IDoorwayable, IKnifable, IPausible, ISquashable, IRPGAble, ITractableStalkable, IDisposable, IVibratable, ILaserable, IPopable, IFeedable

    {
        private IResourceHelper _InterfacesHelper;
        private ILooper _ILooper;
        private IExploder _IExploder;
        private IEntrailsHandler _IEntrailsHandler;
		private IAudioPlayer _IAudioPlayer;
		private IMessageBot _IMessageBot;
        private MonoBehaviour ParentMonoBehaviour;
        private MonoBehaviour MonoBehaviour;
        private DisposeCallbackHandler _DisposeCallbackHandler;
        private ParticleSystem[] _ParticleSystemKnifeSpills;
        private SquashInfo _SquashInfo = new SquashInfo();
        private SuperMooController _SuperMooController;
        private ParticleSystem _ParticleSystemSquash;
        private ParticleSystem ParticleSystemSquash
        {
            get
            {
                if (_ParticleSystemSquash == null)
                {
                    GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/squash"));
                    _ParticleSystemSquash = go.GetComponent<ParticleSystem>();
                    _ParticleSystemSquash.startColor = MilkshakeColor;
                    go.transform.parent = gameObject.transform;
                    go.transform.position = new Vector3(0, 0, gameObject.transform.position.z - 0.1f);
                }
                return _ParticleSystemSquash;
            }
        }
        private bool touched = false;
        private bool _Slaughtered = false;
        public bool sick = true;
        private bool _BeingSlaughtered = false;
        private string _TriggerString;
        private bool _Paused = false;
        private float _OriginalHeight;
        private WeakReference _TractableOwner;
        private WeakReference _StalkableOwner;
        private bool _TractableEnabled = true;
        private Vector2 ThroatPosition
        {
            get
            {
                switch (CowType)
                {

                    case Enums.CowType.Terror:
                    case Enums.CowType.Edible:
                        if (!FoodType.Equals(Enums.FoodType.Burger))
                            return new Vector2(0, -0.20f);
                        return new Vector2(0, -0.23f);
                    case Enums.CowType.Religious:
                        return new Vector2(0, -0.23f);
                    default:
                        return new Vector2(0, -0.2f);
                }
            }
        }
        private float RitualKnifeParticleSystemScale
        {
            get
            {
                switch (CowType)
                {

                    case Enums.CowType.Terror:
                    case Enums.CowType.Edible:
                        if (!FoodType.Equals(Enums.FoodType.Burger))
                            return 1.2f;
                        return 1f;
                    case Enums.CowType.Religious:
                        return 0.9f;
                    default:
                        return 1f;
                }
            }
        }

        public GameObject gameObject;
        private GameObject floorGameObject; private Bomb _Bomb;
        private Action<Cow> _OnSlaughter;
        private Enums.CowType _CowType;
        public Enums.CowType CowType
        {
            get {
				return _CowType;
			}
        }
		private void SetCowType(Enums.CowType cowType){
			_TriggerString = GetTrigger(FoodType, cowType);
			Animator.SetTrigger(_TriggerString);
			_CowType = cowType;
		}
        private Enums.FoodType _FoodType;
        public Enums.FoodType FoodType
        {
            set
            {
                _FoodType = value;
                _TriggerString = GetTrigger(value, CowType);
                Animator.SetTrigger(_TriggerString);
            }
            get
            {
                return _FoodType;
            }
        }
        private Enums.Health _Health;
        public Enums.Health Health
        {
            set
            {
                _Health = value;
                Animator.SetBool("sick", _Health.Equals(Enums.Health.Unwell));
            }
            get
            {
                return _Health;
            }
        }
        private Animator _Animator;
        private Animator Animator
        {
            get
            {
                if (_Animator == null)
                {
                    if (gameObject != null)
                        _Animator = gameObject.GetComponent<Animator>();
                }
                return _Animator;
            }
        }
        private Color? _MilkshakeColor;
        private Color MilkshakeColor
        {
            get
            {
                if (_MilkshakeColor == null)
                {
                    switch (FoodType)
                    {
                        case Enums.FoodType.Banana:
                            _MilkshakeColor = new Color(1, 0.8f, 0.412f);
                            break;
                        case Enums.FoodType.Strawberry:
                            _MilkshakeColor = new Color(0.925f, 0.6357f, 0.812f);
                            break;
                        case Enums.FoodType.Chocolate:
                            _MilkshakeColor = new Color(0.31f, 0.11f, 0f);
                            break;
                        default:
                            _MilkshakeColor = new Color(1, 0, 0);
                            break;
                    }
                }
                return (Color)_MilkshakeColor;
            }
        }
        private Rigidbody2D _Rigidbody2D;
        public Rigidbody2D Rigidbody2D
        {
            get
            {
                if (_Rigidbody2D == null)
                    if (gameObject != null)
                        _Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                return _Rigidbody2D;
            }
        }
        private ParticleSystem _BurningFire;
        private ParticleSystem BurningFire
        {
            get
            {
                if (_BurningFire == null)
                {
                    if (gameObject != null)
                        _BurningFire = gameObject.GetComponentInChildren<ParticleSystem>();
                }
                return _BurningFire;
            }
        }
        private CapsuleCollider2D _CapsuleCollider2D;
        private CapsuleCollider2D CapsuleCollider2D
        {
            get
            {
                if (_CapsuleCollider2D == null)
                    if (gameObject != null)
                        _CapsuleCollider2D = gameObject.GetComponent<CapsuleCollider2D>();
                return _CapsuleCollider2D;
            }
        }
        private Renderer _Renderer;
        private Renderer Renderer
        {
            get
            {
                if (_Renderer == null)
                {
                    if (gameObject != null)
                        _Renderer = gameObject.GetComponent<Renderer>();
                }
                return _Renderer;
            }
        }
        private Texture2D Texture2D
        {
            get
            {
                return (Texture2D)Renderer.material.mainTexture;
            }
        }
        private RectTransform _RectTransformRitualKnife;
        private RectTransform RectTransformRitualKnife
        {
            get
            {
                if (_RectTransformRitualKnife == null) _RectTransformRitualKnife = gameObject.transform.Find("knife_bounds").GetComponent<RectTransform>();
                return _RectTransformRitualKnife;
            }
        }
        private HealthBar _HealthBar;
        public HealthBar HealthBar
        {
            get
            {
                if (_HealthBar == null)
                    if (gameObject != null)
                        _HealthBar = gameObject.GetComponentInChildren<HealthBar>(true);
                return _HealthBar;
            }
        }
        public Cow(Vector3 position, MonoBehaviour parent, IResourceHelper interfacesHelper, Action<Cow> onSlaughter, CowInfo cowInfo)
        {
            _InterfacesHelper = interfacesHelper;
            _ILooper = interfacesHelper.Get<ILooper>();
            _IExploder = interfacesHelper.Get<IExploder>();
			_IMessageBot = interfacesHelper.Get<IMessageBot> ();
            _IEntrailsHandler = interfacesHelper.Get<IEntrailsHandler>();
            _DisposeCallbackHandler = new DisposeCallbackHandler(this);
			_IAudioPlayer = interfacesHelper.Get<IAudioPlayer>();
            ParentMonoBehaviour = parent;
            _OnSlaughter = onSlaughter;
            gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/cow"));
            gameObject.transform.parent = parent.transform;
            gameObject.transform.localPosition = position;
            ((CowMonoBehaviour)gameObject.GetComponent<MonoBehaviour>()).SetInterface(this);
            Health = cowInfo.Health;
            FoodType = cowInfo.FoodType;
			SetCowType (cowInfo.CowType);
            _OriginalHeight = gameObject.GetComponent<Renderer>().bounds.size.y * gameObject.transform.localScale.y;
            switch (cowInfo.CowType)
            {
                case Enums.CowType.Terror:
                    AddBomb(cowInfo.ExplositeType);
                    break;
                case Enums.CowType.SuperMoo:
                    _TractableEnabled = false;

                    break;
			case Enums.CowType.Devil:
				_TractableEnabled = false;
				_IsPhasable = false;
                    break;
            }
            Lookup.AddBody(gameObject, this);
            SetNotHovering();
        }
        private string GetTrigger(Enums.FoodType foodType, Enums.CowType cowType)
        {
            Debug.Log(foodType);
            switch (cowType)
            {
                case Enums.CowType.Edible:
                case Enums.CowType.Terror:
                    switch (foodType)
                    {
                        case Enums.FoodType.Banana:
                            return "cow_banana";
                        case Enums.FoodType.Chocolate:
                            return "cow_chocolate";
                        case Enums.FoodType.Strawberry:
                            Debug.Log("STRAWBERRYSTRAWBERRYSTRAWBERRY");
                            return "cow_strawberry";
                        default:
                            return "bull";
                    }
                case Enums.CowType.Religious:
                    return "religious";
                case Enums.CowType.SuperMoo:
                    return "super_moo";
                default:
                    return "bull_devil";
            }
        }
        private bool IsFlooring(Collision2D collision)
        {
            return collision.gameObject.transform.position.y < gameObject.transform.position.y;
        }
        #region ICowMonoBehaviour
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsFlooring(collision))
            {
                floorGameObject = collision.gameObject;
            }
            ISquashable iSquashable = Lookup.GetBody<ISquashable>(collision.gameObject);
            if (iSquashable != null)
                _SquashInfo.Add(iSquashable);
        }
        public void OnCollisionExit2D(Collision2D collision)
        {
            ISquashable iSquashable = Lookup.GetBody<ISquashable>(collision.gameObject);
            if (iSquashable != null)
                _SquashInfo.Remove(iSquashable);
        }
        public void OnDestroy()
        {
            Slaughter();
        }
        #endregion
        #region IShove
        public bool Shove(ShoveInfo shoveInfo)
        {
            if (_Paused)
                return false;
            if (gameObject == null)
                return true;
            float y = 0;
            if (!touched)
            {
                float distance = shoveInfo.FingerPosition - gameObject.transform.position.x;
                if ((shoveInfo.IsLeft && distance > 0) || (!shoveInfo.IsLeft && distance < 0))
                    return true;
                Rigidbody2D.velocity += new Vector2(200 * distance * shoveInfo.DT, y);

            }
            else
            {
                float toVel = 20f;
                float maxVel = 40.0f;
                float maxForce = 200.0f;
                float gain = 50f;

                Vector2 dist = new Vector2(shoveInfo.FingerPosition - gameObject.transform.position.x, 0);
                // calc a target vel proportional to distance (clamped to maxVel)
                Vector2 tgtVel = Vector2.ClampMagnitude(toVel * dist, maxVel);
                // calculate the velocity error
                Vector2 error = tgtVel - (Rigidbody2D.velocity);
                // calc a force proportional to the error (clamped to maxForce)
                Vector2 force = Vector2.ClampMagnitude(gain * error, maxForce);
                Rigidbody2D.AddForce(force);//.velocity+=new Vector2(dV, y);
                Rigidbody2D.velocity += new Vector2(0, y);
            }
            return false;
        }
        public bool IsShovable
        {
            get
            {
                return WeakCollectionKinematicOwners.Count < 1;
            }
        }
        #endregion
        #region ITouch
        public void Swipe(SwipingInfo swipingInfo)
        {
        }
        public void Touch(TouchInfo touchInfo)
        {
            touched = true;
        }
        public void TouchEnded(Vector2 position)
        {
            touched = false;
        }
        #endregion
        #region Slaughter
        public void Slaughter()
        {
            _DisposeCallbackHandler.Dispose();
            if (!_Slaughtered)
            {
                _OnSlaughter(this);
                if (_SuperMooController != null)
                    _SuperMooController.Dispose();
                Lookup.RemoveBody(gameObject);
                UnityEngine.Object.Destroy(gameObject);
                _Slaughtered = true;
            }
        }
        public void OnSlaughterStart()
        {
            _BeingSlaughtered = true;
            if (_Bomb != null)
            {
                _Bomb.Dissable();
            }
        }
        public bool IsBeingSlaughtered()
        {
            return _BeingSlaughtered;
        }
        #endregion
        #region IGetRect
        public Rect? GetRect()
        {
            if (gameObject != null)
            {
                Rect r = MyGeometry.GetRect(gameObject);
                return MyGeometry.GetRect(gameObject);
            }
            return null;
        }
        #endregion
        #region IPhase
		private bool _IsPhasable=true;
		public bool IsPhasable{get{if (!_IsPhasable) {
					if (_CowType.Equals (Enums.CowType.Devil))
						_IMessageBot.NotPhasable (Enums.CowType.Devil);
				}return _IsPhasable;}}

        public void Phase()
        {
            if (gameObject != null)
            {
                Animator.SetTrigger("phase");
            }
        }
        public void DonePhase()
        {
            Slaughter();
        }
        public bool IsPhased()
        {
            if (Animator != null)
            {
                AnimatorStateInfo animatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
                return animatorStateInfo.normalizedTime > animatorStateInfo.length && (animatorStateInfo.IsName("cow_phased") || animatorStateInfo.IsName("bull_phased") || animatorStateInfo.IsName("cow_religious_phased"));
            }
            return true;
        }
        #endregion
        #region IBurn
        public void Burn()
        {

			if (_Bomb != null&&_Bomb.ExplosiveType.Equals(Enums.ExplosiveType.TNT)) {
				_Bomb.Explode ();
			} else {
				SoundEffectInfo soundEffectInfo = SoundEffects.Moo.Hurting.Random;
				_IAudioPlayer.Play (soundEffectInfo.Name, soundEffectInfo.Volume);
				if (gameObject != null) {
					Animator.SetTrigger ("burn");
					if (BurningFire != null)
						BurningFire.Play ();
				}
			}
        }
        public void DoneBurn()
        {
            if (BurningFire != null)
                BurningFire.Stop();
            Slaughter();
        }
        public bool IsBurnt()
        {
            if (Animator != null)
            {
                AnimatorStateInfo animatorStateInfo = Animator.GetCurrentAnimatorStateInfo(0);
                return animatorStateInfo.normalizedTime > animatorStateInfo.length && (animatorStateInfo.IsName("cow_burning") || animatorStateInfo.IsName("bull_burning") || animatorStateInfo.IsName("cow_religious_burning"));
            }
            return true;
        }
        #endregion
        #region ICrushable
        public GameObject GetCrushParticleSystem()
        {
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/spill"));
            foreach (ParticleSystem particleSystem in go.GetComponentsInChildren<ParticleSystem>()) { particleSystem.startColor = MilkshakeColor; }
            return go;
        }
				private bool _DidCrushBloodSplatSound=false;
        public void Crush(CrushInfo crushInfo)
        {
            if (gameObject != null)
            {
                if (crushInfo.state.Equals(CrushInfo.States.Dropping))
                {
                    if (crushInfo.height < _OriginalHeight)
                    {
						if(!_DidCrushBloodSplatSound)
						{
						_DidCrushBloodSplatSound=true;
			SoundEffectInfo soundEffectInfo = SoundEffects.Blood.Splat;
			_PlayingSoundEffectHandleMachineGun = _IAudioPlayer.Play(soundEffectInfo.Name, soundEffectInfo.Volume, false, false);
						}
                        float newHeight = crushInfo.height / _OriginalHeight;
                        if (newHeight > 0.01)
                            gameObject.transform.localScale = new Vector3(1, newHeight, 1);
                        else
                        {
							gameObject.transform.localScale = new Vector3(1, 0.01f, 1);
                        }
                    }
					else{
						_DidCrushBloodSplatSound=false;
					}
                }
                else
                {
					gameObject.transform.localScale = new Vector3(1, 0.01f, 1);
					if (_Bomb != null&&_Bomb.ExplosiveType.Equals(Enums.ExplosiveType.TNT)) {
						_Bomb.Explode ();
					}
					_IMessageBot.YouCrushedTNT();
                }
            }
        }
        #endregion
        private void BulletWound(Vector2 position)
        {
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/bullet_wound"));
            go.transform.parent = gameObject.transform;
            go.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z - 6f);
        }
        private void Spray(Vector2 position)
        {
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/spray"));
            go.GetComponent<ParticleSystem>().startColor = MilkshakeColor;
            go.transform.parent = gameObject.transform;
            go.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z - 0.1f);
        }
        #region IMachineGun
		private PlayingSoundEffectHandle _PlayingSoundEffectHandleMachineGun;
        public void MachineGun(Vector2 position)
        {
			if(_PlayingSoundEffectHandleMachineGun==null||!_PlayingSoundEffectHandleMachineGun.IsAlive)
			{
			SoundEffectInfo soundEffectInfo = SoundEffects.Moo.Hurting.Random;
			_PlayingSoundEffectHandleMachineGun = _IAudioPlayer.Play(soundEffectInfo.Name, soundEffectInfo.Volume, false, false);
			}
            Texture2D texture = Texture2D;
            //Sprite = Sprite.Create(Texture2D, new Rect(0, 0, Texture2D.width, Texture2D.height), new Vector2(0, 0));
            BulletWound(position);
            Spray(position);
        }
        public void DoneMachineGun()
        {
            Slaughter();
        }
        #endregion
        #region IExplodible
        public void Explode(Vector2 point, float force)
        {
            Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Rigidbody2D.constraints = RigidbodyConstraints2D.None;
            Vector2 normalized = new Vector2(gameObject.transform.position.x - point.x, gameObject.transform.position.y - point.y).normalized;
            Rigidbody2D.AddForce(new Vector2(normalized.x * force, normalized.y * force));
            if (_IEntrailsHandler != null)
                _IEntrailsHandler.Scatter(point, typeof(CowEntrails));
            Slaughter();
        }
        #endregion
        #region IKnifable
        public void RitualKnife()
        {
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/knife_spill"));
            _ParticleSystemKnifeSpills = go.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in _ParticleSystemKnifeSpills)
            {
                ps.startColor = MilkshakeColor;
                ps.transform.localScale = new Vector3(1, 1, RitualKnifeParticleSystemScale);
            }
            go.transform.parent = gameObject.transform;
            Vector2 position = gameObject.transform.TransformPoint(ThroatPosition);
            go.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z - 6f);
        }
        public void DoneRitualKnife()
        {
            Slaughter();
        }
        public bool IsRitualKnifed()
        {
            if (_ParticleSystemKnifeSpills != null)
            {
                if (_ParticleSystemKnifeSpills.Length > 0)
                {
                    if (_ParticleSystemKnifeSpills[0] != null)
                    {
                        return !_ParticleSystemKnifeSpills[0].IsAlive();
                    }
                }
            }
            return true;
        }
        public bool IsInRitualKnifeBounds(Vector2 position)
        {
            return RectTransformRitualKnife.rect.Contains(RectTransformRitualKnife.InverseTransformPoint(position));
        }
        public Vector2 GetRitualKnifeSnapInPlacePosition()
        {
            return gameObject.transform.TransformPoint(ThroatPosition);
        }
        #endregion
        #region IGetPosition
        public Vector3? GetPosition3()
        {
            if (gameObject != null)
                return gameObject.transform.position;
            return null;
        }
        public Vector2? GetPosition2()
        {
            if (gameObject != null)
                return (Vector2)gameObject.transform.position;
            return null;
        }
        #endregion
        #region ISetPosition
        public void SetPosition(Vector2 position)
        {
            if (gameObject != null)
                gameObject.transform.position = position;
        }
        #endregion
        #region IGhost
        public GameObject GetGhostPrefab()
        {
            return (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/cow_ghost"));
        }
        #endregion
        private void AddBomb(Enums.ExplosiveType explosiveType)
        {
            _Bomb = new Bomb(gameObject.transform, _InterfacesHelper, explosiveType);
        }
        #region Pause
        public virtual void Pause()
        {
            _Paused = true;
            if (_Bomb != null)
            {
                _Bomb.Pause();
            }
        }
        public virtual void Unpause()
        {
            _Paused = false;
            if (_Bomb != null)
            {
                _Bomb.Unpause();
            }
        }
        #endregion
        #region ISquashable
        public SquashInfo GetSquashInfo()
        {
            return _SquashInfo;
        }
        public void Squash()
        {
            ParticleSystemSquash.Play();
        }
        public bool IsSquashed()
        {
            if (ParticleSystemSquash != null)
            {
                if (ParticleSystemSquash.duration > 0)
                    return !ParticleSystemSquash.IsAlive();
            }
            return true;
        }
        public void DoneSquash()
        {
            Slaughter();
        }
        #endregion
        #region IGetTransform
        public Transform GetTransform()
        {
            return gameObject.transform;
        }

        #endregion
        #region IISAlive
        public bool IsAlive
        {
            get
            {
                return !_Slaughtered;
            }
        }
        #endregion
        #region ITractableStalkable
        public void TakeTractable(WeakReference o)
        {
            _TractableOwner = o;
            _StalkableOwner = null;
        }
        public bool TractableIsTaken
        {
            get
            {
                if (_TractableOwner != null)
                    return _TractableOwner.IsAlive;
                return false;
            }
        }
        public bool TractableEnabled
        {
            get
            {
                return _TractableEnabled;
            }
        }
        public void ReleaseTractable()
        {
            _TractableOwner = null;
        }
        public bool StalkableIsTaken
        {
            get
            {
                if (_StalkableOwner != null)
                    if (_StalkableOwner.IsAlive)
                        return true;
                return TractableIsTaken;
            }
        }

        public bool IsBeingStalked
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void TakeStalkable(WeakReference stalker)
        {
            _StalkableOwner = stalker;
        }
        public void StopStalking()
        {
            _StalkableOwner = null;
        }
        #endregion
        #region IDisposingCallbackHandler

        public void AddBeingDisposedOfCallback(IDisposingCallback iDisposingCallback)
        {
            _DisposeCallbackHandler.AddBeingDisposedOfCallback(iDisposingCallback);

        }
        #endregion
        #region ISetBodyType
        private void SetBodyType(RigidbodyType2D type)
        {
            Rigidbody2D.bodyType = type;
            if (type.Equals(RigidbodyType2D.Kinematic))
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
            }
        }
        #endregion
        #region IGetBodyType
        public RigidbodyType2D GetDefaultBodyType()
        {
            return RigidbodyType2D.Dynamic;
        }
        #endregion
        #region IDispose
        public void Dispose()
        {
            Slaughter();
        }
        #endregion
        #region IMassless
        private float? _Mass;
        public void SetMassless(bool state)
        {
            if (state)
            {
                if (_Mass == null)
                    _Mass = Rigidbody2D.mass;
            }
            else
            {
                if (_Mass != null)
                    Rigidbody2D.mass = (float)_Mass;

            }
        }
        #endregion
        #region IBodyType
        private WeakCollection<object> _WeakCollectionKinematicOwners;
        private WeakCollection<object> WeakCollectionKinematicOwners
        {
            get
            {

                if (_WeakCollectionKinematicOwners == null)
                    _WeakCollectionKinematicOwners = new WeakCollection<object>();
                return _WeakCollectionKinematicOwners;
            }
        }

        public void TakeKinematic(object owner)
        {
            if (WeakCollectionKinematicOwners.Count < 1)
                SetBodyType(RigidbodyType2D.Kinematic);
            WeakCollectionKinematicOwners.Add(owner);
        }
        public void ReleaseKinematic(object owner)
        {
            WeakCollectionKinematicOwners.Remove(owner);
            if (WeakCollectionKinematicOwners.Count < 1)
            {
                SetBodyType(RigidbodyType2D.Dynamic);
            }
        }
        #endregion
        #region IVibratable
        public void Vibrate()
        {

        }
        #endregion
        #region ILasered
        private LaseredInfo _LaseredInfoLatest;
        public bool IsLasered
        {
            get
            {
                if (_LaseredInfoLatest != null)
                {
                    return _LaseredInfoLatest.IsDone;
                }
                return true;
            }
        }
        public void Lasered()
        {
            _LaseredInfoLatest = new LaseredInfo(this);
            _InterfacesHelper.Get<ILaseredHandler>().Lasered(_LaseredInfoLatest);
        }
        #endregion
        #region IPopable
        private PopInfo _PopInfo;
        public bool IsPopped
        {
            get
            {
                if (_PopInfo != null)
                {
                    return _PopInfo.IsDone;
                }
                return true;
            }
        }
        public void Pop(PopInfo _PopInfo)
        {
            _InterfacesHelper.Get<IPopHandler>().Pop(_PopInfo);
        }
        #endregion
        private void SetNotHovering()
        {
            if (Renderer == null) return;
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                Renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Outline", 0.0f);
                Renderer.SetPropertyBlock(mpb);
        }
        private bool _InitializedHoveringMaterialProperties = false;
        private void SetHovering()
        {
            if (Renderer == null) return;
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                Renderer.GetPropertyBlock(mpb);
            if (!_InitializedHoveringMaterialProperties)
            {
                mpb.SetColor("_OutlineColor", new Color(0.137f, 1f, 0.95f));
                _InitializedHoveringMaterialProperties = true;
            }
                mpb.SetFloat("_Outline", 1.0f);
                mpb.SetFloat("_OutlineSize", 1.0f);
                Renderer.SetPropertyBlock(mpb);
        }
        #region IFeedable 
        public bool FeedHover<T>(T iFeed) where T : IFeed
        {
            SetHovering();
            return true;
        }
        public void FeedUnHover()
        {
            SetNotHovering();
        }
        public bool Feed<T>(T iFeed) where T : IFeed
        {
			Debug.Log ("fed");
            if (typeof(Takeout).IsAssignableFrom(iFeed.GetType()))
            {
                HealthBar.Health.Percentage += 25;
                return true;
            }
            return false;
        }
        #endregion
    }
}