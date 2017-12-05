using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class RPG : MonoBehaviour, ITouchable, ILooperFixedUpdate
    {
        private const float MAX_X_ANGLE = 10;
        private const float MIN_X_ANGLE = -50;
        private const float MAX_Y_ANGLE = 45;
        private const float RELOAD_TIME = 4;
        private const float MIN_Y_ANGLE = -45;
        private const float ROCKET_SPEED = 20;
        private bool touching = false;
        private ITouchSensor _ITouchSensor;
        private ILooper _ILooper;
        private IExploder _IExploder;
        private IGhosts _IGhosts;
        private Action<IGhost> _GoGhost;
        private List<RocketInfo> rocketInfos = new List<RocketInfo>();
        private bool _Loaded = true;
        private float _BeganReloading;
        public class RocketInfo
        {
            public enum States {Traveling, Explode, Exploding, Done}
            public States State = States.Traveling;
            private const float BLAST_RADIUS = 0.5f;
            private Vector3 _Direction;
            public GameObject _GameObjectRocket;
            private GameObject _GameObjectExplosion;
            private ParticleSystem _ParticleSystemExplosion;
            private Action<Vector2> _Explode;
            //List<IRPGAble> _Spectators = new List<IRPGAble>();
            public RocketInfo(Vector3 direction, GameObject gameObject, Action<Vector2> explode)
            {
                _Direction = direction;
                _GameObjectRocket = gameObject;
                _Explode = explode;
            }
            private void ExplodeNearby(Vector2 point)
            {
                int angle = 0;
                List<IRPGAble> _Spectators = new List<IRPGAble>();
                while (angle < 359)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(point, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), BLAST_RADIUS);
                    foreach (RaycastHit2D hit in hits)
                        if (hit)
                        {
                            IRPGAble iRPGAble = Lookup.GetBody<IRPGAble>(hit.collider.gameObject);
                            if (iRPGAble != null)
                            {
                                if (!_Spectators.Contains(iRPGAble))
                                {
                                    _Spectators.Add(iRPGAble);
                                    iRPGAble.Explode(point, 100);
                                }
                            }
                        }
                    angle += 10;
                }
            }
            private void Explode()
            {
                RaycastHit2D hit = Physics2D.Raycast((Vector2)_GameObjectRocket.transform.position, Vector2.zero);
                if (hit)
                {
                    IRPGAble iRPGAble = Lookup.GetBody<IRPGAble>(hit.collider.gameObject);
                    if (iRPGAble != null)
                    {
                        _Explode(_GameObjectRocket.transform.position);
                        ExplodeNearby(_GameObjectRocket.transform.position);
                    }
                }
                UnityEngine.Object.Destroy(_GameObjectRocket);
                State = States.Exploding;
            }
            private bool IsFinishedExploding()
            {
                if (_ParticleSystemExplosion != null)
                {
                            return !_ParticleSystemExplosion.IsAlive();
                }
                return true;
            }
            private void Exploding()
            {
                if(IsFinishedExploding())
                {
                    State = States.Done;
                }
            }
            public bool Tick()
            {
                switch (State)
                {
                    case States.Traveling:

                        float dT = Time.fixedDeltaTime;
                        _GameObjectRocket.transform.position = new Vector3(_GameObjectRocket.transform.position.x + (dT * _Direction.x * ROCKET_SPEED), _GameObjectRocket.transform.position.y + (dT * _Direction.y * ROCKET_SPEED), _GameObjectRocket.transform.position.z + (dT * _Direction.z * ROCKET_SPEED));
                        if(_GameObjectRocket.transform.position.z>-1.5)
                        {
                            State = States.Explode;
                        }
                        return false;
                    case States.Explode:
                        Explode();
                        return false;
                    case States.Exploding:
                        Exploding();
                        return false;
                    case States.Done:
                        return true;
                }
                return true;
            }
        }
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
        private ParticleSystem _Smoke;
        private ParticleSystem Smoke
        {
            get
            {
                if (_Smoke == null) _Smoke = Gun.transform.Find("smoke").gameObject.GetComponent<ParticleSystem>();
                return _Smoke;
            }
        }
        private GameObject _Gun;
        private GameObject Gun
        {
            get
            {
                if (_Gun == null)
                    _Gun = gameObject.transform.Find("Gun").gameObject;
                return _Gun;
            }
        }
        private GameObject _RocketStatic;
        private GameObject RocketStatic
        {
            get
            {
                if (_RocketStatic == null)
                    _RocketStatic = Gun.transform.Find("Rocket").gameObject;
                return _RocketStatic;
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
        private float GunAngle
        {
            get
            {
                return GunTransform.localEulerAngles.x;
            }
            set { GunTransform.eulerAngles = new Vector3(value, GunTransform.eulerAngles.y, GunTransform.eulerAngles.z); }
       }
        private void Start()
        {

        }
        public void Activate()
        {
            gameObject.SetActive(true);
            _ITouchSensor.AddTouchable(TouchPriority.Weapon, this);
        }
        public void Deactivate()
        {
            gameObject.SetActive(false);
            _ITouchSensor.RemoveTouchable(this);
        }
        public bool LooperFixedUpdate()
        {
            int i = 0;
            int count = rocketInfos.Count;
            while(i<count)
            {
                RocketInfo rocketInfo = rocketInfos[i];
                if(rocketInfo.Tick())
                {
                    count--;
                    rocketInfos.Remove(rocketInfo);
                }
                else
                {
                    i++;
                }
            }
            bool remove = rocketInfos.Count < 1;
            if(!_Loaded)
            {
                if (Time.time - _BeganReloading > RELOAD_TIME)
                {
                    _Loaded = true;RocketStatic.SetActive(true);
                }
                else
                    remove = false;
            }
            return remove;
        }
        private void UpdateRotation(Vector2 position)
        {
            float dz = GunTransform.position.z + 1;
            float dx = GunTransform.position.x - position.x;
            float dy = position.y - GunTransform.position.y;
            float xAngle = 180f * Mathf.Atan(dy / dz) / Mathf.PI; ;
            float yAngle = 180f*Mathf.Atan(dx / dz)/Mathf.PI;
            GunTransform.localEulerAngles = new Vector3(xAngle, yAngle, 0);
        }
        private void Launch(Vector2 position)
        {
            RocketStatic.SetActive
                (false);
            float elevation = Mathf.PI*GunTransform.eulerAngles.x/180;
            float heading = Mathf.PI*GunTransform.eulerAngles.y / 180f;
            Vector3 direction = new Vector3(Mathf.Cos(elevation) * Mathf.Sin(heading), -Mathf.Sin(elevation), Mathf.Cos(elevation) * Mathf.Cos(heading));
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/rpg_rocket"));
            gameObject.transform.parent = gameObject.transform;
            gameObject.transform.position = RocketStatic.transform.position;
            gameObject.transform.eulerAngles = RocketStatic.transform.eulerAngles;
            rocketInfos.Add(new RocketInfo(direction, gameObject, (Vector2 point) => { if (_IExploder != null) _IExploder.Explode(point, new RPGExplosion()); }));
            RocketStatic.SetActive
                (false);
            _Loaded = false;
            _BeganReloading = Time.time;
            Smoke.Play();

        }
        public void Swipe(SwipingInfo swipingInfo)
        {
            if (!_Handled)
                if (swipingInfo.Type.Equals(SwipingInfo.Types.Traveled))
            {
                _FingerPosition = swipingInfo.Position;
                UpdateRotation(swipingInfo.Position);
            }
        }
        private bool _Handled = false;
        public void Touch(TouchInfo touchInfo)
        {
            if (!touchInfo.Handled)
            {
                _Handled = false;
                _FingerPosition = touchInfo.Position;
                UpdateRotation(touchInfo.Position);
                touchInfo.SetHandled();
            }
            else
                _Handled = true;


            
        }
        public void TouchEnded(Vector2 position)
        {
            if (!_Handled)
            {
                _ILooper.AddFixedUpdate(this);
                if (_Loaded)
                    Launch(position);
            }
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ITouchSensor).IsAssignableFrom(type))
                _ITouchSensor = (ITouchSensor)o;
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
            if (typeof(IExploder).IsAssignableFrom(type))
                _IExploder = (IExploder)o;
        }
    }
}
