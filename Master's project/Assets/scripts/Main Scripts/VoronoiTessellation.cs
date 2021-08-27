using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiTessellation 
{
    public List<Point> vertices;
    public Plane plane;

    public VoronoiTessellation(List<Point> v, Plane p)
    {
        vertices = v;
        plane = p;
    }

}
