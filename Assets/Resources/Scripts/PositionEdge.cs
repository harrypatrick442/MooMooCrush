using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    class PositionEdge : MonoBehaviour
    {
        public Enums.Position Position;
        private void Awake()
        {
            Vector2 position; Renderer renderer = GetComponent<Renderer>();
            Vector3 size = renderer.bounds.size;
            if (Position.Equals(Enums.Position.Left))
                gameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x + (size.x / 2), gameObject.transform.position.y, gameObject.transform.position.z);
            else
                gameObject.transform.position = new Vector3(-(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x + (size.x / 2)), gameObject.transform.position.y, gameObject.transform.position.z);
        }
        }
}
