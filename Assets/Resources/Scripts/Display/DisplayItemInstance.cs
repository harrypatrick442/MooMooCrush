using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public abstract class DisplayItemInstance
	{
		public abstract bool DoEndNow{ get; }
        protected IDone<DisplayItemInstance> _IDone;
        protected IReadyToFinish<DisplayItemInstance> _IReadyToFinish;
        public abstract void Destroy();
    }
}
