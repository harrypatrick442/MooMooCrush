using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class SquashSourceSink : MonoBehaviour, ISquashSourceSink, ILooperFixedUpdate
    {
        private ILooper _ILooper;
        private ISquashHandler _ISquashHandler;
        public enum Types { Source, Both, Sink };
        public Types Type;
        private List<SquashFinishInfo> _BeingFinished = new List<SquashFinishInfo>();
        private class SquashFinishInfo {
            private ISquashable _ISquashable;
            public SquashFinishInfo(ISquashable iSquashable)
            { _ISquashable = iSquashable; StartTime = Time.time; }
            public float StartTime;
            public bool IsSquashed()
            {
                return _ISquashable.IsSquashed();
            }
            public void DoneSquash()
            {
                _ISquashable.DoneSquash();
            }

        }

        private static List<WeakReference> _Sinks = new List<WeakReference>();
        public List<ISquashable> GetSinks()
        {
            return Sinks;
        }
        private int GetIndex(ISquashable required)
        {
            int index = 0;
            int count = _Sinks.Count;
            while (index < count)
            {
                WeakReference weakReference = _Sinks[index];
                if (weakReference.IsAlive)
                {
                    ISquashable iSquashable = (ISquashable)weakReference.Target;
                    if (iSquashable == required)
                        return index;
                    index++;
                }
                else
                {
                    _Sinks.Remove(_Sinks[index]);
                    count--;
                }
            }  return -1;
        }
        private static List<ISquashable> Sinks
        {
            get
            {
                List<ISquashable> list = new List<ISquashable>();
                int index = 0;
                int count = _Sinks.Count;
                while(index<count)
                {
                    WeakReference weakReference = _Sinks[index];
                    if (weakReference.IsAlive)
                    {
                        ISquashable iSquashable = (ISquashable)weakReference.Target;
                        index++;
                        list.Add(iSquashable);
                    }
                    else
                    {
                        _Sinks.Remove(_Sinks[index]);
                        count--;
                    }
                }
                return list;
            }
        }
        public class SquashTree
        {
            public Dictionary<ISquashable, List<List<ISquashable>>> SharedNodes = new Dictionary<ISquashable, List<List<ISquashable>>>();
            public List<List<ISquashable>> Paths = new List<List<ISquashable>>();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            ISquashable iSquashable = Lookup.GetBody<ISquashable>(collision.gameObject);
            if (iSquashable != null)
            {
                if (!Type.Equals(Types.Sink))
                {
                    List<ISquashable> sinks = _ISquashHandler.GetAllSinks();
                    if (sinks.Contains(iSquashable))
                        sinks.Remove(iSquashable);
                    Squash(iSquashable, sinks);
                }
                if (!Type.Equals(Types.Source))
                {
                    if (!Sinks.Contains(iSquashable))
                    {
                        _Sinks.Add(new WeakReference(iSquashable));
                    }
                }
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            ISquashable iSquashable = Lookup.GetBody<ISquashable>(collision.gameObject);
            if (iSquashable != null)
            {
                int index = GetIndex(iSquashable);
                if (index > -1)
                {
                    _Sinks.Remove(_Sinks[index]);
                }
            }
        }
        public bool IsShowingOnScreen(ISquashable s)
        {
            return true;
        }
        public void Squash(ISquashable iSquashable, List<ISquashable> sinks)
        {
            SquashTree st = GetPathsToSinks(iSquashable, sinks);
            List<ISquashable> toSquash = new List<ISquashable>();
            foreach (ISquashable sharedNode in st.SharedNodes.Keys)
            {
                if (IsShowingOnScreen(sharedNode))
                {
                    toSquash.Add(sharedNode);
                    List<List<ISquashable>> listPaths = st.SharedNodes[sharedNode];
                    foreach (List<ISquashable> path in listPaths)
                    {
                        if (st.Paths.Contains(path))
                        {
                            st.Paths.Remove(path);
                        }
                    }
                    if (st.Paths.Count < 1)
                        break;
                }
            }
            foreach (List<ISquashable> list in st.Paths)
            {
                bool found = false;
                int i = list.Count-1;
                while (i > -1)
                {
                    ISquashable j = list[i];
                    if (IsShowingOnScreen(j))
                    {
                        found = true;
                        if (!toSquash.Contains(j))
                        { toSquash.Add(j);
                        }
                        break;
                    }
                    i--;
                }
                if (!found)
                    if (list.Count > 0)
                    {
                        ISquashable j = list[list.Count - 1];
                        if (!toSquash.Contains(j))
                        {
                            toSquash.Add(j);
                        }

                    }
            }
            foreach (ISquashable i in toSquash)
            {
                i.Squash();
                if(_BeingFinished.Count<1)
                {
                    _ILooper.AddFixedUpdate(this);
                }
                _BeingFinished.Add(new SquashFinishInfo(i));
            }
        }

        public SquashTree GetPathsToSinks(ISquashable s, List<ISquashable> sinks)
        {
            SquashTree sT = new SquashTree();
            _GetPathsToSinks(s, sT, new List<ISquashable>(), sinks);
            return sT;
        }
        private List<List<ISquashable>> _GetPathsToSinks(ISquashable parent, SquashTree squashTree, List<ISquashable> seen, List<ISquashable> sinks)
        {
            SquashInfo parentInfo = parent.GetSquashInfo();
            List<List<ISquashable>> returnss = new List<List<ISquashable>>();
            if (sinks.Contains(parent))
            {
                if (seen.Count > 0)
                {
                    List<ISquashable> returns = new List<ISquashable>(seen);
                    squashTree.Paths.Add(returns);
                    returnss.Add(returns);
                    return returnss;
                }
            }
            seen.Add(parent);
            List<ISquashable> children = parentInfo.GetChildren();
            int divisionCount = 0;
            foreach(ISquashable child in children)
            {
                if (!seen.Contains(child))
                {
                    List<List<ISquashable>> returns = _GetPathsToSinks(child, squashTree, seen, sinks);
                    if (returns.Count > 0)
                    {
                        divisionCount++;
                    }
                    returnss.AddRange(returns);
                }
            }
            if (divisionCount > 1)
                squashTree.SharedNodes.Add(parent, returnss);
            seen.Remove(parent);
            return returnss;
        }
        public void SetInterface(object o)
        {
            if (typeof(ISquashHandler).IsAssignableFrom(o.GetType()))
                _ISquashHandler = (ISquashHandler)o;
            if (typeof(ILooper).IsAssignableFrom(o.GetType()))
                _ILooper = (ILooper)o;
        }
        public bool LooperFixedUpdate()
        {
                List<SquashFinishInfo> removes = new List<SquashFinishInfo>();
                foreach(SquashFinishInfo squashInfo in _BeingFinished)
                {
                    if(squashInfo.IsSquashed()||Time.time-squashInfo.StartTime>1f)
                    {
                        squashInfo.DoneSquash();
                    }
                    removes.Add(squashInfo);
                }
                foreach(SquashFinishInfo squashInfo in removes)
                {
                    _BeingFinished.Remove(squashInfo);
                }
                return _BeingFinished.Count>0?false:true;
        }
    }
}
