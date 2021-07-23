using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Event 
{
    public Point point;
    public bool isPointEvent;
    public float y;
    public Parabola arch;

    public Event(Point p, bool e)
    {
        point = p;
        isPointEvent = e;
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
