using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    class Phaser : GunAimFireQueuer, IGunAimFire, ITouchable
    {
        private ITouchSensor _ITouchSensor;
		private IAudioPlayer _IAudioPlayer;
        public override float activeAngle { get { return 110f; } }
        public override float droppingAngularVelocity { get { return 0.04f; } }
        public override float trackingAngularVelocity { get { return 0.28f; } }
        private void Start()
        {
            base.SetInterface(this);
        }
        private class Wrapper : IGunnable
        {
            private IPhasable _IPhasable;
            public Wrapper(IPhasable iPhasable)
            {
                _IPhasable = iPhasable;
            }
            public void Gun()
            {
                _IPhasable.Phase();
            }
            public void Done()
            {
                _IPhasable.OnSlaughterStart();
                _IPhasable.DonePhase();
            }
            public bool IsGunned()
            {
                return _IPhasable.IsPhased();
            }
            public Vector2? GetPosition2()
            {
                return _IPhasable.GetPosition2();
            }
            public void OnSlaughterStart()
            {
                _IPhasable.OnSlaughterStart();
            }
            public bool IsBeingSlaughtered()
            {
                return _IPhasable.IsBeingSlaughtered();
            }
        }
        public void Phase(IPhasable iPhasable)
        {
			if(iPhasable.IsPhasable)
            	base.Gun(new Wrapper(iPhasable));
        }


        private void DrawLaser(float requiredAngle, Vector3 position, Vector3 nozelPosition)
        {
            LineRenderer.SetPosition(0, new Vector3(nozelPosition.x, nozelPosition.y, -4f));
            LineRenderer.SetPosition(1, new Vector3(position.x, position.y, -4f));
        }
        private LineRenderer _LineRenderer;
        private LineRenderer LineRenderer
        {
            get
            {
                if (_LineRenderer == null) _LineRenderer = GetComponentInChildren<LineRenderer>();
                return _LineRenderer;
            }
        }
        public void Shooting(float requiredAngle, Vector3 position, Vector3 nozelPosition)
        {
            DrawLaser(requiredAngle, position, nozelPosition);
			SoundEffectInfo soundEffectInfo = SoundEffects.Phaser.Running;
			_IAudioPlayer.Play(soundEffectInfo.Name, soundEffectInfo.Volume);
        }
        public void DoneProjectile()
        {
            LineRenderer.SetPosition(0, new Vector3(0, 0, 0));
            LineRenderer.SetPosition(1, new Vector3(0, 0, 0));
        }
        public List<IGunnable> GetTargets()
        {
            return null;
        }
        public void Touch(TouchInfo touchInfo)
        {
            if (!touchInfo.Handled)
                if (touchInfo.Hit)
                {
                    IPhasable iPhasable = Lookup.GetBody<IPhasable>(touchInfo.Hit.collider.gameObject);
                    if (iPhasable != null)
                    {
                        Phase(iPhasable);
                        return;
                    }
                }
        }
        public void Swipe(SwipingInfo swipingInfo)
        {

        }
        public void TouchEnded(Vector2 position)
        {

        }
        public void Activate()
        {
            _ITouchSensor.AddTouchable(TouchPriority.Weapon, this);
            base.Activate();
        }
        public void Deactivate()
        {
            _ITouchSensor.RemoveTouchable(this);
            base.Deactivate();
        }
        public void SetInterface(object o)
        {
            base.SetInterface(o);
            if (typeof(ITouchSensor).IsAssignableFrom(o.GetType()))
                _ITouchSensor = (ITouchSensor)o;
            if (typeof(IAudioPlayer).IsAssignableFrom(o.GetType()))
                _IAudioPlayer = (IAudioPlayer)o;

        }
    }
}
