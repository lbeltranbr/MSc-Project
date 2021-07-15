using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public Point Point1;
    public Point Point2;
    public Point Point3;

    public Tetrahedron left;
    public Tetrahedron right;
    public Face(Point point1, Point point2, Point point3, Tetrahedron parent)
    {
        Point1 = point1;
        Point2 = point2;
        Point3 = point3;
        left = parent;
    }
}
