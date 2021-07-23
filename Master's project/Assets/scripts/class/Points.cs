using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public float x, y, z;
    public bool inP1;
    public Point(float xpos,float ypos, float zpos)
    {
        x = xpos;
        y = ypos;
        z = zpos;
    }
    public Point(Vector3 p)
    {
        x = p.x;
        y = p.y;
        z = p.z;
    }  
 
    public Vector3 getPoint()
    {
        return new Vector3(x, y, z);
    }
  
}
