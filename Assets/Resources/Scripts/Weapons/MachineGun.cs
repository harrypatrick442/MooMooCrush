using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class MachineGun : MonoBehaviour, ITouchable, ILooperFixedUpdate
    {
        private const float MAX_X_ANGLE = 10;
        private const float MIN_X_ANGLE = -50;
        private const float MAX_Y_ANGLE = 45;
        private const float MIN_Y_ANGLE = -45;
        private bool touching = false;
        private ILooper _ILooper;
        private ITouchSensor _ITouchSensor;
        private IGhosts _IGhosts;
        private class GunComponent
        {
            public IAudioPlayer IAudioPlayer
            {
                set
                {
                    _IAudioPlayer = value;
                }
            }
            private GameObject gameObject;
            private Action<IGhost> _GoGhost;
            class HitInfo
            {
                public  float firstHitAt;
                public HitInfo()
                {
                    firstHitAt = Time.time;
                }
            }
            private Dictionary<IMachineGunable, HitInfo> mapIGunnableToHitInfo = new Dictionary<IMachineGunable, HitInfo>();
            public GunComponent(GameObject gameObject, Action<IGhost> goGhost)
            {
                this.gameObject = gameObject;
                _GoGhost = goGhost;
            }
            private bool shooting = false;
            private float ANGULAR_VELOCITY_END = 2f;
            private Vector2? _FingerPosition;
            private GameObject _Model;
            private GameObject Model
            {
                get
                {
                    if (_Model == null) _Model = gameObject.transform.Find("Model").gameObject;
                    return _Model;
                }
            }
            private ParticleSystem[] _ParticleSystemBulletsis;
            private ParticleSystem[] ParticleSystemBulletsis
            {
                get
                {
                    if (_ParticleSystemBulletsis == null) _ParticleSystemBulletsis = gameObject.GetComponentsInChildren<ParticleSystem>();
                    return _ParticleSystemBulletsis;
                }
            }
            private GameObject _UpperBody;
            private GameObject UpperBody
            {
                get
                {
                    if (_UpperBody == null)
                        _UpperBody = Model.transform.Find("UpperBody").gameObject;
                    return _UpperBody;
                }
            }
            private GameObject _Target;
            private GameObject Target
            {
                get
                {
                    if (_Target == null)
                        _Target = gameObject.transform.Find("target").gameObject;
                    return _Target;
                }
            }
            private GameObject _Gun;
            private GameObject Gun
            {
                get
                {
                    if (_Gun == null)
                        _Gun = UpperBody.transform.Find("Gun").gameObject;
                    return _Gun;
                }
            }
            private GameObject _RotatingEnd;
            private GameObject RotatingEnd
            {
                get
                {
                    if (_RotatingEnd == null)
                        _RotatingEnd = Gun.transform.Find("Model").gameObject.transform.Find("RotatingEnd").gameObject;
                    return _RotatingEnd;
                }
            }
            private GameObject _HandlesMarker;
            private GameObject HandlesMarker
            {
                get
                {
                    if (_HandlesMarker == null)
                        _HandlesMarker = Gun.transform.Find("Model").gameObject.transform.Find("HandlesMarker").gameObject;
                    return _HandlesMarker;
                }
            }
            private GameObject _CentreMassMarker;
            private GameObject CentreMassMarker
            {
                get
                {
                    if (_CentreMassMarker == null)
                        _CentreMassMarker = Gun.transform.Find("Model").gameObject.transform.Find("CentreMassMarker").gameObject;
                    return _CentreMassMarker;
                }
            }
            private GameObject _EndMarker;
            private GameObject EndMarker
            {
                get
                {
                    if (_EndMarker == null)
                        _EndMarker = Gun.transform.Find("Model").gameObject.transform.Find("EndMarker").gameObject;
                    return _EndMarker;
                }
            }
            private Transform _UpperBodyTransform;
            private Transform UpperBodyTransform
            {
                get
                {
                    if (_UpperBodyTransform == null)
                    {
                        _UpperBodyTransform = UpperBody.GetComponent<Transform>();
                    }
                    return _UpperBodyTransform;
                }
            }
            private Transform _GunTransform;
            private Transform GunTransform
            {
                get
                {
                    if (_GunTransform == null)
                    {
                        _GunTransform = Gun.GetComponent<Transform>();
                    }
                    return _GunTransform;
                }
            }
            private Transform _RotatingEndTransform;
            private Transform RotatingEndTransform
            {
                get
                {
                    if (_RotatingEndTransform == null)
                    {
                        _RotatingEndTransform = RotatingEnd.GetComponent<Transform>();
                    }
                    return _RotatingEndTransform;
                }
            }
            private float UpperBodyAngle
            {
                get
                {
                    return UpperBodyTransform.localEulerAngles.y;
                }
                set
                {
                    UpperBodyTransform.localEulerAngles = new Vector3(UpperBodyTransform.localEulerAngles.x, value, UpperBodyTransform.localEulerAngles.z);
                }
            }
            private float GunAngle
            {
                get
                {
                    return GunTransform.localEulerAngles.x;
                }
                set { GunTransform.eulerAngles = new Vector3(value, GunTransform.eulerAngles.y, GunTransform.eulerAngles.z); }
            }
            private float LengthHandles
            {
                get { return (HandlesMarker.transform.position - CentreMassMarker.transform.position).magnitude; }
            }
            private float RotatingEndAngle
            {
                get
                {
                    return RotatingEndTransform.localEulerAngles.z;
                }
                set { RotatingEndTransform.localEulerAngles = new Vector3(RotatingEndTransform.localEulerAngles.x, RotatingEndTransform.localEulerAngles.y, value); }
            }
            private enum Modes { Resting, Activating, Deactivating, Active }
            private Modes Mode;
            private void Start()
            {

            }
			private PlayingSoundEffectHandle _PlayingSoundEffectHandle;
			private IAudioPlayer _IAudioPlayer;
            private void StopShooting()
            {
				
				if(_PlayingSoundEffectHandle!=null){
				_PlayingSoundEffectHandle.Dispose();
				_PlayingSoundEffectHandle=null;}
                foreach (ParticleSystem particleSystem in ParticleSystemBulletsis)
                {
                    particleSystem.Stop();
                }
            }
            private void StartShooting()
            {
				SoundEffectInfo soundEffectInfo = SoundEffects.MachineGun.Running;
				_PlayingSoundEffectHandle = _IAudioPlayer.Play(soundEffectInfo.Name, soundEffectInfo.Volume, true, true);
                foreach (ParticleSystem particleSystem in ParticleSystemBulletsis)
                    particleSystem.Play();
            }
            public void Activate()
            {
                SetMode(Modes.Activating);
            }
            public void Deactivate()
            {
                SetMode(Modes.Deactivating);
            }
            public bool LooperFixedUpdate()
            {
                bool remove = true;
                if (!Mode.Equals(Modes.Resting))
                {
                    if (Mode.Equals(Modes.Activating))
                    {
                        remove = false;
                        _Activating();
                    }
                    else
                    {
                        if (Mode.Equals(Modes.Active))
                        {
                            remove = _Active();
                        }
                        else
                        {
                            if (Mode.Equals(Modes.Deactivating))
                            {
                                remove = _Deactivating();
                            }
                        }
                    }
                }
                bool r = DoFinished();
                return remove && r;
            }
            private void SetMode(Modes mode)
            {
                switch (mode)
                {
                    case Modes.Active:
                        if (!Mode.Equals(Modes.Active))
                            StartShooting();
                        break;
                    default:
                            StopShooting();
                        break;
                }
                Mode = mode;
            }
            private void _Activating()
            {
                SetMode(Modes.Active);
            }
            private Vector2 GetPointHit()
            {
                Vector3 direction = CentreMassMarker.transform.position - HandlesMarker.transform.position;
                float a = -EndMarker.transform.position.z / direction.z;
                float x = a * direction.x;
                float y = a * direction.y;
                 return new Vector2(x + EndMarker.transform.position.x, y + EndMarker.transform.position.y);
                
            }
            private void DoHits(Vector2 point)
            {
               point = new Vector2(point.x + Random.Range(-0.07f, 0.07f), point.y + UnityEngine.Random.Range(-0.07f, 0.07f));
                RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
                if (hit)
                    {
                        IMachineGunable iMachineGunable = Lookup.GetBody<IMachineGunable>(hit.collider.gameObject);
                    if (iMachineGunable != null)
                    {
                            if (!mapIGunnableToHitInfo.ContainsKey(iMachineGunable))
                                mapIGunnableToHitInfo.Add(iMachineGunable, new HitInfo()); iMachineGunable.OnSlaughterStart();
                        iMachineGunable.MachineGun(point);
                    }
                }
            }
            private bool DoFinished()
            {
                List<IMachineGunable> removes = new List<IMachineGunable>();
                foreach(IMachineGunable iMachineGunable in mapIGunnableToHitInfo.Keys)
                {
                    if (mapIGunnableToHitInfo[iMachineGunable].firstHitAt + 1.5f < Time.time)
                    {
                        removes.Add(iMachineGunable);
                    }
                }
                foreach(IMachineGunable iMachineGunable in removes)
                {
                    _GoGhost(iMachineGunable);
                    iMachineGunable.DoneMachineGun();
                    mapIGunnableToHitInfo.Remove(iMachineGunable);
                }
                return (mapIGunnableToHitInfo.Keys.Count < 1);
            }
            private bool _Active()
            {
                UpdateRotatingEndRotation();
                Vector2 point = GetPointHit();
                DrawTarget(point);
                DoHits(point);
                return false;
            }
            private bool _Deactivating()
            {
                Target.SetActive(false);
                return false;
            }
            private void UpdateRotationY(Vector2 position)
            {
                float oOH = (UpperBodyTransform.position.x - position.x) / LengthHandles;
                float sign = Mathf.Sign(oOH);
                if (oOH * sign > 1)
                    oOH = 1 * sign;
                float a= 180 * Mathf.Asin(oOH) / Mathf.PI; ;
                UpperBodyAngle = a < MIN_Y_ANGLE ? MIN_Y_ANGLE : (a > MAX_Y_ANGLE ? MAX_Y_ANGLE : a);
            }
            private void UpdateRotationX(Vector2 position)
            {
                float oOH = (position.y - CentreMassMarker.transform.position.y) / LengthHandles;
                float sign = Mathf.Sign(oOH);
                if (oOH * sign > 1)
                    oOH = 1 * sign;

                float a  = 180 * Mathf.Asin(oOH) / Mathf.PI;
                GunAngle = a < MIN_X_ANGLE ? MIN_X_ANGLE:(a>MAX_X_ANGLE? MAX_X_ANGLE:a);
            }
            private void UpdateRotatingEndRotation()
            {
                float angle = RotatingEndTransform.localEulerAngles.z + (ANGULAR_VELOCITY_END / Time.fixedDeltaTime);
                if (angle > 360)
                    angle = 0;
                RotatingEndTransform.localEulerAngles = new Vector3(RotatingEndTransform.localEulerAngles.x, RotatingEndTransform.localEulerAngles.y, angle);
            }
            private void DrawTarget(Vector2 point)
            {
                Target.SetActive(true);
                Target.transform.position = new Vector3(point.x, point.y, -6);
            }
            private void UpdatePosition(Vector2 position)
            {
                UpdateRotationY(position);
                UpdateRotationX(position);
            }
            public void Swipe(SwipingInfo swipingInfo)
            {
                    if (swipingInfo.Type.Equals(SwipingInfo.Types.Traveled))
                {
                    SetMode(Modes.Active);
                    _FingerPosition = swipingInfo.Position;
                    UpdatePosition(swipingInfo.Position);
                }
            }
            public void Touch(TouchInfo touchInfo)
            {
                    _FingerPosition = touchInfo.Position;
                    Activate();
                    UpdatePosition(touchInfo.Position);
            }
            public void TouchEnded(Vector2 position)
            {
                _FingerPosition = null;
                Deactivate();
            }
        }
        private class TrollyComponent
        {
            private GameObject gameObject;
            private const float LEFT_SPEED = 3.3f;
            private const float RIGHT_SPEED = 3.3f;
            public TrollyComponent(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }
            private Transform _Transform;
            private Transform Transform
            {
                get
                {
                    if (_Transform == null)
                    {
                        _Transform = gameObject.transform.Find("Model").gameObject.transform;
                    }
                    return _Transform;
                }
            }
            private Vector3 Position
            {
                get
                {
                    
                    return Transform.position;
                }
                set
                {
                    Transform.position = value;

                }
            }
            private enum Modes { Resting, Activating, Deactivating, Active }
            private Modes Mode;
            public void Activate()
            {
                SetMode(Modes.Activating);
            }
            public void Deactivate()
            {
                SetMode(Modes.Deactivating);
            }
            public bool LooperFixedUpdate()
            {
                if (!Mode.Equals(Modes.Resting))
                {
                    if (Mode.Equals(Modes.Activating))
                    {
                        _Activating();
                        return false;
                    }
                    else
                    {
                        if (Mode.Equals(Modes.Active))
                        {
                            return true;
                        }
                        else
                        {
                            if (Mode.Equals(Modes.Deactivating))
                            {
                                _Deactivating();
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            private void SetMode(Modes mode)
            {
                switch (mode)
                {
                    case Modes.Active:
                        break;
                    default:
                        break;
                }
                Mode = mode;
            }
            private void _Activating()
            {
                if (Position.x > 0)
                {
                    Position = new Vector3(Position.x - (LEFT_SPEED*Time.fixedDeltaTime), Position.y, Position.z);
                }
                else
                {
                    Position = new Vector3(0, Position.y, Position.z);
                    SetMode(Modes.Active);
                }
            }
            private void _Deactivating()
            {
                if (Position.x < 3.5f)
                {
                    Position = new Vector3(Position.x + (RIGHT_SPEED*Time.fixedDeltaTime), Position.y, Position.z);
                }
                else
                {
                    SetMode(Modes.Resting);
                }
            }
        }
        private TrollyComponent _TrollyComponent;
        private GunComponent __GunComponent;
        private GunComponent _GunComponent
        {
            get
            {
                if (__GunComponent == null)
                {
                    __GunComponent = new GunComponent(gameObject, (IGhost iGhost) => { if (_IGhosts != null) _IGhosts.Add(iGhost); });
                }
                return __GunComponent;
            }
        }

		private bool _DoMachineGunTouch = false;
        private void Awake()
        {
            _TrollyComponent = new TrollyComponent(gameObject);
        }
        public void Activate()
        {
            _DoMachineGunTouch = true;
            _ILooper.AddFixedUpdate(this);
            _TrollyComponent.Activate();
            _ITouchSensor.AddTouchable(  TouchPriority . Weapon, this);

        }
        public void Deactivate()
        {
            _DoMachineGunTouch = false;
            _GunComponent.Deactivate();
            _ILooper.AddFixedUpdate(this);
            _TrollyComponent.Deactivate();
            _ITouchSensor.RemoveTouchable(this);
        }
        private bool _Handled = false;
        public void Touch(TouchInfo touchInfo)
        {
            if (!touchInfo.Handled)
            {
                _Handled = false;
                touchInfo.SetHandled();
                if (_DoMachineGunTouch)
                    _GunComponent.Touch(touchInfo);
                touching = true;
                _ILooper.AddFixedUpdate(this);
            }
            else
                _Handled = true;
        }
        public void TouchEnded(Vector2 position)
        {
            if (!_Handled)
            {
                if (_DoMachineGunTouch)
                    _GunComponent.TouchEnded(position);
                touching = false;
            }
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
            
            if (typeof(IGhosts).IsAssignableFrom(type))
                _IGhosts = (IGhosts)o;
            if (typeof(ITouchSensor).IsAssignableFrom(type))
                _ITouchSensor = (ITouchSensor)o;
            if (typeof(IAudioPlayer).IsAssignableFrom(type))
               _GunComponent.IAudioPlayer = (IAudioPlayer)o;
        }
        public void Swipe(SwipingInfo swipingInfo)
        {
            if (!_Handled)
            {
                if (_DoMachineGunTouch)
                    _GunComponent.Swipe(swipingInfo);
            }
        }
        public bool LooperFixedUpdate()
        {
            bool a = _GunComponent.LooperFixedUpdate();
            bool b = _TrollyComponent.LooperFixedUpdate();
            return a && b;
        }
    }
}
