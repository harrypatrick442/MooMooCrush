using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public interface IBurnable: ISlaughter
    {
        void Burn();
        void DoneBurn();
        bool IsBurnt();
    }
}
