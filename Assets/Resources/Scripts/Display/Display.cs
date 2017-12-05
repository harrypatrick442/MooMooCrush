using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class Display:MonoBehaviour, IReadyToFinish<DisplayItemInstance>, IDone<DisplayItemInstance>
    {
        private const float MAX_QUEUE_SIZE = 3;
        private DisplayItemInstance _Ending;
        private List<DisplayItem> _ToDisplayNext = new List<DisplayItem>();
        private DisplayItemInstance _CurrentDisplayItem;
        public void DisplayItem(DisplayItem displayItem)
        {
            if (_CurrentDisplayItem != null)
                _CurrentDisplayItem.Destroy();
            _CurrentDisplayItem = displayItem.Create(gameObject, this, this);
        }
        public void Queue(DisplayItem displayItem)
        {
            if (_CurrentDisplayItem != null)

            {
                while (_ToDisplayNext.Count > MAX_QUEUE_SIZE)
                    _ToDisplayNext.Remove(_ToDisplayNext[0]);
                _ToDisplayNext.Add(displayItem);
            }
            else
                DisplayItem(displayItem);

        }
		public bool ReadyToFinish(DisplayItemInstance displayItem)
        {
			if (_ToDisplayNext.Count > 0) {
				//if (_Ending == null) {

				//	_Ending = displayItem;
					DisplayItem displayItemToDisplay = _ToDisplayNext [0];
					_CurrentDisplayItem = displayItemToDisplay.Create (gameObject, this, this);
					_ToDisplayNext.Remove (displayItemToDisplay);
					return true;
				//}
			} else {
				if (displayItem.DoEndNow) {
					_CurrentDisplayItem = null;
				}
			}
            return false;
        }
        public void Done(DisplayItemInstance displayItemInstance)
        {
            displayItemInstance.Destroy();
                _Ending = null;
        }
    }
}
