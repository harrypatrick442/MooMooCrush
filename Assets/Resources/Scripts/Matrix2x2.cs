using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts {public class Matrix2x2 {
        public float _A, _B, _C, _D;
        public float A { get { return _A; } }
        public float B { get { return _B; } }
        public float C { get { return _C; } }
        public float D { get { return _D; } }
        public Matrix2x2(float a, float b, float c, float d)
        {
            _A = a;
            _B = b;
            _C = c;
            _D = d;
        }
       public static Vector2 operator *(Matrix2x2 m, Vector2 v)
        {
            return new Vector2((m.A * v.x) + (m.C * v.y), (m.B * v.x) + (m.D * v.y));
        }
    }
}
