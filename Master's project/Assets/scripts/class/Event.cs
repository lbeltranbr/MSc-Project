using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event 
{
    public Point point;
    public bool e;
    public float y;
    public Parabola arch;

    public Event(Point p, bool ev)
    {
        point = p;
        e = ev;
        y = p.y;
        arch = null;
    }

    public bool compareEvent(Event l, Event r)
    {
        if (l.y < r.y)
            return true;
        else
            return false;

    }

}
