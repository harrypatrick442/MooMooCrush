﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IVibrator
    {
        void Vibrate(Vibratable vibratable);
    }
}