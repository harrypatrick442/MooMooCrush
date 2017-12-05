using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class UFO : IUFO
    {
        private static ILooper _ILooper;
        private GameObject gameObject;
        private KinematicMotionController _MotionController;
        public Transform _Parent;
        private TractorBeam _TractorBeam;
        private TractorBeam TractorBeam
        {
            get
            {
                if (_TractorBeam == null) _TractorBeam = gameObject.GetComponentInChildren<TractorBeam>(true);
                return _TractorBeam;
            }
        }
        public bool LooperFixedUpdate()
        {
            return true;
        }
        private MotionControllerEventHandler _MotionControllerEventHandler;
        public UFO(Transform parent, UFOInfo ufoInfo, IResourceHelper interfaceHelper)
        {
            _Parent = parent;
            _ILooper = interfaceHelper.Get<ILooper>();
            gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/ufo"));
            gameObject.transform.parent = parent.transform;
            gameObject.transform.localPosition = ufoInfo.Position;
            _MotionController = new KinematicMotionController(_ILooper, this.gameObject.transform, 2.4f, 0.01f, null, KinematicMotionController.DimensionsTypes.XOnly);
            _MotionControllerEventHandler = new MotionControllerEventHandler(OnReachedTarget);
            _MotionController.AddEventHandlerReachedTarget(_MotionControllerEventHandler);
            _MotionController.MoveTo(new Vector2(0, 0));
            TractorBeam.SetInterface(_ILooper);
            TractorBeam.SetInterface(this);
            TractorBeam.HandlePickup = new Action<IDisposable>(HandleItem);
        }
        public bool OnReachedTarget(MotionControllerEventArgs e)
        {
            return true;
        }
        public void MoveTo(Target target)
        {
            _MotionController.MoveTo(target);
        }
        public void StopMoving()
        {
            _MotionController.Cancel();
        }
        public void Pause()
        { 

        }
        public void Unpause()
        {

        }
        public void HandleItem(IDisposable o)
        {
            o.Dispose();
            Debug.Log("Got Picked Up Item");
        }
    }
}
