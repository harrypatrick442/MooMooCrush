using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MonoBehaviourHelper
    {

        public static Rect GetRectFromMarkers(Transform transform, string[] path)
        {
            foreach (string str in path)
            {
                transform = transform.Find(str);

            }
            Vector2 bottomRight = transform.Find("bottom_right").position;
            Vector2 topLeft = transform.Find("top_left").position;
            return new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
        }
        public static Rect[] GetRectsFromMarkers(Transform transform, string[] path)
        {
            foreach (string str in path)
            {
                transform = transform.Find(str);
            }
            Rect[] rects = new Rect[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform rect = transform.GetChild(i);
                Vector2 bottomRight = rect.Find("bottom_right").position;
                Vector2 topLeft = rect.Find("top_left").position;
                rects[i] = new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, topLeft.y - bottomRight.y);
            }
            return rects;
        }
        public  static Vector2[] GetVectorsFromMarkers(Transform transform, string[] path)
        {
            foreach (string str in path)
            {
                transform = transform.Find(str);

            }
            Vector2[] vectors = new Vector2[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                vectors[i] = transform.GetChild(i).position;
            }
            return vectors;
        }
    }
}