using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts
{
    class Types
    {
        public static List<Type> GetAllTypes(Type type)
        {
            List<Type> list = new List<Type>();
            list.AddRange(type.GetInterfaces());
            list.Add(type);
            type = type.BaseType;
            while (type != null)
            {
                if (type.Equals(typeof(MonoBehaviour)))
                    break;
                list.Add(type);
                type = type.BaseType;
            }
            return list;
        }
    }
}
