using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public interface ILooper
    {
        void AddUpdate(ILooperUpdate iLooperUpdate);
        void AddFixedUpdate(ILooperFixedUpdate iLooperFixedUpdate);
        void Add1Hz(ILooper1Hz iLooper1Hz);
        void Add2Hz(ILooper2Hz iLooper2Hz);
        void Add5Hz(ILooper5Hz iLooper5Hz);
        void RemoveUpdate(ILooperUpdate iLooperUpdate);
        void RemoveFixedUpdate(ILooperFixedUpdate iLooperFixedUpdate);
        void Remove5Hz(ILooper5Hz iLooperUpdate);
        void Remove2Hz(ILooper2Hz iLooperUpdate);
        void Remove1Hz(ILooper1Hz iLooperUpdate);
    }
}
