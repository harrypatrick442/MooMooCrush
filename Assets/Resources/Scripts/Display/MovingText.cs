using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    public class MovingText : DisplayItem
	{
		public enum Times{Once, UntilReplaced}
        private string _Message;
        private float _Speed;
        private ILooper _ILooper;
		private Times _Times;
		public MovingText(string message, float speed, ILooper iLooper, Times times = Times.UntilReplaced)
        {
            _Message = message;
            _Speed = speed;
            _ILooper = iLooper;
			_Times = times;
        }
        private class MovingTextInstance : DisplayItemInstance, ILooperFixedUpdate
        {
            float _RHSParent;
            private float END_SPACE = 0.2f;
            private float _Speed;
            private List<GameObject> gameObjects = new List<GameObject>();
            private float _Width;
            private float _CutoffX, _ReadyToFinishCutoff;
            private float _X;
			private bool _Ending = false;
			MovingText.Times _Times;
			private IReadyToFinish<DisplayItemInstance> _IReadyToFinish;
			private IDone<DisplayItemInstance> _IDone;
			public override bool DoEndNow{ get { return _Times.Equals (Times.Once);} }
			public MovingTextInstance(ILooper iLooper, float speed, string message, GameObject parent, IDone<DisplayItemInstance> iDone, IReadyToFinish<DisplayItemInstance> iReadyToFinish, MovingText.Times times)
            {
				_Times = times;
                _IDone = iDone;
                _IReadyToFinish = iReadyToFinish;
                _Speed = speed;
                GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Display/moving_text"));
                gameObject.transform.parent = parent.transform;
                gameObjects.Add(gameObject);
                RectTransform rtParent = parent.GetComponent<RectTransform>();
                float parentWidth = rtParent.rect.width;
                _X = parentWidth / 2;
                gameObject.transform.localPosition = new Vector3(_X, -rtParent.rect.height / 2, 0);
                Text text = gameObject.GetComponentInChildren<Text>();
                text.text = message;
                RectTransform rt = gameObject.GetComponent<RectTransform>();
                _Width = (text.preferredWidth * rt.localScale.x) + END_SPACE;
                float actualWidth = text.preferredWidth;
                rt.sizeDelta = new Vector2(actualWidth, rt.rect.height);
                float widthTotal = 0;
                float x = _X;
                do
                {
                    x += _Width;
                    GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Display/moving_text"));
                    rt = gameObject2.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(actualWidth, rt.rect.height);
                    gameObject2.transform.parent = parent.transform;
                    gameObject2.transform.localPosition = new Vector3(x, -rtParent.rect.height / 2, 0);
                    text = gameObject2.GetComponentInChildren<Text>();
                    text.text = message;
                    gameObjects.Add(gameObject2);
                    widthTotal += _Width;
                }
                while (widthTotal < parentWidth);
                 _RHSParent = ((parentWidth))/2;
                _ReadyToFinishCutoff = ((0.5f * parentWidth) - (actualWidth * rt.localScale.x))- END_SPACE;
                _CutoffX = -((0.5f * parentWidth) + (actualWidth * rt.localScale.x));
                iLooper.AddFixedUpdate(this);
            }
            private bool _DoneReadyToFinish = false;
            private bool _Done = false;
            public bool LooperFixedUpdate()
            {	
				if (_Done)
					return true;
                float step = _Speed * Time.fixedDeltaTime; 
                GameObject gameObject = gameObjects[0];
                _X -= step;
                if (!_DoneReadyToFinish && _X <= _ReadyToFinishCutoff)
                {
					if(_IReadyToFinish.ReadyToFinish(this)||_Times.Equals(Times.Once))
                    {

                        _Ending = true;
                        int c = gameObjects.Count;
                        int i = 0;
                        while (i < c)
                        {
                            GameObject gO = gameObjects[i];
                            Debug.Log(_RHSParent);
                            Debug.Log(gO.transform.position.x);
                            if (gO.transform.position.x >= _RHSParent)
                            {
                                gameObjects.Remove(gO);
                                UnityEngine.Object.Destroy(gO);
                                c--;
                            }
                            else
                                i++;
                        }
                    }
                    _DoneReadyToFinish = true;
                }
                if (_X <= _CutoffX)
                {if (!_Ending)
                    {
                        _DoneReadyToFinish = false;
                        gameObjects.Remove(gameObject);
                        gameObjects.Add(gameObject);
                        _X += _Width;
                    }
                    else
                    {
                        _IDone.Done(this);
                    }
                }
                int count = gameObjects.Count;
                float x = _X;
                for (int i = 0; i < count; i++)
                {
                    gameObject = gameObjects[i];
                    gameObject.transform.position = new Vector3(x, gameObject.transform.position.y, gameObject.transform.position.z);
                    x += _Width;
                }
                return _Done;
            }
            public override void Destroy()
            {
                _Done = true;
                foreach (GameObject gameObject in gameObjects)
                    UnityEngine.Object.Destroy(gameObject);
            }
        }
        public override DisplayItemInstance Create(GameObject parent, IDone<DisplayItemInstance> iDone , IReadyToFinish<DisplayItemInstance> iReadyToFinish)
        {
            return new MovingTextInstance( _ILooper, _Speed, _Message, parent, iDone, iReadyToFinish, _Times);
        }
    }
}
