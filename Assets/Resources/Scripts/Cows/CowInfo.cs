using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class CowInfo
    {
        private Enums.CowType _CowType = Enums.CowType.Edible;
        public Enums.CowType CowType
        {
            get
            {
                return _CowType;
            }
            set
            {
                _CowType = value;
            }
        }
        private Enums.FoodType _FoodType = Enums.FoodType.Banana;
        public Enums.FoodType FoodType
        {
            set
            {
                _FoodType = value;
            }
            get
            {
                return _FoodType;
            }
        }
        private Enums.Health _Health = Enums.Health.Well;
        public Enums.Health Health
        {
            set
            {
                _Health = value;
            }
            get
            {
                return _Health;
            }
        }
        private Enums.ExplosiveType _ExplosiveType = Enums.ExplosiveType.TNT;
        public Enums.ExplosiveType ExplositeType
        {
            set
            {
                _ExplosiveType = value;
            }
            get
            {
                return _ExplosiveType;
            }
        }
        public CowInfo()
        {

        }
    }

}