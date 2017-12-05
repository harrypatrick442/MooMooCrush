using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class TouchPriority
    {
        public static TouchPriority UI1 = new TouchPriority();
        public static TouchPriority UI0 = new TouchPriority();
        public static TouchPriority Weapon = new TouchPriority();
        public static TouchPriority Cows = new TouchPriority();
        public static TouchPriority Camera = new TouchPriority();
        public static List<TouchPriority> List = new List<TouchPriority>() {
        UI1, UI0, Weapon, Cows, Camera
        }; 
    }
}
