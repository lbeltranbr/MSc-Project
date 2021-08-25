using UnityEngine;

public class Point
{
    public float x, y, z;
    public int id;
    public Point(float xpos, float ypos, float zpos)
    {
        x = xpos;
        y = ypos;
        z = zpos;
    }
    public Point(float xpos, float ypos, float zpos, int i)
    {
        x = xpos;
        y = ypos;
        z = zpos;
        id = i;
    }
    public Point(Vector3 p)
    {
        x = p.x;
        y = p.y;
        z = p.z;
    }
    public Point(Vector3 p, int i)
    {
        x = p.x;
        y = p.y;
        z = p.z;
        id = i;

    }

    public Vector3 getPoint()
    {
        return new Vector3(x, y, z);
    }
    public void SetP(Vector3 p)
    {
        x = p.x;
        y = p.y;
        z = p.z;
    }

}
