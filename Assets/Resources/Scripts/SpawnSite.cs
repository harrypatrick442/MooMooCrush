using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class SpawnSite:MonoBehaviour
    {
public Vector2 Position {
            get {
                return   (Vector2)gameObject.transform.position;
            }
        }
    }
}
