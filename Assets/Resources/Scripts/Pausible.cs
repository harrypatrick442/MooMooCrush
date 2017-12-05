using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Pausible:MonoBehaviour, IPausible
    {
        public void Pause()
        {

        }
        public void Unpause()
        {

        }
        private void Start()
        {
            Lookup.AddBody(gameObject, this);
        }
    }
}
