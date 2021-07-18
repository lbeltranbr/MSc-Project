using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public Point Point1;
    public Point Point2;
    public Point Point3;
    public int id;

    public Tetrahedron parent;
    public Face(Point point1, Point point2, Point point3, Tetrahedron p, int i)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        parent = p;
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
}
