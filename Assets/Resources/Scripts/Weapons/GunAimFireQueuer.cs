using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public abstract class GunAimFireQueuer : MonoBehaviour, ILooperFixedUpdate

    {
        private enum Modes { Resting, Activating, Deactivating, Active, Shooting }
        private Modes Mode;
        public abstract float activeAngle { get; }
        public abstract float droppingAngularVelocity { get; }
        public abstract float trackingAngularVelocity { get; }
        private List<GunInfo> list = new List<GunInfo>();
        private bool doDeactivate = false;
        private ILooper _ILooper;
        private bool _PickupWhenActivated;
        private IGunAimFire _IGunAimFire;
        private Vector3 _TargetPosition;
        private Transform TransformNozel
        {
            get
            {
                return  gameObject.transform.GetChild(0).GetChild(0);
            }
        }
        private class GunInfo
        {
            public float StartTime;
            public enum Modes { Start, Aiming, Shooting, Done };
            public Modes Mode = Modes.Start;
            private IGunnable iGunnable;
            private Action _Gun;
            private Action _Done;
            private Func<bool> _IsGunned;
            private Func<Vector3?> _ShootAt;
            public Vector3? ShootAt
            {
                get
                {
                    return _ShootAt();
                }
            }
            public void Gun()
            {
                _Gun();
            }
            public void Done()
            {
              _Done();
            }
            public bool IsGunned()
            {
                return _IsGunned();
            }
            public GunInfo(IGunnable iGunnable)
            {
                this.iGunnable = iGunnable;
                _Gun = new Action(() => { iGunnable.Gun(); });
                _Done = new Action(() => { iGunnable.Done(); });
                _IsGunned = new Func<bool>(() => { return iGunnable.IsGunned(); });
                _ShootAt = new Func<Vector3?>(() => {
                    Vector2? position = iGunnable.GetPosition2();
                    return position;
                });
            }
            public GunInfo(Vector3 position, List<IGunnable>iGunnables)
            {
                _Gun = new Action(() => { foreach(IGunnable iGunnable in iGunnables)iGunnable.Gun(); });
                _Done = new Action(() => { foreach(IGunnable iGunnable in iGunnables) iGunnable.Done(); });
                _IsGunned = new Func<bool>(() => { return iGunnables[0].IsGunned(); });
                _ShootAt = new Func<Vector3?>(() => {
                    return position;
                });
            }
        }
        private SpriteRenderer _SpriteRendererGun;
        private SpriteRenderer SpriteRendererGun
        {
            get
            {
                if (_SpriteRendererGun == null) _SpriteRendererGun = GetComponentInChildren<SpriteRenderer>();
                return _SpriteRendererGun;
            }
        }
        private GunInfo currentlyShooting;
        private void _Aim()
        {
            Vector3? position = currentlyShooting.ShootAt;
            if (position != null)
            {
                if (Time.time - currentlyShooting.StartTime < 0.4)
                {
                    float dX = ((Vector3)position).x - gameObject.transform.position.x;
                    float a = (Mathf.Atan((gameObject.transform.position.y - ((Vector3)position).y) / (dX)) * 180 / Mathf.PI);
                    float requiredAngle = (dX > 0 ? 180 - a : -a);
                    float dAngle = requiredAngle - gameObject.transform.localEulerAngles.z;
                    float dAngleMagnitude = Mathf.Sign(dAngle) * dAngle;
                    if (dAngleMagnitude < 10)
                    {
                        currentlyShooting.Mode = GunInfo.Modes.Shooting;
                        return;
                    }

                    gameObject.transform.localEulerAngles = new Vector3(0, 0, gameObject.transform.localEulerAngles.z + (dAngle > 0 ? (trackingAngularVelocity / Time.fixedDeltaTime) : (-trackingAngularVelocity / Time.fixedDeltaTime)));
                }
            }
            else
                currentlyShooting.Mode = GunInfo.Modes.Shooting;
        }
        private void _Shooting()
        {
            currentlyShooting.Gun();
            Vector3? position = currentlyShooting.ShootAt;
            if (position != null)
            {
                float dX = ((Vector3)position).x - gameObject.transform.position.x;
                float a = (Mathf.Atan((gameObject.transform.position.y - ((Vector3)position).y) / (dX)) * 180 / Mathf.PI);
                float requiredAngle = (dX > 0 ? 180 - a : -a);
                gameObject.transform.localEulerAngles = new Vector3(0, 0, requiredAngle);
                _IGunAimFire.Shooting(requiredAngle, (Vector3)position, new Vector3(TransformNozel.position.x, TransformNozel.position.y, -2f));
                currentlyShooting.StartTime = Time.time;
                currentlyShooting.Mode = GunInfo.Modes.Done;
            }

            else
            {
                currentlyShooting.Done();
                currentlyShooting = null;
            }
        }
        private void _Done()
        {
            float dT = Time.time - currentlyShooting.StartTime;
            if (dT > 0.2)
            {
                if (dT > 1 || currentlyShooting.IsGunned())
                {

                    currentlyShooting.Done();
                    currentlyShooting = null;
                }
                _IGunAimFire.DoneProjectile();
            }
        }
        private void _Gun()
        {

            if (currentlyShooting.Mode.Equals(GunInfo.Modes.Shooting))
            {
                _Shooting();
            }
            else
            {
                if (currentlyShooting.Mode.Equals(GunInfo.Modes.Aiming))
                {
                    _Aim();
                }
                else
                {
                    if (currentlyShooting.Mode.Equals(GunInfo.Modes.Start))
                    {
                        currentlyShooting.StartTime = Time.time;
                        currentlyShooting.Mode = GunInfo.Modes.Aiming;
                        _Aim();
                    }
                    else
                    {
                        _Done();
                    }
                }
            }
        }
        public void Gun(Vector3 position)
        {
            _TargetPosition = position;
               _PickupWhenActivated = true;
            if (Mode.Equals(Modes.Resting) || Mode.Equals(Modes.Activating))
            {
                Mode = Modes.Active;
                _ILooper.AddFixedUpdate(this);
            }
        }
        public void Gun(IGunnable iGunnable)
        {
            _PickupWhenActivated = false;
            lock (list)
            {
                list.Add(new GunInfo(iGunnable));
            }
            if (Mode.Equals(Modes.Resting) || Mode.Equals(Modes.Activating))
            {
                Mode = Modes.Active;
                _ILooper.AddFixedUpdate(this);
            }
        }
        public void Deactivate()
        {
            if (list.Count > 0)
            {
                doDeactivate = true;
            }
            else
                _Deactivate();
        }
        public void Activate()
        {
            Mode = Modes.Activating;
            _ILooper.AddFixedUpdate(this);
        }
        private void _Deactivate()
        {
            doDeactivate = false;
            Mode = Modes.Deactivating;
        }
        private void _Activating()
        {
            
            float z = gameObject.transform.eulerAngles.z + (droppingAngularVelocity / Time.fixedDeltaTime);
            if (z > activeAngle)
            {
                z = activeAngle;
                if (_PickupWhenActivated)
                    lock (list)
                    {
                        list.Add(new GunInfo(_TargetPosition, _IGunAimFire.GetTargets()));
                    }
                Mode = Modes.Active;
            }
            gameObject.transform.localEulerAngles = new Vector3(0f, 0f, z);
        }
        private void DoShootings()
        {
            if (currentlyShooting == null)
            {
                lock (list)
                {
                    if (list.Count > 0)
                    {
                        currentlyShooting = list[0];
                        list.Remove(currentlyShooting);
                    }
                }
            }
            if (currentlyShooting != null)
                _Gun();
        }
        private void _Deactivating()
        {
            float z = gameObject.transform.eulerAngles.z - (droppingAngularVelocity / Time.fixedDeltaTime);
            if (z < 0)
            {
                z = 0;
                Mode = Modes.Resting;
            }
            transform.localEulerAngles = new Vector3(0, 0, z);
        }
        public bool LooperFixedUpdate()
        {
            DoShootings();
            if (!Mode.Equals(Modes.Resting))
            {
                if (Mode.Equals(Modes.Activating))
                {
                    _Activating();
                }
                else
                {
                        if (Mode.Equals(Modes.Deactivating))
                        {
                            _Deactivating();
                        }
                }
            }
            else {  return list.Count<1; }
            return false;
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(IGunAimFire).IsAssignableFrom( type))
                _IGunAimFire = (IGunAimFire)o;
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
        }
    }
}
