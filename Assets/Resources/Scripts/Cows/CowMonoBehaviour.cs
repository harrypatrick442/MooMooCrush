using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    public class CowMonoBehaviour : MonoBehaviour
    {
        private ICowMonoBehaviou I;
        public void SetInterface(ICowMonoBehaviou i)
        {
            I = i;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            I.OnCollisionEnter2D(collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            I.OnCollisionExit2D(collision);
        }
        private void OnDestroy()
        {
            I.OnDestroy();
        }
    }

}