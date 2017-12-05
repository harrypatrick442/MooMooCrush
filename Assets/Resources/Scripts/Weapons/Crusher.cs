using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Crusher : MonoBehaviour, ILooperFixedUpdate, IExplodible, IRelocatible
    {

        private float height = 0f;
        private enum States { Dropping, Rising, Resting };
        private States _State = States.Resting;
        private Vector3 restingPosition;
        private float upHeight;
        private float downHeight;
        private bool _StartedDrop = false;
        private const float SPEED_DROPPING = 12f;
        private const float SPEED_RISING = 3.0f;
        private const float EXTENSION_DISTANCE = -0.82f;
        private const float CRUSH_ZONE_WIDTH = 1f;
        private Rect CrushZone;
        private ICrusher _ICrusher;
		private IAudioPlayer _IAudioPlayer;
        private List<ICrushable> BeingCrushed = new List<ICrushable>();
        private ILooper _ILooper;
        private ISlaughterPlatform _ISlaughterPlatform;
        private GameObject _Piston;
        private GameObject Piston
        {
            get
            {
                if (_Piston == null)
                {
                    _Piston = gameObject.transform.Find("Piston").gameObject;
                }
                return _Piston;
            }
        }
        private GameObject _CrusherSquisher;
        private GameObject CrusherSquisher
        {
            get
            {
                if (_CrusherSquisher == null)
                {
                    _CrusherSquisher = Piston.transform.Find("squisher").gameObject;
                }
                return _CrusherSquisher;
            }
        }
        private Transform CrusherSquisherTransform
        {
            get
            {
                return CrusherSquisher.transform;
            }
        }
        private Renderer CrusherSquisherRenderer
        {
            get
            {
                return CrusherSquisher.GetComponent<Renderer>();
            }
        }
        void Start()
        {
            restingPosition = CrusherSquisherTransform.position;
            upHeight = restingPosition.y;
            downHeight = EXTENSION_DISTANCE + upHeight;
            height = upHeight;
            Lookup.AddBody(gameObject, this);
            CalculateCrushZone();
        }
        public void Crush()
        {
            _ILooper.AddFixedUpdate(this);
            if (_State.Equals(States.Resting) || _State.Equals(States.Rising))
            {
                _State = States.Dropping;
                List<ICrushable> iCrushables = _ISlaughterPlatform.GetInstances<ICrushable>();
                foreach (ICrushable iCrushable in iCrushables)
                {
                    Rect? rect = iCrushable.GetRect();
                    if (rect != null)
                    {
                        if (MyGeometry.Contains(CrushZone, (Rect)rect))
                        {
                            BeingCrushed.Add(iCrushable);
                        }
                    }
                }
            }
        }
        private void CalculateCrushZone()
        {
            Vector3 size = CrusherSquisherRenderer.bounds.size;
            //float width = size.x * transform.localScale.x;
            float height = EXTENSION_DISTANCE;
            float y = CrusherSquisherTransform.position.y + (height / 2) - (CrusherSquisherTransform.localScale.y * size.y / 2);
            float x = CrusherSquisherTransform.position.x;
            CrushZone = new Rect(x, y - .25f, CRUSH_ZONE_WIDTH/*width*/, 0.5f - (height));
        }
        private void Spill(ICrushable iCrushable)
        {
            GameObject go = iCrushable.GetCrushParticleSystem();
            Transform crushPlatTransform = _ISlaughterPlatform.GetTransform();
            go.transform.parent = crushPlatTransform;
            go.transform.position = new Vector3(crushPlatTransform.position.x, crushPlatTransform.position.y - 0.06f, crushPlatTransform.position.z - 1f);
            go.GetComponent<ParticleSystem>().Play();

        }
        public bool LooperFixedUpdate()
        {
            if (!_State.Equals(States.Resting))
            {
                if (_State.Equals(States.Dropping))
                {
                    height = height - (SPEED_DROPPING * Time.deltaTime);
                    if (height < downHeight)
                    {
                        height = downHeight;
                        _State = States.Rising;
						SoundEffectInfo soundEffectInfo = SoundEffects.Crusher.Stomp;
						_IAudioPlayer.Play(soundEffectInfo.Name, soundEffectInfo.Volume);
                        foreach (ICrushable iCrushable in BeingCrushed)
                        {
                            iCrushable.Crush(new CrushInfo(CrushInfo.States.Down, height - downHeight));
                        }
                        if (_ICrusher != null)
                            _ICrusher.ICrushablesCrushed(BeingCrushed);
                        BeingCrushed.Clear();
                    }
                    else
                    {
                        foreach (ICrushable iCrushable in BeingCrushed)
                        {
                            if (!_StartedDrop)
                            {
                                _StartedDrop = true;
                                Spill(iCrushable);
                            }
                            iCrushable.Crush(new CrushInfo(CrushInfo.States.Dropping, height - downHeight));
                        }
                    }
                }
                else
                {
                    _StartedDrop = false;
                    height = height + (SPEED_RISING * Time.deltaTime);
                    if (height > upHeight)
                    {
                        height = upHeight;
                        _State = States.Resting;
                    }
                }
                CrusherSquisherTransform.position = new Vector3(0, height, restingPosition.z);
            }
            else { return true; }
            return false;
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if (typeof(ICrusher).IsAssignableFrom(type))
                _ICrusher = (ICrusher)o;
            if (typeof(ILooper).IsAssignableFrom(type))
                _ILooper = (ILooper)o;
            if (typeof(IAudioPlayer).IsAssignableFrom(type))
                _IAudioPlayer = (IAudioPlayer)o;
            if (typeof(ISlaughterPlatform).IsAssignableFrom(type))
                _ISlaughterPlatform = (ISlaughterPlatform)o;

        }
        private void _Explode(Vector2 point, float force, GameObject gameObject)
        {
            Rigidbody2D Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            Rigidbody2D.constraints = RigidbodyConstraints2D.None;
            Vector2 normalized = new Vector2(gameObject.transform.position.x - point.x, gameObject.transform.position.y - point.y).normalized;
            Rigidbody2D.AddForce(new Vector2(normalized.x * force, normalized.y * force));

        }
        public void Explode(Vector2 point, float force)
        {
            _Explode(point, force, Piston);
        }
        public Relocator GetRelocator()
        {
            return new Relocator(Piston.GetComponent<Rigidbody2D>(), CrusherSquisher.GetComponent<Rigidbody2D>());
        }
    }

}