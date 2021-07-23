using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edges
{
    public Point start;
    public Point end;
    public Point direction;
    public Point left;
    public Point right;
    public Vector3 n;
    public Plane plane;

    public Edges neighbour;

    public float f, g , d;
    public Edges(Point s, Point l, Point r)
    {
        start = s;
        left = l;
        right = r;
        neighbour = null;
        end = null;

        // line equation y = f * x + g
        // plane equation ax + by + cz + d = 0
        Vector3 sl = new Vector3(l.x-s.x,l.y-s.y,l.z-s.z);
        Vector3 sr = new Vector3(r.x-s.x,r.y-s.y,r.z-s.z);

        n = Vector3.Cross(sr, sl).normalized; //a,b,c

        d = -(n.x * s.x + n.y * s.y + n.z * s.z);


        f = (r.x - l.x) / (l.y - r.y); //slope
        g = s.y - (f * s.x);// y axis intersection


        //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        
        direction = new Point(r.y - l.y, -(r.x - l.x), r.z - l.z);
    }
}
