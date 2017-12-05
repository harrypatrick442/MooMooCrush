using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IHandleItem<T>
    {
        void HandleItem(T item);
    }
}
