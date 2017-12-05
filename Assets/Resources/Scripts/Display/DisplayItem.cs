using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public abstract class DisplayItem
    {
		public enum Times{Once, UntilReplaced}
		public abstract DisplayItemInstance Create(GameObject gameObject, IDone<DisplayItemInstance> iDone, IReadyToFinish<DisplayItemInstance> iReadyToFinish);
    }
}
