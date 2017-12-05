using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BulletWound : MonoBehaviour
    {
        private Animator _Animator;
        private Animator Animator
        {
            get
            {
                if (_Animator == null)
                    _Animator = GetComponent<Animator>();
                return _Animator;
            }
        }
        private void Awake()
        {
            Animator.SetTrigger(Random.Range(0, 3).ToString());
        }
    }
}
