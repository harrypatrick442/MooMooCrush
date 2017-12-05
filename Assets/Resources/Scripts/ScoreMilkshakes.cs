

















using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class ScoreMilkshakes:MonoBehaviour, ILooper5Hz, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private bool _Flashing = false;
		private bool _Dragging = false;
		private Vector2 _LastDragPosition;
		private GameObject _DragDropMilkshake;

		private GameObject DragDropMilkshake {

			get {
				if (_DragDropMilkshake == null)
					_DragDropMilkshake = transform.Find ("dragDropMilkshake").gameObject;
				return _DragDropMilkshake;
			}
		}

		private Image _ImageNormal;

		private Image ImageNormal {
			get {
				if (_ImageNormal == null)
					_ImageNormal = transform.Find ("normal").GetComponent<Image> ();
				return _ImageNormal;
			}
		}

		private double _FlashingStartTime;
		private bool _ShowingNormal = true;
		private int _DurationSeconds = 1;
		private Image _ImageFlashing;

		private Image ImageFlashing {
			get {
				if (_ImageFlashing == null)
					_ImageFlashing = transform.Find ("flashing").GetComponent<Image> ();
				return _ImageFlashing;
			}
		}

		private void Flash (int durationSeconds = 1)
		{
			_Flashing = true;
			_DurationSeconds = durationSeconds;
			_FlashingStartTime = Time.time;
			_ILooper.Add5Hz (this);
		}

		public bool Looper5Hz ()
		{
			if (_Flashing) {
				if (Time.time - _FlashingStartTime > _DurationSeconds) {
					ImageFlashing.gameObject.SetActive (false);
					ImageNormal.gameObject.SetActive (true);
					_Flashing = false;
				} else {
					ImageNormal.gameObject.SetActive (_ShowingNormal);
					ImageFlashing.gameObject.SetActive (!_ShowingNormal);
					_ShowingNormal = !_ShowingNormal;
				}
			}
			if (_Dragging) {
				TestHit ();
			}
			return !(_Flashing || _Dragging);
		}

		private ILooper _ILooper;
		private ISetTouchHandled _ISetTouchHandled;

		public void SetInterface<T> (T o)
		{
			if (typeof(ILooper).IsAssignableFrom (typeof(T))) {
				_ILooper = ((ILooper)o);
			}
			if (typeof(ISetTouchHandled).IsAssignableFrom (typeof(T))) {
				_ISetTouchHandled = ((ISetTouchHandled)o);
			}
		}

		public void OnPointerDown (PointerEventData eventData)
		{
			_ISetTouchHandled.SetTouchHandled ();
		}

		public void OnPointerUp (PointerEventData eventData)
		{

		}

		private Vector2 _DragOffset;

		public void OnBeginDrag (PointerEventData eventData)
		{
			_Dragging = true;
			_ILooper.Add5Hz (this);
			DragDropMilkshake.SetActive (true);
			_DragOffset = (Vector2)ImageNormal.transform.localPosition - eventData.position;
		}

		private IFeedable _CurrentlyHovering;

		public void OnDrag (PointerEventData eventData)
		{
			var pos = new Vector3(eventData.position.x + _DragOffset.x, eventData.position.y + _DragOffset.y, DragDropMilkshake.transform.localPosition.z);
			DragDropMilkshake.transform.localPosition = pos;
			_LastDragPosition = (Vector2)DragDropMilkshake.transform.position;
			TestHit ();
		}

		private void TestHit ()
		{

			RaycastHit2D hit = Physics2D.Raycast(_LastDragPosition, Vector2.zero);
			if (hit)
			{
				IFeedable iFeedable = Lookup.GetBody<IFeedable>(hit.collider.gameObject);
				if (iFeedable != null)
				{
					if(_CurrentlyHovering!=null&&_CurrentlyHovering!=iFeedable)
					{
						_CurrentlyHovering.FeedUnHover();
					}
					if(iFeedable.FeedHover<Takeout>(null))
					{
						_CurrentlyHovering=iFeedable;
					}
				}
			}
			else
			{

				if(_CurrentlyHovering!=null)
				{
					_CurrentlyHovering.FeedUnHover();
				}

			}
		}

		public void OnEndDrag (PointerEventData eventData)
		{
			_Dragging = false;
			DragDropMilkshake.SetActive (false);
			if (_CurrentlyHovering != null) {
				_CurrentlyHovering.FeedUnHover ();
				_CurrentlyHovering.Feed (new Takeout ());
			}
		}
	}
}

