using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class Doorway:MonoBehaviour, IGetPosition2D, IExplodible, IRelocatible
    {
        public Enums.DoorwayType Type;// = Enums.DoorwayType.Heaven;
        private IDoorway _IDoorway;
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
        private void Start()
        {
            Lookup.AddBody(gameObject, this);
        }
        public void OnCollisionEnter2D(Collision2D collision)
        {
            IDoorwayable iDoorwayable = Lookup.GetBody<IDoorwayable>(collision.gameObject);
            if (iDoorwayable != null)
                _IDoorway.ExitedThroughDoorway(Type, iDoorwayable);
        }
        public void SetInterface(object o)
        {
            if (typeof(IDoorway).IsAssignableFrom(o.GetType()))
                _IDoorway = (IDoorway)o;
        }
        public Vector2? GetPosition2()
        {
            return gameObject.transform.position;
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
