using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    public class TractoringInfo
    {
        private TractorMotionController _TractorMotionController;
        public RigidbodyType2D _FormerBodyType;
        private ITractable _ITractable;
        public TractoringInfo(ITractable iTractable, TractorMotionController tractorMotionController)
        {
            _ITractable = iTractable;
            _ITractable.TakeKinematic(this);
            _TractorMotionController = tractorMotionController;
        }
        public void Removing()
        {
            _TractorMotionController.Dispose();
            _ITractable.ReleaseKinematic(this);
            _ITractable.SetMassless(false);
            Debug.Log("Removing");
        }
    }
}
