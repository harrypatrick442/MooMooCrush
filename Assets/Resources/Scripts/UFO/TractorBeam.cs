using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    public class TractorBeam: MonoBehaviour, IPausible, ITractor, ILooper2Hz, IHandleItem<ITractable>
    {
        private IMove _IMove;
        private IStopMoving _IStopMoving;
        private TractorSensor _TractorSensor;
        private TractorMotionMasterController _TractorMotionController;
        private SurroundingsScanner<ITractableStalkable> _SurroundingsScanner;
        private bool _Paused = false;
        private EventDictionary<ITractable, TractoringInfo>.DictionaryUpdateHandler _DictionaryUpdateHandler;
        private EventDictionary<ITractable, TractoringInfo> _InterractingWith;
        private WeakReference _WasOrIsStalking;
        public float Width = 0.5f;
        public float Height = 9f;
        private bool _Scan = false;
        public float MinWidthTargets = 0.2f;
        private GameObject _Animation;
        private GameObject Animation
        {
            get
            {
                if (_Animation == null) _Animation = transform.Find("animation").gameObject;return 
                _Animation;
            }
        }
        private Action<IDisposable> _HandlePickup;
        public Action<IDisposable> HandlePickup {
            set {
                _HandlePickup = value;
            }
            get
            {
                return _HandlePickup;
            }
        }
        private GameObject _RecipricalMarker;
        private GameObject RecipricalMarker
        {
            get
            {
                if (_RecipricalMarker == null) _RecipricalMarker = gameObject.transform.Find("recipricol_marker").gameObject;
                return _RecipricalMarker;
            }
        }
        private GameObject _TopMarker;
        private GameObject TopMarker
        {
            get
            {
                if (_TopMarker == null) _TopMarker = gameObject.transform.Find("top_marker").gameObject;
                return _TopMarker;
            }
        }
        private ILooper _ILooper;
        public TractorBeam(ILooper iLooper)
        {
            _ILooper = iLooper;
        }
        public void Pause()
        {
            if (!_Paused)
            {
            }
            _Paused = true;
        }
        public void Unpause()
        {
            if (_Paused)
            {
            }
            _Paused = false;
        }
        public EventDictionary<ITractable, TractoringInfo> GetInterractions()
        {
            return _InterractingWith;
        }
        public Vector2 GetBeamDirection()
        {
            return (Vector2)RecipricalMarker.transform.position - (Vector2)TopMarker.transform.position;
        }
        public void AddInterraction(ITractable iTractable)
        {
            _IStopMoving.StopMoving();
            iTractable.TakeTractable(new WeakReference(this));
            iTractable.AddBeingDisposedOfCallback(this);
            iTractable.SetMassless(true);
            _InterractingWith.Add(iTractable, new TractoringInfo(iTractable, new TractorMotionController(this, iTractable, _TractorMotionController, _ILooper, iTractable.Rigidbody2D)));
        }
        public void SetInterface(object o)
        {
            Type type = o.GetType();
            if(typeof(ILooper).IsAssignableFrom(type))
            {
                _TractorSensor.ILooper = (ILooper)o;
                _ILooper = (ILooper)o;
                _ILooper.Add2Hz(this);
            }
            if (typeof(IMove).IsAssignableFrom(type))
            {
                _IMove = (IMove)o;
            }
            if (typeof(IStopMoving).IsAssignableFrom(type))
            {
                _IStopMoving = (IStopMoving)o;
            }
        }
        public void OnDictionaryUpdated(DictionaryEventArgs<ITractable, TractoringInfo> dictionaryEventArgs)
        {
            switch (dictionaryEventArgs.UpdateType)
            {
                case DictionaryEventArgs<ITractable, TractoringInfo>.UpdateTypes.Add:
                Animation.SetActive(true);
                _TractorMotionController.TractablesChanged();
                    break;
                case DictionaryEventArgs<ITractable, TractoringInfo>.UpdateTypes.Remove:
                    if (_InterractingWith.Count < 1)
                    {
                        Animation.SetActive(false);
                        ScanUntilFindOne();
                    }
                    dictionaryEventArgs.Value.Removing();
                    dictionaryEventArgs.Key.ReleaseTractable();
                    break;
                case DictionaryEventArgs<ITractable, TractoringInfo>.UpdateTypes.Clear:
                        Animation.SetActive(false);
                        ScanUntilFindOne();
                    foreach(KeyValuePair<ITractable, TractoringInfo> keyValuePair in dictionaryEventArgs.List)
                    {
                        keyValuePair.Value.Removing();
                        keyValuePair.Key.ReleaseTractable();
                    }
                    break;
            }
    }
        private void Awake()
        {
            _InterractingWith = new EventDictionary<ITractable, TractoringInfo>();
            _DictionaryUpdateHandler = new EventDictionary<ITractable, TractoringInfo>.DictionaryUpdateHandler(OnDictionaryUpdated);
            _InterractingWith.AddEventHandler(_DictionaryUpdateHandler);
            _TractorSensor = new TractorSensor(this, MinWidthTargets);
            _TractorMotionController = new TractorMotionMasterController(this);
            _SurroundingsScanner = new SurroundingsScanner<ITractableStalkable>(5f, Height, gameObject.transform, 180f);
        }
        private void ReleaseWasOrIsStalking()
        {
            if(_WasOrIsStalking!=null)
                if(_WasOrIsStalking.IsAlive)
                {
                    ((IStalkable)_WasOrIsStalking.Target).StopStalking();
                }
        }
        private int _ScanCount = 0;
        private void Scan()
        {
            if (_Scan)
            {
                List<ITractableStalkable> list = _SurroundingsScanner.Scan(1);
                List<ITractableStalkable> removes = new List<ITractableStalkable>();
                foreach(ITractableStalkable iTractable in list)
                {
                        if(iTractable.StalkableIsTaken||!iTractable.TractableEnabled)
                    {
                            removes.Add(iTractable);
                    }
                }
                foreach(ITractableStalkable iTractable in removes)
                {
                        list.Remove(iTractable);
                }
                if (list.Count > 0)
                    {
                    _Scan = false;
                    ReleaseWasOrIsStalking();
                    ITractableStalkable chosen = list[0];
                    _WasOrIsStalking = new WeakReference(chosen);
                    chosen.TakeStalkable(new WeakReference(this));
                    _IMove.MoveTo(new Target(chosen.GetTransform()));
                }
            }
        }
        private void ScanUntilFindOne()
        {
            _Scan = true;
            _ILooper.Add2Hz(this);
        }
        public bool Looper2Hz() {
            Scan();
            return !_Scan;
        }
        private void Start()
        {
            ScanUntilFindOne();
        }
        private void OnDestroy()
        {
            if (_TractorSensor != null)
                _TractorSensor.Dispose();
            if (_TractorMotionController != null)
                _TractorMotionController.Dispose();
            _ILooper.Remove2Hz(this);
        }
        public float GetWidth()
        {
            return Width;
        }
        public float GetHeight()
        {
            return Height;
        }
        public void HandleItem(ITractable item)
        {
            if (_InterractingWith.Keys.Contains(item))
            {
                _InterractingWith.Remove(item);
            }
            if (_HandlePickup!=null)
                _HandlePickup(item);
        }
        public void Disposing(object itemBeingDisposedOf)
        {
            Type type = itemBeingDisposedOf.GetType();
            if (typeof(ITractable).IsAssignableFrom(type))
            {
                ITractable iTractable = (ITractable)itemBeingDisposedOf;
                if (_InterractingWith.Keys.Contains(iTractable))
                {
                    Debug.Log("Disposing done");
                    _InterractingWith.Remove(iTractable);
                }
            }
        }

        public Transform GetRecipricolTransform()
        {
            return RecipricalMarker.transform;
        }
    }
}
