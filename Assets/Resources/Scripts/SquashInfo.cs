using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class SquashInfo
    {
        private List<WeakReference> list = new List<WeakReference>();
        private int Contains(ISquashable iSquashable)
        {
            int i = 0;
            int count = list.Count;
            while (i < count)
            {
                WeakReference weakReference = list[i];
                if (weakReference.IsAlive)
                {
                    ISquashable c = (ISquashable)weakReference.Target;
                    if (iSquashable == c)
                        return i;
                    i++;
                }
                else
                {
                    list.Remove(weakReference);
                    count--;
                }
            }
            return -1;
        }
        public void Add(ISquashable iSquashable)
        {
            if (iSquashable != null)
            {
                int index = Contains(iSquashable);
                if (index<0)
                    list.Add(new WeakReference(iSquashable));
            }
        }
        public void Remove(ISquashable iSquashable)
        {
            if (iSquashable != null)
            {
                int index = Contains(iSquashable);
                if (index > -1)
                    list.Remove(list[index]);
            }
        }public List<ISquashable> GetChildren()
        {
            List<ISquashable> list = new List<ISquashable>();
            int count =this.list.Count;
            int i = 0;
            while(i<count)
            {
                WeakReference weakReference = this.list[i];
                if (weakReference.IsAlive)
                {
                    list.Add((ISquashable)weakReference.Target);
                    i++;
                }
                else
                {
                    this.list.Remove(weakReference);
                    count--;
                }
            }
            return list;
        }
    }
}
