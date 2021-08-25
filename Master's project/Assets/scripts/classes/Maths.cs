using UnityEngine;

public class Maths
{
    public static Point LinePlaneIntersection(Line line, Vector3 plane_p, Vector3 normal)
    {
        float t = -Vector3.Dot(plane_p, normal) + Vector3.Dot(line.start.getPoint(), normal);
        float t1 = Vector3.Dot(line.direction.getPoint(), normal);

        if (Mathf.Abs(t1) < .000001f)
            return null;

        t = -(t / t1);
        Vector3 p = line.start.getPoint() + line.direction.getPoint() * t;

        return new Point(p);

    }

    public static Plane CalcMiddlePlane(Point p1, Point p2)
    {
        Vector3 m = (p1.getPoint() + p2.getPoint()) / 2;
        Vector3 p = p2.getPoint() - p1.getPoint();

        return new Plane(p.normalized, m);
    }

    public static Vector3 GetPointofMiddlePlane(Point p1, Point p2)
    {
        Vector3 m = (p1.getPoint() + p2.getPoint()) / 2;

        return m;
    }
    public static Point CalcPlaneIntersection(Plane p1, Plane p2, Plane p3)
    {
        var det = Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal));

        if (det < 0.000001)
            return null;


        Vector3 intersectionPoint = (-(p1.distance * Vector3.Cross(p2.normal, p3.normal)) - (p2.distance * Vector3.Cross(p3.normal, p1.normal)) - (p3.distance * Vector3.Cross(p1.normal, p2.normal))) / det;

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
