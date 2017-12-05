using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Bomb:  ILooperFixedUpdate, IPause
    {
        private IExploder _IExploder;
        private ILooper _ILooper;
        private float _TimeStart;
        private const int START_COUNT_C4 = 7;
        private const int START_COUNT_TNT = 12;
        private int _StartCount;
        private bool _Paused = false;
        private Transform _Detonator;
        private float _PauseStart;
        private Transform Detonator
        {
            get

            {
                if (_Detonator == null && gameObject != null)
                    _Detonator = gameObject.transform.Find("Detonator");
                return _Detonator;
            }
        }
        private Animator[] _Bombs;
        private Animator[] Bombs
        {
            get
            {
                if(_Bombs==null)
                {
                    _Bombs = gameObject.GetComponentsInChildren<Animator>();
                }
               return _Bombs;
            }
        }
        private Enums.ExplosiveType _Type;
		public Enums.ExplosiveType ExplosiveType{ get { return _Type;}}
        private GameObject gameObject;
        private Transform _Screen;
        private Transform Screen
        {
            get

            {
                if (_Screen == null && Detonator != null)
                    _Screen = Detonator.transform.Find("Screen");
                return _Screen;
            }
        }
        private Text _Text;
        private Text Text
        {
            get

            {
                if (_Text == null && Screen != null)
                    _Text=Screen.transform.Find("Text").GetComponent<Text>();
                return _Text;
            }
        }
        private int _Value;
        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                Text text = Text;
                if(text != null)
                    text.text = value.ToString("");
            }

        }
        public void Dissable()
        {
			_ILooper.RemoveFixedUpdate (this);
			Text text = Text;
			if(text != null)
				Text.text = "---";
        }
        public void Explode()
        {
            if (gameObject != null)
            {
                _IExploder.Explode(gameObject.transform.position, null);
            }

        }
        private void SetAnimation()
        {
            String trigger = _Type.Equals(Enums.ExplosiveType.C4) ? "c4" : "tnt";
            foreach(Animator animator in Bombs)
            {
                animator.SetTrigger(trigger);
            }
        }
        public Bomb(Transform parent, IResourceHelper interfacesHelper, Enums.ExplosiveType type)
        {
            _ILooper = interfacesHelper.Get<ILooper>();
            _IExploder = interfacesHelper.Get<IExploder>();
            _Type = type;
            _TimeStart = Time.time;
            gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/bomb"));
            gameObject.transform.parent = parent.transform;
            gameObject.transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z - 0.1f);
            _StartCount = type.Equals(Enums.ExplosiveType.C4) ? START_COUNT_C4 : START_COUNT_TNT;
            Value = _StartCount;
            SetAnimation();
            _ILooper.AddFixedUpdate(this);
        }
        public bool LooperFixedUpdate()
        {
            if (_Paused)
                return true;
            int count = _StartCount-(int)(Time.time - _TimeStart);
            if (count < 1)
            {
                Explode();
                return true;
            }
            if (count != Value)
            {
                Value = count;
            }
            return false;
        }
        private void OnDestroy()
        {
            if(_ILooper!=null)
            _ILooper.RemoveFixedUpdate(this);
        }
        public void Pause()
        {
            _Paused = true;
            _PauseStart = Time.time;
        }
        public void Unpause()
        {
            if (_Paused)
            {
                _TimeStart += (Time.time - _PauseStart);
                _ILooper.AddFixedUpdate(this);
                _Paused = false;
            }
        }
    }
}
