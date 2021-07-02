using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public float x, y, z;
    public Point(float xpos,float ypos, float zpos)
    {
        x = xpos;
        y = ypos;
        z = zpos;
    }
    public Vector3 getPoint()
    {
        return new Vector3(x, y, z);
    }
  
}
