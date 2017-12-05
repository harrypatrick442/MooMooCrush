using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
namespace Assets.Scripts
{
    class SurroundingsScanner<T>:IScanSurroundings<T>
    {
        private float _AngleStep;
        private float _Distance;
        private Transform _Transform;
        private float _OffsetAngle;
        public SurroundingsScanner(float angleStep, float distance, Transform transform, float startAngle)
        {
            _AngleStep = angleStep;
            _Distance = distance;
            _Transform = transform;
            _OffsetAngle = Mathf.PI * (startAngle % 360) / 180;
        }
        private void _Scan(List<T> list, float angle)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(_Transform.position, new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)), _Distance);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit)
                {
                    T i = Lookup.GetBody<T>(hit.collider.gameObject);
                    if (i != null)
                    {
                        if (!list.Contains(i))
                            list.Add(i);
                    }
                }
            }
        }
        public List<T> Scan(int nItems)
        {
            float angle = _OffsetAngle;
            float step = (Mathf.PI * _AngleStep / 180);
            float limit = 2 * Mathf.PI;
            List<T> list = new List<T>();
            while (angle < limit&&list.Count<nItems)
            {
                _Scan(list, angle);
                angle += step;
            }
            angle = 0;
            while (angle < _OffsetAngle && list.Count < nItems)
            {
                _Scan(list, angle);
                angle += step;
            }
            return list;
        }
    }
}
