using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class DisposeCallbackHandler
    {
        private object _Owner;
        private List<IDisposingCallback> list = new List<IDisposingCallback>();
        public void AddBeingDisposedOfCallback(IDisposingCallback iDisposingCallback)
        {
            list.Add(iDisposingCallback);
        }
        public DisposeCallbackHandler(object owner)
        {
            _Owner = owner;
        }
        public void Dispose()
        {
            foreach (IDisposingCallback iDisposingCallback in list)
            {
                try
                {
                    iDisposingCallback.Disposing(_Owner);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }
    }
    
}
