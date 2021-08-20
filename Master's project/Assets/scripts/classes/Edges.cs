using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edges
{
    public Point start;
    public Point end;
    public Vector3 direction;

    public Edges neighbour;

    public float f, g , d;
    public Edges(Point s, Point e)
    {
        start = s;
        end = e;
        direction = (e.getPoint() - s.getPoint());
    }
}
