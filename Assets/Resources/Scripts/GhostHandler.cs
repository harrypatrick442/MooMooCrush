using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    class GhostHandler : MonoBehaviour, ILooperFixedUpdate, IGhosts
    {
        private ILooper _ILooper;
        private IGetPosition2D _IPositionHeaven;
        private IGetPosition2D _IPositionHell;
        private const float _SPEED = 0.8f;
        private List<Ghost> ghosts = new List<Ghost>();
        private class Ghost
        {
            private Vector2 _Step;
            private GameObject gameObject;
            private Vector2 _StartPosition;
            private Vector3 _CurrentPosition;
            private Vector2 _Destination;
            public Ghost(GameObject gameObject, Vector2 startPosition,  Vector2 destination)
            {
                this.gameObject = gameObject;
                _StartPosition = startPosition;
                _Destination = destination;
                _CurrentPosition = _StartPosition;
                _Step = _SPEED*new Vector2(destination.x - startPosition.x, destination.y - startPosition.y).normalized;
            }
            public bool DoMove()
            {
                float dT = Time.fixedDeltaTime;
                _CurrentPosition = new Vector3(_CurrentPosition.x+(_Step.x*dT), _CurrentPosition.y+(_Step.y*dT), -6);
                gameObject.transform.position = _CurrentPosition;
                if (_Step.x>0? _CurrentPosition.x > _Destination.x: _CurrentPosition.x < _Destination.x)
                {
                    if (_Step.y > 0 ? _CurrentPosition.y > _Destination.y : _CurrentPosition.y < _Destination.y)
                    {
                        Object.Destroy(gameObject);
                        return true;
                    }
                }
                return false;
            }
        }
        public void Add(IGhost iGhost)
        {
            var position = iGhost.GetPosition2();
            if (position != null)
            {
                GameObject gameObject = iGhost.GetGhostPrefab();
                gameObject.transform.parent = this.gameObject.transform;
                gameObject.transform.position = new Vector3(((Vector2)position).x, ((Vector2)position).y, -1);
                Ghost ghost = new Ghost(gameObject, (Vector2)position, ((Random.Range((int)0, (int)7) < 6) ? ((Vector2)_IPositionHell.GetPosition2()) : ((Vector2)_IPositionHeaven.GetPosition2())));
                ghosts.Add(ghost);
                _ILooper.AddFixedUpdate(this);
            }
        }
        public bool LooperFixedUpdate()
        {
                int i = 0;
                int count = ghosts.Count;
                if (count < 1)
                    return true;
                while(i<count)
                {
                    Ghost ghost = ghosts[i];
                    if (ghost.DoMove())
                    {
                        ghosts.Remove(ghost);
                        count--;
                    }
                    else i++;
                }
            return false;
        }
        public void SetPositions (IGetPosition2D iPositionHeaven, IGetPosition2D iPositionHell)
        {
            _IPositionHeaven = iPositionHeaven;
            _IPositionHell = iPositionHell;
        }
        public void SetInterface(object o)
        {
            if (typeof(ILooper).IsAssignableFrom(o.GetType()))
            {
                _ILooper = (ILooper)o;
            }
        }
    }
}
