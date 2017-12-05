using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class RelocationSave
    {
        private List<Relocator> _Relocators;
        public RelocationSave(List<Relocator> relocators)
        {
            _Relocators = relocators;
        }
        public void Restore()
        {
            foreach(Relocator relocator in _Relocators)
            {
                relocator.Relocate();
            }
        }
    }
}