using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paraboloid
{
    public Point focus;
    public Paraboloid left, right, parent;

    public Paraboloid(Point p)
    {
        focus = p;
    }
    public void SetLeft(Paraboloid p)
    {
        left = p;
        left.parent = this;
    }
    public void SetRight(Paraboloid p)
    {
        right = p;
        right.parent = this;

    }
}
