using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class FoodItem : MonoBehaviour
    {
        Crusher crusherPiston;
        private GameObject go;
        private int _Completeness = 0;
        public string PrefabString;
        private Animator _Animator;
        private Animator Animator
        {
            get
            {
                if (_Animator == null)
                {
                    _Animator = GetComponent<Animator>();
                }
                return _Animator;
            }
        }
        public int Completeness
        {
            get
            {
                return _Completeness;
            }
            set
            {
                if (value > 5)
                {
                    value = 5;

                }
                else
                {
                    if (value < 0) value = 0;
                }
                if (value != _Completeness)
                {
                    Animator.SetInteger("completeness", value);
                }
                _Completeness = value;
            }
        }
        public void Next()
        {
            Completeness = 0;
        }
		public static bool WillAcceptIngredient (Enums.FoodType foodType, object o)
		{
			UnityEngine.Debug.Log (o.GetType ());
			if (typeof(SuperMooCow).IsAssignableFrom (o.GetType())) {
				return true;
			}
			if (typeof(Cow).IsAssignableFrom (o.GetType())) {
				Cow cow = (Cow)o;
				if (foodType.Equals (cow.FoodType)) {
					return true;
				}
			}
			return false;
		}
    }

}