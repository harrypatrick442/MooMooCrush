using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ITractor: ITractorDimensions, IDisposingCallback
    {
        EventDictionary<ITractable, TractoringInfo> GetInterractions();
        void AddInterraction(ITractable iTractable);
        Action<IDisposable> HandlePickup { get; set; }
    }
}
