using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public abstract class EntrailsItem
    {
        public virtual int NMax { get; }
        public virtual string[] Prefabs { get; }
        public int NUsed = 0;
        private GameObject gameObject;
        private string _PickRandomPrefab()
        {
            return Prefabs[Random.Range(0, Prefabs.Length)];
        }
        public EntrailsItem()
        {

        }
        public void Initialize(Transform transform, Vector2 position, Vector2 force)
        {
            string prefabName = _PickRandomPrefab();
            gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(prefabName));
            gameObject.transform.parent = transform;
            gameObject.transform.position = new Vector3(position.x, position.y, -6);
            Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            rigidbody2D.AddForce(force);
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
