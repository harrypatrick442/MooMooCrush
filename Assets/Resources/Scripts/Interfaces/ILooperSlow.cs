using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public interface ILooper1Hz
    {
        bool Looper1Hz();
    }
    public interface ILooper2Hz
    {
        bool Looper2Hz();
    }
    public interface ILooper5Hz
    {
        bool Looper5Hz();
    }
}
