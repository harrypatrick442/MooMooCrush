using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Explodible:MonoBehaviour, IExplodible
    {

        public void Explode(Vector2 point, float force)
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
            Vector2 normalized = new Vector2(gameObject.transform.position.x - point.x, gameObject.transform.position.y - point.y).normalized;
            rigidbody2D.AddForce(new Vector2(normalized.x * force, normalized.y * force));
        }
        private void Start()
        {
            Lookup.AddBody(gameObject, this);
        }
    }
}
