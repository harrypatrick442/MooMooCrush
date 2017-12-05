using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ILaserEyes: IHealth, ILaserable, IPausible
    {
        void ShowLaserEyesLasers();
        void SetLasersPositions(Vector2 enemyPositionLeft, Vector2 enemyPositionRight);
        void HideLaserEyesLasers();
        Vector2? GetLeftEyePosition();
        Vector2? GetRightEyePosition();
    }
}
