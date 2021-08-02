using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public Point Point1;
    public Point Point2;
    public Point Point3;
    public List<Point> vertices = new List<Point>();
    public int id;

    public Tetrahedron parent;
    public Face(Point point1, Point point2, Point point3, Tetrahedron p, int i)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        parent = p;
        vertices.Add(point1);
        vertices.Add(point2);
        vertices.Add(point3);
        id = i;
    }
    public Face(Point point1, Point point2, Point point3)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        vertices.Add(point1);
        vertices.Add(point2);
        vertices.Add(point3);
    }
    public Face(Point point1, Point point2, Point point3, int i)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        vertices.Add(point1);
        vertices.Add(point2);
        vertices.Add(point3);
        id = i;
    }
    public bool IsEqual(Face f)
    {
        if (f == null) 
            return false;

        if (Point1 == f.Point1 && Point2 == f.Point3 && Point3 == f.Point2)
            return true;
        if (Point1 == f.Point1 && Point2 == f.Point2 && Point3 == f.Point3)
            return true;
        if (Point1 == f.Point2 && Point2 == f.Point1 && Point3 == f.Point3)
            return true;
        if (Point1 == f.Point2 && Point2 == f.Point3 && Point3 == f.Point1)
            return true;
        if (Point1 == f.Point3 && Point2 == f.Point1 && Point3 == f.Point2)
            return true;
        if (Point1 == f.Point3 && Point2 == f.Point2 && Point3 == f.Point1)
            return true;

        return false;
    }
    public Vector3 CalculateCentroid()
    {
        return new Vector3((Point1.x + Point2.x + Point3.x) / 3, (Point1.y + Point2.y + Point3.y) / 3, (Point1.z + Point2.z + Point3.z) / 3);
    }

    public void ReverseFace(Face f)
    {
        Vector3 index = f.Point1.getPoint();

        f.Point1.SetP(f.Point2.getPoint());
        f.Point2.SetP(index);

    }
}
