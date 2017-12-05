using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Assets.Scripts
{
    public interface IReadyToFinish<T>
    {
        bool ReadyToFinish(T t);
    }
}