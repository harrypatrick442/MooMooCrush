using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Trolly : MonoBehaviour, IExplodible, IRelocatible
    {
        private Enums.FoodType[] FoodTypes = { Enums.FoodType.Strawberry, Enums.FoodType.Banana, Enums.FoodType.Chocolate, Enums.FoodType.Burger};
        private FoodItem[] FoodItems = new FoodItem[4];
        private float[] Positions = new float[4] { 0.5f, 0.16f, -0.18f, -0.52f };
        private FoodItem _FoodItem;
        private int FoodItemIndex = 1;
        private Rigidbody2D _Rigidbody2D;
        private Rigidbody2D Rigidbody2D
        {
            get
            {
                if (_Rigidbody2D == null)
                    _Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
                return _Rigidbody2D;
            }
        }
        public Enums.FoodType FoodType
        {
            set
            {
                switch(value)
                {
                    case Enums.FoodType.Strawberry:
                        FoodItemIndex = 0;
                        break;
                    case Enums.FoodType.Banana:
                        FoodItemIndex = 1;
                        break;
                    case Enums.FoodType.Chocolate:
                        FoodItemIndex = 2;
                        break;
                    case Enums.FoodType.Burger:
                        FoodItemIndex = 3;
                        break;
                }
                SetPosition();
            }
        }
        public FoodItem FoodItem
        {
            get
            {
                return FoodItems[FoodItemIndex];
            }
        }
        public int Completeness
        {
            get
            {
                return FoodItem.Completeness;
            }
            set
            {
                if (value > 5) value = 5; else if (value < 0) value = 0;
                FoodItem.Completeness = value;
            }
        }
        private Enums.FoodType _CurrentFoodType = Enums.FoodType.Banana;
        public Enums.FoodType CurrentFoodType
        {
            get
            {
                return _CurrentFoodType;
            }
            set
            {
                _CurrentFoodType = value;
            }
        }
        public void ShiftLeft()
        {
            if (FoodItemIndex < 3)
            {
                FoodItemIndex++;
                SetPosition();
            }
        }
        public void ShiftRight()
        {
            if (FoodItemIndex > 0)
            {
                FoodItemIndex--;
                SetPosition();
            }
        }
        private void SetPosition()
        {
            CurrentFoodType = FoodTypes[FoodItemIndex];
            transform.localPosition = new Vector3(Positions[FoodItemIndex], -1.13f, transform.localPosition.z);
        }
        private void Awake()
        {

        }
        private void Start()
        {
            FoodItems[0] = gameObject.GetComponentInChildren<MilkshakeStrawberry>(true);
            FoodItems[1] = gameObject.GetComponentInChildren<MilkshakeBanana>(true);
            FoodItems[2] = gameObject.GetComponentInChildren<MilkshakeChocolate>(true);
            FoodItems[3] = gameObject.GetComponentInChildren<Burger>(true);
            Lookup.AddBody(gameObject, this);
        }
		public Vector3 CurrentFoodItemPosition{
			get{ 
				return FoodItem.transform.position;
			}
		}
        public void Explode(Vector2 point, float force)
        {
            Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Rigidbody2D.constraints = RigidbodyConstraints2D.None;
            Vector2 normalized = new Vector2(gameObject.transform.position.x - point.x, gameObject.transform.position.y - point.y).normalized;
            Rigidbody2D.AddForce(new Vector2(normalized.x * force, normalized.y * force));
        }
        public Relocator GetRelocator()
        {
            return new Scripts.Relocator(Rigidbody2D);
        }

    }


}