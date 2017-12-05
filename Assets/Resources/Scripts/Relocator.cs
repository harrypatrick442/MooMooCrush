using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class Relocator
    {
        private class Rigidbody2DInfo
        {
            private Rigidbody2D _Rigidbody2D;
            private RigidbodyConstraints2D _RigidbodyConstraints2D;
            private Vector3 _Position;
            private Vector3 _EulerAngles;
            public Rigidbody2DInfo(Rigidbody2D rigidbody2D)
            {
                _Rigidbody2D = rigidbody2D;
                _Position = _Rigidbody2D.transform.position;
                _EulerAngles = _Rigidbody2D.transform.eulerAngles;
                _RigidbodyConstraints2D = _Rigidbody2D.constraints;
            }
            public void Restore()
            {
                _Rigidbody2D.constraints = _RigidbodyConstraints2D;
                _Rigidbody2D.transform.position = _Position;
                _Rigidbody2D.transform.rotation.SetEulerAngles(_EulerAngles);
            }
        }
        private List<Rigidbody2DInfo> list = new List<Rigidbody2DInfo>();
        public Relocator(params Rigidbody2D[] rigidbody2Ds)
        {
            foreach (Rigidbody2D rigidbody2D in rigidbody2Ds)
            {
                if (rigidbody2D != null)
                {
                    list.Add(new Rigidbody2DInfo(rigidbody2D));
                }
                else
                    throw new NullReferenceException("should not be null mother fucker");
            }
        }
        public void Relocate()
        {
            foreach (Rigidbody2DInfo rigidbody2DInfo in list)
                rigidbody2DInfo.Restore();
        }
    }
}