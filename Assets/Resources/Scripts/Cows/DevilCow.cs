using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class DevilCow : Cow, IFightable, IDisposingCallback, ILaserEyes, IPausible, IDefencable, IIsRepositioning
    {
        public DevilCow(Rect[] allowedRegions, Vector3 position, MonoBehaviour parent, IResourceHelper interfacesHelper, Action<Cow> onSlaughter, CowInfo cowInfo):base(position, parent, interfacesHelper, onSlaughter, cowInfo)
        {
            object forceGet = this.HealthBar;
            _AllowedRegions = allowedRegions;
            _IFightsController = interfacesHelper.Get<IFightsController>();
            _ILooper = interfacesHelper.Get<ILooper>();
            _DevilCowController = new DevilCowController(this, this.HealthBar, _IFightsController, allowedRegions, interfacesHelper, this, this.GetTransform(), this, this);
            base.AddBeingDisposedOfCallback(this);
        }
        private const float MIN_ENGAGE_TIME = 3000;
        private const float SWITCH_FIGHTING_WITH_PROBABILITY = 0.1F;
        private ILooper _ILooper;
        private IFightsController _IFightsController;
        private Rect[] _AllowedRegions;
        private Vector2 _LeftEyePosition = new Vector2(-0.060f, 0.030f);
        private Vector2 _RightEyePosition = new Vector2(0.060f, 0.030f);
        private Laser _LaserLeft;
        private Laser LaserLeft
        {
            get
            {
                if (_LaserLeft == null) _LaserLeft = new Laser(this.gameObject, "Prefabs/devil_cow_laser");
                return _LaserLeft;
            }
        }
        private Laser _LaserRight;
        private Laser LaserRight
        {
            get
            {
                if (_LaserRight == null) _LaserRight = new Laser(this.gameObject, "Prefabs/devil_cow_laser");
                return _LaserRight;
            }
        }
        private DevilCowController _DevilCowController;
        #region IFightable
        public IFighterController FighterController
        {
            get
            {
                return _DevilCowController.FighterController;
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
        //private bool _IsFighting;
        //public bool IsFighting { get { return _IsFighting; } }
        private ViolenceHandler _ViolenceHandler;
        public IViolenceHandler ViolenceHandler
        {
            get
            {
                if (_ViolenceHandler == null)
                    _ViolenceHandler = new ViolenceHandler(new List<IViolence>()
                    {
                        new LaserEyes(5, this), new Defenceless(this)
                    });
                return _ViolenceHandler;
            }
        }

        Health IHealth.Health
        {
            get
            {
                return HealthBar!=null?HealthBar.Health:null;
            }
        }

        public void Disposing(object itemBeingDisposedOf)
        {
            _DevilCowController.Dispose();
        }
        public void ShowLaserEyesLasers()
        {
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
    }
}