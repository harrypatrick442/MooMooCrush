using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    class Laser:IPausible
    {
        private GameObject gameObject;
        private string _PrefabName;
        private bool _BeamInstantiated = false;
        private bool _Playing = false;
        private GameObject _Beam;
        private GameObject Beam
        {
            get
            {
                if (_Beam == null&&!_BeamInstantiated)
                {
                    _BeamInstantiated = true;
                    _Beam = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(_PrefabName));
                    _Beam.transform.parent = gameObject.transform;
                    _Beam.transform.localPosition = new Vector3(0, 0, 0);
                }
                return _Beam;
            }
        }
        private LineRenderer _LineRenderer;
        private LineRenderer LineRenderer
        {
            get
            {
                if (_LineRenderer == null)
                {
                    _LineRenderer = Beam.GetComponentInChildren<LineRenderer>();
                }
                return _LineRenderer;
            }
        }
        private bool _ParticleSystemInstantiated = false;
        private ParticleSystem _ParticleSystem;
        private ParticleSystem ParticleSystem
        {
            get
            {
                if (_ParticleSystem == null && !_ParticleSystemInstantiated)
                {
                    _ParticleSystemInstantiated = true;
                    _ParticleSystem = Beam.GetComponentInChildren<ParticleSystem>();
                }
                return _ParticleSystem;
            }
        }
        public void SetPosition(Vector3 positionA, Vector3 positionB)
        {
            LineRenderer.SetPosition(0, positionA);
            LineRenderer.SetPosition(1, positionB);
            ParticleSystem.transform.position = positionB;
        }
        public void SetActive(bool value)
        {
            if(Beam!=null)
                Beam.SetActive(value);
            if (ParticleSystem != null)
            {
                if (value)
                {
                    _Playing = true;
                    ParticleSystem.Play();
                }
                else
                {
                    _Playing = false;
                    ParticleSystem.Stop();
                }
            }
        }

        public void Pause()
        {
            if(_Playing)
            {
                ParticleSystem.Pause();
                Beam.SetActive(false);
            }
        }

        public void Unpause()
        {
            if (_Playing)
            {
                Beam.SetActive(true);
                ParticleSystem.Play();
            }
        }

        public Laser(GameObject gameObject, string prefabName)
        {
            _PrefabName = prefabName;
            this.gameObject = gameObject;
        }
    }
}