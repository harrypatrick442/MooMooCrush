using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class CollisionHandler
    {
        public bool IsPath(ISquashable a, ISquashable b)
        {
            List<ISquashable> o;
            return IsPath(a, b, out o);
        }
        public bool IsPath(ISquashable a, ISquashable b, out List<ISquashable> outs)
        {
            List<ISquashable> seen = new List<ISquashable>();
            outs = _IsPath(a, seen, b);
            return outs!=null;
        }
        private List<ISquashable> _IsPath(ISquashable parent, List<ISquashable> seen, ISquashable b)
        {
            seen.Add(parent);
            List<ISquashable> children = parent.GetSquashInfo().GetChildren();
            foreach(ISquashable child in children)
            {
                if(child==b)
                {
                    List<ISquashable> returns = new List<ISquashable>();
                    returns.Add(b);
                    return returns;
                }
            }
            foreach(ISquashable child in children)
            {
                if (!seen.Contains(child))
                {
                    List<ISquashable> a = _IsPath(child, seen, b);
                    if (a != null)
                    {
                        return a;
                        a.Insert(0, parent);
                    }
                }
            }
            seen.Remove(parent);
            return null;
        }
    }
}