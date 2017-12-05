using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface ICowMonoBehaviou
    {
        void OnCollisionEnter2D(Collision2D collision);
        void OnCollisionExit2D(Collision2D collision);
        void OnDestroy();
    }
}
