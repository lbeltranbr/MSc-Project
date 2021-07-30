using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maths 
{
   /* public bool LinePlaneIntersection(Vector3 start, Vector3 dir, Vector3 plane_p, Vector3 normal)
    {
        float D = Vector3.Dot(normal, dir);
        float N = -Vector3.Dot(normal, start - plane_p);

        if (Mathf.Abs(D) < .000001f)
            return false;

        float sI = N / D;
        if (sI < 0 || sI > 1)
            return false;

        return true;
    }*/

    public static Plane CalcMiddlePlane(Point p1, Point p2)
    {
        Vector3 m = (p1.getPoint() + p2.getPoint()) / 2;
        Vector3 p = p2.getPoint() - p1.getPoint();

        return new Plane(p.normalized, m);
    }

    public static Point CalcPlaneIntersection(Plane p1, Plane p2, Plane p3)
    {
        var det = Vector3.Dot(Vector3.Cross(p1.normal, p2.normal), p3.normal);
        if (det < 0.000001)
        {
            return null;
        }

       Vector3 intersectionPoint =
            (-(p1.distance * Vector3.Cross(p2.normal, p3.normal)) -
            (p2.distance * Vector3.Cross(p3.normal, p1.normal)) -
            (p3.distance * Vector3.Cross(p1.normal, p2.normal))) / det;

        return new Point(intersectionPoint);

    }


    public static Line CalcLine(Point p1, Point p2, Point p3)
    {
        Plane plane1 = CalcMiddlePlane(p1, p2);
        Plane plane2 = CalcMiddlePlane(p1, p3);
        Plane plane3 = new Plane(p1.getPoint(), p2.getPoint(), p3.getPoint());

        Point p = CalcPlaneIntersection(plane1, plane2, plane3);
        Point dir = new Point(plane3.normal);
        return new Line(p, dir);
    }

}
