using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public interface ILooperTick<T>
    {
        bool LooperTick<T>();
    }
}
