using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IScanSurroundings<T>
    {
        List<T> Scan(int nItems);
    }
}