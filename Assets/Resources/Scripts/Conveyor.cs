using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class Conveyor : MonoBehaviour, IExplodible, IRelocatible, IPausible
    {
        public bool FixedDirection = true;
        public enum Directions { Left, Right, None }
        private float _PreviousSpeed = 0;
        private float _SpeedMag;
        private Animator _Animator;
        private Animator Animator
        {
            get
            {
                if (_Animator == null) _Animator = GetComponent<Animator>();
                return _Animator;
            }
        }
        private SurfaceEffector2D _SurfaceEffector2D;
        private SurfaceEffector2D SurfaceEffector2D
        {
            get
            {
                if(_SurfaceEffector2D==null)
                {
                    _SurfaceEffector2D = GetComponent<SurfaceEffector2D>();
                }
                return _SurfaceEffector2D;
            }
        }
        private bool _Stopped = false;
        public Directions Direction
        {
            set
            {
                if (!FixedDirection)
                {
                   if(_Stopped)
                    {
                        _Stopped = false;
                        Animator.enabled = true;
                    }
                    _SpeedMag = Mathf.Sign(SurfaceEffector2D.speed) * SurfaceEffector2D.speed;
                    if (value.Equals(Directions.Left))
                    {
                        Animator.SetTrigger("left");
                        SurfaceEffector2D.speed = -(float)_SpeedMag;
                    }
                    else
                    {
                        Animator.SetTrigger("right");
                        SurfaceEffector2D.speed = (float)_SpeedMag;
                    }
                }
            }
        }
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
        public void Explode(Vector2 point, float force)
        {
            Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Rigidbody2D.constraints = RigidbodyConstraints2D.None;
            Vector2 normalized = new Vector2(gameObject.transform.position.x - point.x, gameObject.transform.position.y - point.y).normalized;
            Rigidbody2D.AddForce(new Vector2(normalized.x * force, normalized.y * force));
        }
        private void Start()
        {
            Lookup.AddBody(gameObject, this);
        }
        public Relocator GetRelocator()
        {
            return new Relocator(Rigidbody2D);
        }
        public void Pause()
        {
            Animator.enabled = false;
            if(_PreviousSpeed>0)
                _PreviousSpeed = SurfaceEffector2D.speed;
            SurfaceEffector2D.enabled= false;
        }
        public void Unpause()
        {
            Animator.enabled = true;
            SurfaceEffector2D.enabled = true;
        }
    }
}
