using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    class Pauser : IPause
    {
        private List<IPause> _Parents = new List<IPause>();
        private List<WeakReference> _Pausibles = new List<WeakReference>();
        public void Add(params IPausible[] pausibles)
        {
            foreach (IPausible iPausible in pausibles)
                if (iPausible != null)
                    _Pausibles.Add(new WeakReference(iPausible));
                else
                    throw new NullReferenceException("should not be fucking null");
        }
        public void AddParent(IPause iPause)
        {
            if (!_Parents.Contains(iPause))
                _Parents.Add(iPause);
        }
        public void Pause()
        {
            List<WeakReference> toRemove = new List<WeakReference>();
            foreach (WeakReference weakReference in _Pausibles)
            {
                if (weakReference.IsAlive)
                    try
                    {
                        IPausible iPausible = (IPausible)weakReference.Target;
                        iPausible.Pause();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                else
                {
                    Debug.Log("removed");
                    toRemove.Add(weakReference);
                }
            }
            foreach (WeakReference weakReference in toRemove)
            {
                _Pausibles.Remove(weakReference);
            }
            foreach (IPause iPause in _Parents)
            {
                iPause.Pause();
            }
        }
        public void Unpause()
        {
            List<WeakReference> toRemove = new List<WeakReference>();
            foreach (WeakReference weakReference in _Pausibles)
            {
                if (weakReference.IsAlive)
                    try
                    {
                        IPausible iPausible = (IPausible)weakReference.Target;
                        iPausible.Unpause();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                else
                {
                    Debug.Log("removed");
                    toRemove.Add(weakReference);
                }
            }
            foreach (WeakReference weakReference in toRemove)
            {
                _Pausibles.Remove(weakReference);
            }
            foreach (IPause iPause in _Parents)
            {
                iPause.Unpause();
            }
        }
    }
}
