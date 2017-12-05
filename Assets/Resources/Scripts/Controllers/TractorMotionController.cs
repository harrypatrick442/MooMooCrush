using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    public class TractorMotionController:IDisposable
    {
        private ITractorMotionMasterController _ITractorMotionMasterController;
        private IMotionController _VerticalMotionController;
        private IMotionController _HorizontalMotionController;
        private MotionControllerEventHandler _MotionControllerEventHandlerHorizontally;
        private MotionControllerEventHandler _MotionControllerEventHandlerVertically;
        private bool _ReachedHorizontalTarget = false;
        private bool _ReachedVerticalTarget = false;
        private ITractable _ITractable;
        private IHandleItem<ITractable> _HandlePickedUp;
        public TractorMotionController(IHandleItem<ITractable> handlePickedUp, ITractable iTractable, ITractorMotionMasterController iTractorMotionMasterController, ILooper iLooper, Rigidbody2D rigidbody2D)
        {
               _HandlePickedUp = handlePickedUp;
            _ITractorMotionMasterController = iTractorMotionMasterController;
            _VerticalMotionController = new KinematicMotionController(iLooper, rigidbody2D.transform, 1, 0.05f, null, KinematicMotionController.DimensionsTypes.YOnly);
            _HorizontalMotionController = new KinematicMotionController(iLooper, rigidbody2D.transform, 1, 0.05f, null, KinematicMotionController.DimensionsTypes.XOnly); //new DynamicMotionController(iLooper, rigidbody2D, 1f, 2f, 400f, 50f, 0.01f,DynamicMotionController.DimensionsTypes.XOnly);
           _MotionControllerEventHandlerHorizontally = new MotionControllerEventHandler(ReachedTargetHorizontally);
           _MotionControllerEventHandlerVertically = new MotionControllerEventHandler(ReachedTargetVertically);
            _HorizontalMotionController.AddEventHandlerReachedTarget(_MotionControllerEventHandlerHorizontally);
            _VerticalMotionController.AddEventHandlerReachedTarget(_MotionControllerEventHandlerVertically);
            _HorizontalMotionController.MoveTo(iTractorMotionMasterController.GetRecipricolTransform());
            _VerticalMotionController.MoveTo(iTractorMotionMasterController.GetRecipricolTransform());
            _ITractable = iTractable;
        }
        public bool ReachedTargetHorizontally(MotionControllerEventArgs e)
        {
            _ReachedHorizontalTarget = true;
            ReachedTarget();
            return _ReachedVerticalTarget;
        }
        public bool ReachedTargetVertically(MotionControllerEventArgs e)
        {
            _ReachedVerticalTarget = true;
            ReachedTarget();
            return _ReachedHorizontalTarget;
        }
        private void ReachedTarget()
        {
            if(_ReachedHorizontalTarget&&_ReachedVerticalTarget)
            {
                _HandlePickedUp.HandleItem(_ITractable);
            }
        }
        public void Dispose()
        {
            _VerticalMotionController.Dispose();
            _HorizontalMotionController.Dispose();
        }
    }
}
