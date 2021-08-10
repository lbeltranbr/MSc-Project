using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planes
{
    public Vector3 normal;
    public Vector3 point;
    public float offset;
    
    public Planes(Vector3 n, Vector3 p)
    {
        normal = n;
        point = p;
        offset = Vector3.Dot(n, p);
    }

    public Planes(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3 v1 = p1 - p0;
        Vector3 v2 = p2 - p0;

        Vector3 n = Vector3.Cross(v1, v2);
        n = Vector3.Normalize(n);
        normal = n;
        point = p0;
        offset = Vector3.Dot(n, p0);

    }
    public bool GetSide(Vector3 v)
    {
        if (Vector3.Dot(v, normal) > offset)
            return true;
        else 
            return false;
    }
}
