using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Assets.Scripts
{
    public interface IRotationSensor
    {
        void AddEventHandler(RotationHandler rotationHandler);
        void RemoveEventHandler(RotationHandler rotationHandler);
    }
}