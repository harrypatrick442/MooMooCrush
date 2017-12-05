using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Assets.Scripts
{
    public interface IShove
    {
        bool Shove(ShoveInfo shoveInfo);
        bool IsShovable { get; }
    }
}