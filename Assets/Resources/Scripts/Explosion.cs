using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Explosion: ILooperFixedUpdate
    {
        private static ILooper _ILooper;
        private static Transform _Parent;
        private ParticleSystem _ParticleSystem;
        private bool _Playing = false;
        private GameObject gameObject;
        private ParticleSystem ParticleSystem
        {
            get
            {
                if(_ParticleSystem==null)
                {
                    _ParticleSystem = gameObject.GetComponent<ParticleSystem>();
                }
                return _ParticleSystem;
            }
        }
        public Explosion(Transform parent, Vector2 position, ILooper iLooper,ExplosionInfo explosionInfo)
        {
            _Explosion(parent, position, iLooper, explosionInfo);
        }
        private void _Explosion(Transform parent, Vector2 position, ILooper iLooper, ExplosionInfo explosionInfo)
        {
            if (explosionInfo == null)
                explosionInfo = new ExplosionInfo();
            _ILooper = iLooper;
            gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(explosionInfo.Prefab));
            gameObject.transform.parent = parent.transform;
            gameObject.transform.position = new Vector3(position.x, position.y, parent.transform.position.z - 1f);
            _ILooper.AddFixedUpdate(this);
            if (explosionInfo.IsPhysical)
            foreach (IExplodible iExplodible in Lookup.GetBodies<IExplodible>())
            {
                iExplodible.Explode(gameObject.transform.position, 400);
            }
        }
        public bool LooperFixedUpdate()
        {
            if(!_Playing)
            {
                _Playing = true;
                ParticleSystem.Play();
            }
            else
            {
                if (ParticleSystem != null)
                {
                    if (!ParticleSystem.IsAlive())
                    {
                        UnityEngine.Object.Destroy(gameObject);
                        return true;
                    }
                }
                else
                {
                    UnityEngine.Object.Destroy(gameObject);
                    return true;
                }
            }
            return false;
        }
    }
}
