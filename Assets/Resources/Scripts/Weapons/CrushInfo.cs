using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class CrushInfo
    {
        public CrushInfo(States state, float height)
        {
            this.state = state;
            this.height = height;
        }
        public enum States { Dropping, Down }
        public float height;
        public States state;
        public String toString()
        {
            return "CrushInfo: State: " + state + ", height: " + height + ".";
        }
    }

}