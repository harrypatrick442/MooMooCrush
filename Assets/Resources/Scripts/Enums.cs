using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Enums
    {
        public enum FoodType
        {
            Chocolate, Burger, Strawberry, Banana, Inedible
        }
        public enum Health { Well, Unwell }
        public enum DoorwayType { Heaven, Hell}
        public enum CowType { Edible, Terror, Religious, SuperMoo, Devil}
        public enum SpawnerMode { Normal, Religious, Sick}
        public enum Position { Left, Right}
        public enum ExplosiveType {C4, TNT}
        public enum Attacks { LaserEyes, SonicMoo}
        public enum Difficulties { Easy0, Easy1, Easy2, Medium0, Medium1, Medium2, Hard0, Hard1, Hard2 }
    }
}