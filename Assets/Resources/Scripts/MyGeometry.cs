using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MyGeometry
    {
        public static bool Intersect(Rect rectA, Rect rectB)
        {
            return (Mathf.Abs(rectA.x - rectB.x) < (Mathf.Abs(rectA.width + rectB.width) / 2))
                 && (Mathf.Abs(rectA.y - rectB.y) < (Mathf.Abs(rectA.height + rectB.height) / 2));
        }
        public static bool Contains(Rect rectA, Rect rectB)
        {
            float aX = rectA.x - (rectA.width / 2);
            float bX = rectB.x - (rectB.width / 2);
            float aY = rectA.y - (rectA.height / 2);
            float bY = rectB.y - (rectB.height / 2);
            return aX <= bX && (aX + rectA.width) >= bX + rectB.width
                 && aY <= bY && aY + rectA.height >= rectB.height + bY;
        }
        public static Rect GetRect(GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            Vector3 size = renderer.bounds.size;
            return new Rect(
               gameObject.transform.position.x,
                gameObject.transform.position.y,
                size.x * gameObject.transform.localScale.x,
                size.y * gameObject.transform.localScale.y);
        }
        public static Vector3 GetWorldScale(Transform transform)
        {
            Vector3 worldScale = transform.localScale;
            Transform parent = transform.parent;

            while (parent != null)
            {
                worldScale = Vector3.Scale(worldScale, parent.localScale);
                parent = parent.parent;
            }

            return worldScale;
        }
        public static Rect GetWorldRect(RectTransform rt)
        {
            Vector2 scale = GetWorldScale(rt);
            // Convert the rectangle to world corners and grab the top left
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector3 topLeft = corners[0];

            // Rescale the size appropriately based on the current Canvas scale
            Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

            return new Rect(topLeft, scaledSize);
        }
        public static float UnitsPerPixel(Camera cam)
        {
            var p1 = cam.ScreenToWorldPoint(Vector3.zero);
            var p2 = cam.ScreenToWorldPoint(Vector3.right);
            return Vector3.Distance(p1, p2);
        }

        public static float PixelsPerUnit(Camera cam)
        {
            return 1 / UnitsPerPixel(cam);
        }
        public static Vector2? LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                return null;

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }
        public static List<Vector2> GetLineIntersectRectanglePoints(Rect rect, Tuple<Vector2, Vector2> line)
        {
            List<Vector2> ints = new List<Vector2>();
            float left = rect.x;
            float right = rect.x + rect.width;
            float top = rect.y;
            float bottom = rect.y - rect.height;
            Vector2 topLeft = new Vector2(left, top);
            Vector2 topRight = new Vector2(right, top);
            Vector2 bottomLeft = new Vector2(left, bottom);
            Vector2 bottomRight = new Vector2(right, bottom);
            foreach (Tuple<Vector2, Vector2> tuple in new Tuple<Vector2, Vector2>[] {
            new Tuple<Vector2, Vector2>(topLeft, topRight),
            new Tuple<Vector2, Vector2>(topRight, bottomRight),
            new Tuple<Vector2, Vector2>(bottomRight, bottomLeft),
            new Tuple<Vector2, Vector2>(bottomLeft, topLeft)
            })
            {
                Vector2? lineIntersectionPoint = LineIntersectionPoint(tuple.A, tuple.B, line.A, line.B);
                if (lineIntersectionPoint != null)
                {
                    ints.Add((Vector2)lineIntersectionPoint);
                }
            }
            return ints;
        }
        public static Vector2 GetClosestPoint(List<Vector2> points, Vector2 point)
        {

            Vector2 closestPoint = points[0];
            float magnitude = (closestPoint - point).magnitude;
            foreach (Vector2 p in points)
            {
                if (!closestPoint.Equals(p))
                {
                    float m = (p - point).magnitude;
                    if (m < magnitude)
                    {
                        magnitude
                            = m;
                        closestPoint = p;
                    }
                }
            }
            return closestPoint;
        }
        public static Vector2 GetFurthestPoint(List<Vector2> points, Vector2 point)
        {

            Vector2 furthestPoint = points[0];
            float magnitude = (furthestPoint - point).magnitude;
            foreach (Vector2 p in points)
            {
                if (!furthestPoint.Equals(p))
                {
                    float m = (p - point).magnitude;
                    if (m > magnitude)
                    {
                        magnitude
                            = m;
                        furthestPoint = p;
                    }
                }
            }
            return furthestPoint;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="from">OUTSIDE POINT</param>
        /// <returns></returns>
        public static Vector2 GetClosestLineIntersectRectanglePoints(Rect rect, Vector2 from)
        {
            List<Vector2> ints = new List<Vector2>();
            float left = rect.x;
            float right = rect.x + rect.width;
            float top = rect.y;
            float bottom = rect.y - rect.height;
            Vector2 topLeft = new Vector2(left, top);
            Vector2 topRight = new Vector2(right, top);
            Vector2 bottomLeft = new Vector2(left, bottom);
            Vector2 bottomRight = new Vector2(right, bottom);
            Vector2 closestPoint = GetClosestPoint(new List<Vector2>() { topLeft, topRight, bottomLeft, bottomRight }, from);
            Vector2 connectedToX;
            Vector2 connectedToY;
            Vector2 connectedFromX;
            Vector2 connectedFromY;
            if (closestPoint.Equals(topLeft))
            {
                connectedToY = topLeft;
                connectedFromY = bottomLeft;
                connectedToX = topRight;
                connectedFromX = topLeft;
            }
            else
            {
                if (closestPoint.Equals(topRight))
                {
                    connectedToY = topRight;
                    connectedFromY = bottomRight;
                    connectedToX = topRight;
                    connectedFromX = topLeft;
                }
                else
                {


                    if (closestPoint.Equals(bottomLeft))
                    {
                        connectedToX = bottomRight;
                        connectedFromX = bottomLeft;
                        connectedToY = topLeft;
                        connectedFromY = bottomLeft;
                    }
                    else
                    {
                        connectedToX = bottomRight;
                        connectedFromX = bottomLeft;
                        connectedToY = topRight;
                        connectedFromY = bottomRight;
                    }
                }
            }
            //Debug.Log(from.x);
            //Debug.Log(from.y);
            //Debug.Log(connectedFromX);
            //Debug.Log(connectedToX);
            //Debug.Log(connectedFromY);
            //Debug.Log(connectedToY);
            if ((from.x >= connectedFromX.x && from.x <= connectedToX.x))
            {
                if (from.y >= connectedFromY.y && from.y <= connectedToY.y)
                {
                    return from;
                }
                else
                    return new Vector2(from.x, closestPoint.y);
            }
            if (from.y >= connectedFromY.y && from.y <= connectedToY.y)
            {
                return new Vector2(closestPoint.x, from.y);
            }
            return closestPoint;
        }
        public enum TrigFunction { Sin, Cos, Tan }
        public static float GetSign(Angle angle, TrigFunction trigFunction)
        {
            switch (trigFunction)
            {
                case TrigFunction.Cos:
                    return angle.Radians < Mathf.PI ? 1 : -1;
                case TrigFunction.Tan:
                    return angle.Radians <= (Mathf.PI / 2) || (angle.Radians >= Math.PI && angle.Radians < 3 * Math.PI / 2) ? 1 : -1;
                default:
                    return angle.Radians <= (Mathf.PI / 2) || (angle.Radians >= 3 * Math.PI / 2) ? 1 : -1;
            }
        }
        public static Angle GetRealSinAngle(Angle angle, float dX, float dY)
        {

            if (dX >= 0)
            {
                if (dY >= 0)
                {//quadrant 1
                 //no transformation, do nothing
                }
                else
                { //quadrant 4
                    angle.Radians = (2 * Mathf.PI) + angle.Radians;
                }
            }
            else
            { //quadrant 3
                      //quadrant 2
                        angle.Radians = Mathf.PI - angle.Radians;
            }
            return angle;

        }
        public static Angle GetRealCosAngle(Angle angle, float dX, float dY)
        {
            if (dX >= 0)
            {
                if (dY >= 0)
                {//quadrant 1
                 //no transformation, do nothing
                }
                else
                { //quadrant 4
                    angle.Radians = Mathf.PI - angle.Radians;
                }
            }
            else
            {
                    if (dY > 0)
                    { //quadrant 3

                        angle.Radians = -angle.Radians;
                    } else
                    {
                        //quadrant 2
                        angle.Radians = angle.Radians - Mathf.PI;
                }
            }
            return angle;
        }
        public static Vector2 GetFurthestLineIntersectRectanglePoints(Rect rect, Vector2 from)
        {
            List<Vector2> ints = new List<Vector2>();
            float left = rect.x;
            float right = rect.x + rect.width;
            float top = rect.y;
            float bottom = rect.y - rect.height;
            Vector2 topLeft = new Vector2(left, top);
            Vector2 topRight = new Vector2(right, top);
            Vector2 bottomLeft = new Vector2(left, bottom);
            Vector2 bottomRight = new Vector2(right, bottom);
            return GetFurthestPoint(new List<Vector2>() { topLeft, topRight, bottomLeft, bottomRight }, from);
        }
        public class LineEquation
        {
            public float A, B, C;
            public LineEquation(float a, float b, float c)
            {
                A = a;
                B = b;
                C = c;
            }
        }
        public static LineEquation GetLineEquation(Vector2 a, Vector2 b)
        {
            float dX = b.x - a.x;
            if (dX != 0)
            {
                float m = (b.y - a.y) / (dX);
                return new LineEquation(-m, 1, -(a.y - (m * a.x)));
            }
            return new LineEquation(-1/a.x, 0, 1);
        }
       public static List<Tuple<Vector2, Angle>>GetArcIntersecsWithLine(float length, Vector2 from, Vector2 lineFrom, Vector2 lineTo)
        {//Angles anticlockwise from x
            LineEquation lineEquation = GetLineEquation(lineFrom, lineTo);
            float A = lineEquation.A;
            float B = lineEquation.B;
            float C = lineEquation.C;
            float XC = from.x;
            float YC = from.y;
            float AXc = A*XC;
            float BYC = B * YC;
            float R = length;
            float AR = A*R;
            float BR = B * R;
            float s = (4 * Mathf.Pow(B, 2) * Mathf.Pow(R, 2) - (4 * (-AR + AXc + BYC + C) * (AR + AXc + BYC + C)));
            // and - A R + A X + B Y + C != 0 and R (A ^ 2 R - A ^ 2 X - 1 / 2 B sqrt(4 B ^ 2 R ^ 2 - 4(-A R + A X + B Y + C)(A R + A X + B Y + C)) - A B Y - A C + B ^ 2 R)!= 0 and n element Z
            List<Tuple<Vector2, Angle>> list = new List<Tuple<Vector2, Angle>>();
            float denominator = (-AR + AXc + BYC + C);
            if (denominator != 0&&s>=0)
            {
                float xMin;
                float xMax;
                if (lineFrom.x < lineTo.x)
                {

                    xMin = lineFrom.x;
                    xMax = lineTo.x;
                }
                else
                {
                    xMin = lineTo.x;
                    xMax = lineFrom.x;
                }
                float yMin;
                float yMax;
                if (lineFrom.y < lineTo.y)
                {

                    yMin = lineFrom.y;
                    yMax = lineTo.y;
                }
                else
                {
                    yMin = lineTo.y;
                    yMax = lineFrom.y;
                }
                float Q = 0.5f * Mathf.Sqrt(s);
                Angle o0 = new Angle(2 * (Mathf.Atan((Q - BR) / denominator)));
                Vector2 point0 = from + new Vector2(Mathf.Cos(o0.Radians) * R, Mathf.Sin(o0.Radians) * R);
            if((point0.x>=xMin||MyMaths.Approximately(point0.x, xMin, 0.0001f)) && (point0.x <= xMax|| MyMaths.Approximately(point0.x, xMax, 0.0001f)) && (point0.y >= yMin || MyMaths.Approximately(point0.y, yMin, 0.0001f)) && (point0.y <= yMax || MyMaths.Approximately(point0.y, yMax, 0.0001f)))
                {
                    list.Add(new Tuple<Vector2, Angle>(point0, o0));
                }
                if (Q != 0)
                {
                    Angle o1 = new Angle(2 * (Mathf.Atan((-Q - BR) / denominator)));
                    Vector2 point1 = new Vector2(Mathf.Cos(o1.Radians) * R, Mathf.Sin(o1.Radians) * R);
                    if ((point1.x >= xMin || MyMaths.Approximately(point1.x, xMin, 0.0001f)) && (point1.x <= xMax || MyMaths.Approximately(point1.x, xMax, 0.0001f)) && (point1.y >= yMin || MyMaths.Approximately(point1.y, yMin, 0.0001f)) && (point1.y <= yMax || MyMaths.Approximately(point1.y, yMax, 0.0001f)))                    {
                        list.Add(new Tuple<Vector2, Angle>(point1, o1));
                    }
                }
            }
            return list;
        }
        public static Vector2? GetPointInRectForLineLengthX(Rect rect, Vector2 from, float magnitude)
        {
            Vector2 q = new Vector2(rect.x, rect.y);
            Vector2 r = new Vector2(rect.x + rect.width, rect.y);
            Vector2 s = new Vector2(rect.x + rect.width, rect.y - rect.height);
            Vector2 t = new Vector2(rect.x, rect.y - rect.height);
            List<Tuple<Vector2, Angle>> edges = GetArcIntersecsWithLine(magnitude, from, q, r);
            edges.AddRange(GetArcIntersecsWithLine(magnitude, from, r, s));
            edges.AddRange(GetArcIntersecsWithLine(magnitude, from, s, t));
            edges.AddRange(GetArcIntersecsWithLine(magnitude, from, t, q));

            if (edges.Count > 0)
            {


                #region pair edges by angle
                edges.Sort(new Comparison<Tuple<Vector2, Angle>>((a, b) =>
                {
                    if (a.B.Radians > b.B.Radians)
                        return 1;
                    else
                        if (a.B.Radians < b.B.Radians)
                        return -1;
                    return 0;
                }));
                if (edges.Count < 2)
                    return edges[0].A;
                List<Tuple<float, float>> pairs = new List<Tuple<float, float>>();
                float angleB = edges[0].B.Radians;
                float angleA;
                int i = 1;
                int count = edges.Count;
                while ( i < count) {
                    angleA = angleB;
                    angleB = edges[i].B.Radians;
                    float halfWayAngle = ( angleA + angleB) / 2;
                    Vector2 testHalfWayPoint = new Vector2(from.x + (Mathf.Cos(halfWayAngle) * magnitude), from.y + (Mathf.Sin(halfWayAngle) * magnitude));
                    if (testHalfWayPoint.x >= rect.x && testHalfWayPoint.x <= rect.x + rect.width && testHalfWayPoint.y <= rect.y && testHalfWayPoint.y >= rect.y - rect.height)
                    {
                       pairs.Add(new Tuple<float, float>(angleA, angleB));
                    }
                    i++;
                }
                #endregion
                #region pick random angle in range
                if (pairs.Count > 0)
                {
                    float probability = 100f / pairs.Count;
                    Tuple<float, float> pair = pairs.Count > 1 ?
                        new RouletteWheel<Tuple<float, float>>(
                        (from a in pairs select new RouletteSlot<Tuple<float, float>>(probability, a)).ToArray()).Spin()
                        : pairs[0];
                    float angle = Random.Range(pair.A, pair.B);
                    #endregion
                    return new Vector2((Mathf.Cos(angle) * magnitude) + from.x, (Mathf.Sin(angle) * magnitude) + from.y);
                }
            }
            return null;
        }
        public static Tuple<Rect, bool> GetWithinBoundsRect(Rect toTrim, Rect toFitInside)
        {
            float top;
            float bottom;
            float left;
            float right;
            bool compromised = false;
            if (toTrim.height < toFitInside.height)
            {
                Debug.Log("Compromised"); //")
                compromised = true;
                top = toTrim.y - (toTrim.height / 2);
                bottom = top;
            }
            else
            {
                float a = (toFitInside.height / 2);
                top = toTrim.y - a;
                bottom = toTrim.y + a - toTrim.height;
            }
            if (toTrim.width < toFitInside.width)
            {
                Debug.Log("Compromised"); //")
                compromised = true;
                left = toTrim.x + (toTrim.width / 2);
                right = left;
            }
            else
            {
                left = toTrim.x + (toFitInside.width / 2);
                right = toTrim.x + toTrim.width - (toFitInside.width / 2);
            }
            return new Tuple<Rect, bool>(new Rect(left, top, (right - left), (top - bottom)), compromised);
        }
        public static float GetAngleDifferenceDegrees(float from, float to, bool clockwise)
        {
            if (clockwise)
            {
                while (to > from)
                    to -= 360;
                while (to < from)
                    to += 360;
            }
            else
            {
                while (to < from)
                    to += 360;
                while (to > from)
                    to -= 360;
            }
            return to - from;
        }
        /// <summary>
        /// -
        /// </summary>
        /// <param name="position"></param>
        /// <param name="allowedRegionsUncropped"></param>
        /// <param name="minDistanceFromTarget"></param>
        /// <param name="maxDistanceFromTar/get"></param>
        /// <param name="iGetRect"></param>
        /// <param name="steppedDistances"></param>
        /// <returns></returns>
        public static Vector2? GetCloseTo(Vector2 position, Rect[] allowedRegionsUncropped, float minDistanceFromTarget, float maxDistanceFromTarget, IGetRect iGetRect, List<float> steppedDistances)
        {
            List<Rect> allowedRegions = new List<Rect>();
            Rect? rect = iGetRect.GetRect();
            if (rect == null)
                return null;
            foreach (Rect region in allowedRegionsUncropped)
            {
                Tuple<Rect, bool> c = MyGeometry.GetWithinBoundsRect((Rect)region, (Rect)rect);
                if (!c.B)
                    allowedRegions.Add(c.A);
            }
            List<Tuple<Rect, Tuple<float, float>>> suitableRegions = new List<Tuple<Rect, Tuple<float, float>>>();
            foreach (Rect region in allowedRegions)
            {
                Vector2? closestIntersect = GetClosestLineIntersectRectanglePoints(region, position);
                Vector2? furthestIntersect = GetFurthestLineIntersectRectanglePoints(region, position);
                if (closestIntersect != null)
                {
                    float minLength = ((Vector2)closestIntersect - position).magnitude;
                    float maxLength = ((Vector2)furthestIntersect - position).magnitude;
                    if (minLength >= minDistanceFromTarget)
                    {
                        if (minLength <= maxDistanceFromTarget)
                        {
                            suitableRegions.Add(new Tuple<Rect, Tuple<float, float>>(region, new Tuple<float, float>(minLength, maxLength > maxDistanceFromTarget ? maxDistanceFromTarget : maxLength)));
                        }
                    }
                    else
                    {
                        if (maxLength >= minDistanceFromTarget)
                        {
                            suitableRegions.Add(new Tuple<Rect, Tuple<float, float>>(region, new Tuple<float, float>(minLength < minDistanceFromTarget ? minDistanceFromTarget : minLength, maxLength > maxDistanceFromTarget ? maxDistanceFromTarget : maxLength)));
                        }
                    }
                }
            }
            while (suitableRegions.Count > 0)
            {
                Tuple<Rect, Tuple<float, float>> t = suitableRegions[Random.Range(0, suitableRegions.Count)];
                Rect region = t.A;
                float distance = Random.Range(t.B.A, t.B.B);
                Vector2? toPosition = MyGeometry.GetPointInRectForLineLengthX(region, position, distance);
                if (toPosition != null)
                {//Should be true
                    return (Vector2)toPosition;
                }
                suitableRegions.RemoveAt(0);
            }
            return null;
        }
    }
}