using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class SlaughterPlatform : MonoBehaviour, ISlaughterPlatform, IExplodible
    {
        private List<GameObject> _OnPlatform = new List<GameObject>();
        private ILooper _ILooper;
        private Rigidbody2D _Rigidbody2D;
        private Rigidbody2D Rigidbody2D
        {
            get
            {
                if (_Rigidbody2D == null)
                    _Rigidbody2D = GetComponent<Rigidbody2D>();
                return _Rigidbody2D;
            }
        }
        void Start()
        {
            Lookup.AddBody(gameObject, this);
        }
        public void OnCollisionEnter2D(Collision2D collision)
        {
                if (!_OnPlatform.Contains(collision.gameObject))
                    _OnPlatform.Add(collision.gameObject);
        }
        public void OnCollisionExit2D(Collision2D collision)
        {
                if(_OnPlatform.Contains(collision.gameObject))
                _OnPlatform.Remove(collision.gameObject);
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
        }
        public void Explode(Vector2 point, float force)
        {
            Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Rigidbody2D.constraints = RigidbodyConstraints2D.None;
            Vector2 normalized = new Vector2(gameObject.transform.position.x - point.x, gameObject.transform.position.y - point.y).normalized;
            Rigidbody2D.AddForce(new Vector2(normalized.x * force, normalized.y * force));

        }
        public List<T> GetInstances<T>()
        {
            List<T> returns = new List<T>();
            foreach(GameObject gameObject in _OnPlatform)
            {
                T t = Lookup.GetBody<T>(gameObject);
                if (t != null)
                    returns.Add(t);
            }
            return returns;
        }
        public Relocator GetRelocator()
        {
            return new Relocator(Rigidbody2D);
        }
        public Transform GetTransform()
        {
            return gameObject.transform;
        }
    }

}