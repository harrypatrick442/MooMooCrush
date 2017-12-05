using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts
{
    public class TrashableFoodItem : ITrashable
    {
		private GameObject gameObject;
		public TrashableFoodItem(GameObject parent, Vector2 position, Enums.FoodType foodType, int foodItemCompleteness){
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/trashable_food_item"));
			gameObject.transform.parent = parent.transform;
			gameObject.transform.position = new Vector3(position.x, position.y, -11);
			Animator animator = gameObject.GetComponent<Animator> ();
			animator.SetInteger ("completeness", foodItemCompleteness);
			Debug.Log ("completeness " + foodItemCompleteness);
		}
		public Transform GetTransform(){
			return gameObject.transform;
		}
		public void Dispose(){
			GameObject.Destroy (gameObject);
		}
	}
}