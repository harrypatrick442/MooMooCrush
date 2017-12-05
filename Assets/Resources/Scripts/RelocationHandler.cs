using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class RelocationHandler:IRelocator
    {
        private List<IRelocatible> _Relocatibles = new List<IRelocatible>();
        public void Add(IRelocatible iRelocatible)
        {
            _Relocatibles.Add(iRelocatible);
        }
        public RelocationSave GetRelocationSave()
        {
            List<Relocator> list = new List<Relocator>();
            foreach (IRelocatible ir in _Relocatibles)
                list.Add(ir.GetRelocator());
            return new RelocationSave(list);
        }
    }
}