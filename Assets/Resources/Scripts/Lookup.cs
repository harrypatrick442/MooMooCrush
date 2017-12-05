using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts
{
    class Lookup
    {
        private static Dictionary<GameObject, List<object>> map = new Dictionary<GameObject, List<object>>();
        private static Dictionary<Type, List<object>> mapTypeToObjects = new Dictionary<Type, List<object>>();
        //private static Dictionary<Type, List<object>> mapTypeToObject = new Dictionary<Type, List<object>>();
        public static T  GetBody<T>(GameObject gameObject)
        {
            lock (map)
            {
                if (map.ContainsKey(gameObject))
                {
                    List<object> list  = map[gameObject];
                    foreach (object o in list)
                    {
                        if (typeof(T).IsAssignableFrom(o.GetType()))
                        {
                            return (T)o;
                        }
                    }
                }
                return default(T);
            }
        }
        public static List<T> GetBodies<T>()
        {
            Type type = typeof(T);
            if (mapTypeToObjects.ContainsKey(type))
            {
                return mapTypeToObjects[type].ConvertAll<T>(a=>(T)a);
            }
            return new List<T>();
        }
        private static void AddToTypeMap(Type type, object o)
        {
            List<object> list = null;
            if (!mapTypeToObjects.ContainsKey(type))
            {
                list = new List<object>();
                mapTypeToObjects[type] = list;
            }
            else
                list = mapTypeToObjects[type];
            if (!list.Contains(o))
            {
                list.Add(o);
            }
        }
        private static void RemoveFromTypeMap(Type type, object o)
        {
            if(mapTypeToObjects.ContainsKey(type))
            {
                List<object> list = mapTypeToObjects[type];
                if (list.Contains(o)) {
                    list.Remove(o);
                    if (list.Count < 1)
                        mapTypeToObjects.Remove(type);
                }
            }
        }
        public static void AddBody(GameObject gameObject, object o)
        {
            lock (map)
            {
                if (!map.ContainsKey(gameObject))
                    map[gameObject] = new List<object>();
                List<object> list=   map[gameObject];
                if(!list.Contains(o))
                {
                    list.Add(o);
                }
                Type type = o.GetType();
                foreach(Type t in Types.GetAllTypes(type))
                {
                    AddToTypeMap(t, o);
                }
            }
        }
        public static void RemoveBody(GameObject gameObject)
        {
            lock (map)
            {
                if(map.ContainsKey(gameObject))
                {
                    List<object> list = map[gameObject];
                    foreach (object o in list)
                    {
                        foreach (Type t in Types.GetAllTypes(o.GetType()))
                        {
                            RemoveFromTypeMap(t, o);
                        }
                    }
                }
                map.Remove(gameObject);
            }
        }
        public static void Clear()
        {
            lock (map)
            {
                map.Clear();
            }
        }
    }

}