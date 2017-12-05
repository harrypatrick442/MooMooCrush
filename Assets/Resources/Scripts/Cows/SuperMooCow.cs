using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace Assets.Scripts
{
    public class SuperMooCow :Cow, IFightable, IDisposingCallback, ILaserEyes, IPausible, ISonicMoo, IDefencable, IIsRepositioning
    {
        private const float MIN_ENGAGE_TIME = 3000;
        private const float SWITCH_FIGHTING_WITH_PROBABILITY = 0.1F;
        private ILooper _ILooper;
        private Rect[] _AllowedRegions;
        private Vector2 _LeftEyePosition = new Vector2(-0.041f, 0.1f);
        private Vector2 _RightEyePosition = new Vector2(0.041f, 0.1f);
        private IShow _IShowAlertFeedSuperMooCow;
        private SuperMooController _SuperMooController;
        private Laser _LaserLeft;
        private Laser LaserLeft
        {
            get
            {
                if (_LaserLeft == null) _LaserLeft = new Laser(this.gameObject, "Prefabs/super_moo_cow_laser");
                return _LaserLeft;
            }
        }
        private Laser _LaserRight;
        private Laser LaserRight
        {
            get
            {
                if (_LaserRight == null) _LaserRight = new Laser(this.gameObject, "Prefabs/super_moo_cow_laser");
                return _LaserRight;
            }
        }
        public SuperMooCow(Rect[] allowedRegions, Vector3 position, MonoBehaviour parent, IResourceHelper interfacesHelper, Action<Cow> onSlaughter, IShow iShowAlertFeedSuperMooCow, CowInfo cowInfo):base(position, parent, interfacesHelper, onSlaughter, cowInfo)
        {
            object forceGet = this.HealthBar;
            _IShowAlertFeedSuperMooCow = iShowAlertFeedSuperMooCow;
            _AllowedRegions = allowedRegions;
            _ILooper = interfacesHelper.Get<ILooper>();
            _SuperMooController = new SuperMooController(this, this.HealthBar, interfacesHelper.Get<IFightsController>(), interfacesHelper, this, interfacesHelper.Get<ISuperMooMarkers>(), this.GetTransform(),this, this);
            base.AddBeingDisposedOfCallback(this);
            
        }
        #region IFightable
        private ViolenceHandler _ViolenceHandler;
        public IViolenceHandler ViolenceHandler
        {
            get
            {
                if (_ViolenceHandler == null)
                    _ViolenceHandler = new ViolenceHandler(new List<IViolence>()
                    {
                        new LaserEyes(5, this), new SonicMoo(5, this, this)
                    });
                return _ViolenceHandler;
            }
        }

        public IFighterController FighterController
        {
            get
            {
                return _SuperMooController.FighterController;
            }
        }

        Health IHealth.Health
        {
            get
            {
                return HealthBar != null ? HealthBar.Health : null;
            }
        }
        private bool _IsRepositioning = false;
        public bool IsRepositioning
        {
            get
            {
                return _IsRepositioning;
            }
            set
            {
                _IsRepositioning = value;
            }
        }
        public void Disposing(object itemBeingDisposedOf)
        {
            _SuperMooController.Dispose();
        }
        public void ShowLaserEyesLasers()
        {
            new Thread(() => {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    _IShowAlertFeedSuperMooCow.Show();
                });
            }).Start();
            LaserLeft.SetActive(true);
            LaserRight.SetActive(true);
        }
        public void SetLasersPositions(Vector2 enemyPositionLeft, Vector2 enemyPositionRight)
        {
            Vector2 leftEyePositionGlobal = gameObject.transform.TransformPoint(_LeftEyePosition);
            Vector2 rightEyePositionGlobal = gameObject.transform.TransformPoint(_RightEyePosition);
            LaserLeft.SetPosition(new Vector3(leftEyePositionGlobal.x, leftEyePositionGlobal.y, -3f), new Vector3(enemyPositionLeft.x, enemyPositionLeft.y, -3f));
            LaserRight.SetPosition(new Vector3(rightEyePositionGlobal.x, rightEyePositionGlobal.y, -3f), new Vector3(enemyPositionRight.x, enemyPositionRight.y, -3f));
        }

        public void HideLaserEyesLasers()
        {
            LaserLeft.SetActive(false);
            LaserRight.SetActive(false);
        }

        public Vector2? GetLeftEyePosition()
        {
            if (gameObject != null)
                return gameObject.transform.TransformPoint(_LeftEyePosition);
            return null;
        }

        public Vector2? GetRightEyePosition()
        {
            if (gameObject != null)
                return gameObject.transform.TransformPoint(_RightEyePosition);
            return null;
        }
        #endregion
        #region IPausible
        public override void Pause()
        {
            base.Pause();
            LaserLeft.Pause();
            LaserRight.Pause();
        }
        public override void Unpause()
        {
            base.Unpause();
            LaserLeft.Unpause();
            LaserRight.Unpause();
        }
        #endregion
#region ISuperMoo
        public Vector2? GetMouthPosition()
        {
            if(gameObject!=null)
                return gameObject.transform.TransformPoint(new Vector2(0, 0));
            return null;
        }
#endregion
    }
}