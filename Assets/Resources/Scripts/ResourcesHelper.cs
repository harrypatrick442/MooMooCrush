using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
    public class ResourcesHelper:IResourceHelper
    {
        private Dictionary<Type, object> map = new Dictionary<Type, object>();
        public void AddAllTypes(object[] resources)
        {
            foreach(object o in resources)
            {
                foreach (Type type in Types.GetAllTypes(o.GetType()))
                {
                    map[type] = o;
                }
            }
        }
        public void Add<T>(object o)
        {
            Type type = o.GetType();
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException("The type parameter T was not assignable from the object");
            map[typeof(T)] = o;
        }
        public T Get<T>()
        {
            Type type = typeof(T);
            if(map.ContainsKey(type))
            {
                return (T)map[type];
            }
            throw new ArgumentNullException("Does not have reference to: "+typeof(T)+".");
            //return default(T);
        }
    }
}