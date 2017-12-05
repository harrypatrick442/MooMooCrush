using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Assets.Scripts
{
	public class Bin : MonoBehaviour, IPausible, ILooperFixedUpdate, IBin
	{
		private ILooper _ILooper;
		private bool _Paused = false;
		private const float LID_LIFTED_HEIGHT = 0.45f;
		private const float LID_LIFTING_SPEED = 1f;
		private float? _LidOffset;
		private float LidOffset{ get {     
				if (_LidOffset == null) {
					_LidOffset = Lid.transform.position.y - Base.transform.position.y;
				}
				return (float)_LidOffset;
			}
		}
		private List<TrashHandler> _TrashHandlers = new List<TrashHandler> ();
		private GameObject _Lid;

		private GameObject Lid {
			get {
				if (_Lid == null) {
					_Lid = gameObject.transform.Find ("lid").gameObject;
				}
				return _Lid;
			}
		}

		private GameObject _Base;

		private GameObject Base {

			get {
				if (_Base == null) {
					_Base = gameObject.transform.Find ("base").gameObject;
				}
				return _Base;
			}
		}

		private float? _LidMax;

		private float LidMax {

			get {
				if (_LidMax == null) {
					_LidMax = LidOffset + LID_LIFTED_HEIGHT;
				}
				return(float)_LidMax;
			}
		}

		private float LidMin {
			get {
				return 0;
			}
		}

		private float _LidHeight=0f;

		private float LidHeight {//base to lid.
			get {
				return _LidHeight;
			}
			set {
				_LidHeight = value;
				if (value > LidMax) {
					_LidHeight = LidMax;
				} else {
					if (value < LidMin) {
						_LidHeight = LidMin;
					} else
						_LidHeight = value;
				}
				Lid.transform.position = new Vector3 (Lid.transform.position.x, LidHeight + LidOffset+Base.transform.position.y, Lid.transform.position.z);
			}
		}

		private bool Visible {
			set {
				gameObject.SetActive (value);
			}
		}

		private enum States
		{
			LiftingLid,
			Trashing,
			DroppingLid

		}

		private States _State = States.LiftingLid;

		private States State {
			set {
				switch (value) {
				case States.LiftingLid:
					break;
				case States.Trashing:
					Trash ();
					break;
				default:
					break;
				}
				_State = value;
			}
			get {
				return _State;
			}
		}

		public void SetInterface<T> (T o)
		{
			if (typeof(ILooper).IsAssignableFrom (typeof(T))) {
				_ILooper = (ILooper)o;
			}
		}
		private  float?     _LidWidth;
		private float LidWidth {
			get {
				if (_LidWidth == null) {
					Vector3 worldScale = MyGeometry.GetWorldScale (Lid.transform);
					_LidWidth = worldScale.x * Lid.GetComponent<RectTransform> ().rect.width;
				}
				return (float)_LidWidth;
			}
		}
		//private ///Vector2? _InBinPosition;
		private Vector2 InBinPosition{
			get {
				//if(_InBinPosition==null){
					Vector3 worldScale = MyGeometry.GetWorldScale(Base.transform);
					RectTransform rectTransform = Lid.GetComponent<RectTransform> ();
					//_InBinPosition = new Vector2 (Base.transform.position.x+(worldScale.x*rectTransform.rect.width/2), Base.transform.position.y-(worldScale.y*rectTransform.rect.height/2));
				//}
				return new Vector2 (Base.transform.position.x+(worldScale.x*rectTransform.rect.width/2), Base.transform.position.y-(worldScale.y*rectTransform.rect.height/2));
			}
		}
		public void Trash (ITrashable iTrashable)
		{
			if (iTrashable == null)
				return;
			if (State != States.Trashing) {
				State = States.LiftingLid;
				_ILooper.AddFixedUpdate (this);
				gameObject.SetActive (true);
			}
			_TrashHandlers.Add (new TrashHandler (iTrashable, _ILooper, () => {
				return  new Vector2 (Lid.transform.position.x + (LidWidth / 2), Base.transform.position.y + LidOffset + (LidHeight / 2));
			}, () => {
				return InBinPosition;
			}));
		}

		private void Trash ()
		{
			bool done = true;
			int count = _TrashHandlers.Count;
			int i = 0;
			while(i<count) {
				TrashHandler trashHandler = _TrashHandlers [i];
				bool d = trashHandler.Tick ();
				if (d) {
					_TrashHandlers.Remove (trashHandler);
					count--;
				} else
					i++;
				done = done && d;
			}
			if (done)
				State = States.DroppingLid;
		}

		public bool LooperFixedUpdate ()
		{
			if (!_Paused) {
				switch (State) {
				case States.LiftingLid:
					LidHeight += Time.fixedDeltaTime *LID_LIFTING_SPEED;
					if (LidHeight >= LidMax) {
						State = States.Trashing;
					}
					break;
				case States.Trashing:
					Trash ();
					break;
				default:
					LidHeight -= Time.fixedDeltaTime * LID_LIFTING_SPEED;
					if (LidHeight <= LidMin) {
						_ILooper.RemoveFixedUpdate (this);
						gameObject.SetActive (false);
					}
					break;
				}
			}
			return false;
		}

		public void Pause ()
		{
			_Paused = true;
		}

		public void Unpause ()
		{
			_Paused = false;
		}

		private class TrashHandler
		{
			private bool _Done = false;

			private bool Done{ get { return _Done || _StopWatch.GetMilliseconds () > 6000; } }

			private IMotionController _MotionController;
			private Func<Vector2> _GetToPosition;
			private Func<Vector2> _GetInBinPosition;
			private StopWatch _StopWatch = new StopWatch ();
			private ILooper _ILooper;
			private ITrashable _ITrashable;
			private const float BIN_SPEED=6f;
			private bool _FirstDone = false;
			public TrashHandler (ITrashable iTrashable, ILooper iLooper, Func<Vector2> getToPosition, Func<Vector2> getInBinPosition)
			{
				_GetToPosition = getToPosition;
				_ILooper = iLooper;
				_GetInBinPosition = getInBinPosition;
				_ITrashable = iTrashable;
			}
			private  MotionControllerEventHandler _Callback;
			public bool Tick ()
			{
				if (_MotionController == null) {
					_MotionController = new KinematicMotionController (_ILooper, _ITrashable.GetTransform(), BIN_SPEED, 0.05f, null, KinematicMotionController.DimensionsTypes.All);
					_Callback =new MotionControllerEventHandler((MotionControllerEventArgs e) => {
						if(_FirstDone){
							_Done = true;
							return true;
						}
						_MotionController.MoveTo(_GetInBinPosition());
						_FirstDone=true;
						return false;
					});
					_MotionController.AddEventHandlerReachedTarget (_Callback);
					_MotionController.MoveTo(_GetToPosition());
				}
				bool d = Done;
				if (d) {
					_MotionController.Dispose ();
					_ITrashable.Dispose ();
				}
				return d;
			}
			~TrashHandler(){
				_MotionController.Dispose ();
			}
		}
	}
}