using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    class LoadingBar:MonoBehaviour, IHide, IShow
    {
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}