using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sweep 
{
    public List<Point> sites;
    public List<Event> queue;
    public Paraboloid root;

    public Sweep(List<Point> p, Vector3 obj_pos, Vector3 obj_scale, bool d)
    {
        sites = p;

        foreach (Point it in sites)
            queue.Add(new Event(it, true));
        //order points from lowest to highest y axis
        orderQueue(queue);
        root = null;

        SweepPlane();

    }
    public List<Event> orderQueue(List<Event> q)
    {
        return q.OrderBy(w => w.y).ToList();
    }
    public void SweepPlane()
    {
        while (queue.Count != 0)
        {
            Event e = queue.Last(); // highest y axis
            queue.Remove(e);

            if (e.isPointEvent)
            {
                AddParaboloid(e.point);
            }
            else
            {

            }
        }
    }

    public void AddParaboloid(Point p)
    {
        if (root == null)
        {
            root = new Paraboloid(p);
            return;
        }
    }

}
