using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class SquashHandler: ISquashHandler
    {
        public List<ISquashSourceSink> squashSourceSinks = new List<ISquashSourceSink>();
        public void Add(ISquashSourceSink squashSourceSink)
        {
            if (!squashSourceSinks.Contains(squashSourceSink))
                squashSourceSinks.Add(squashSourceSink);
        }
        public List<ISquashable> GetAllSinks()
        {
            List<ISquashable> list = new List<ISquashable>();
            foreach(ISquashSourceSink iSquashSourceSink in squashSourceSinks)
            {
                List<ISquashable> sourceSinks = iSquashSourceSink.GetSinks();
                foreach(ISquashable I in sourceSinks)
                    if(!list.Contains(I))
                        list.Add(I);
            }
            return list;
        }
    }
}
