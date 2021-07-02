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

    public Edges neighbour;

    public float f, g;
    public void setEdges(Point s, Point l, Point r)
    {
        start = s;
        left = l;
        right = r;
        neighbour = null;
        end = null;

        f = (r.x - l.x) / (l.y - r.y);
        g = s.y - (f * s.x);
    }
}
