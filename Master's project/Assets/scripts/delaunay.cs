using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class delaunay 
{
    public List<Point> places;
    public List<Edges> edges;
    public float width, height,depth, ly;
    public Parabola root;

    public SortedSet<Event> deleted;
    public List<Event> queue;
    public delaunay()
    {
        edges = null;
        queue = new List<Event>();
        deleted = new SortedSet<Event>();
    }

    public List<Edges> getEdges(List<Point> v, float w, float h, float d)
    {
        places = v;
        width = w;
        height = h;
        depth = d;
        root = null;

        if (edges == null)
            edges = new List<Edges>();
        else //clear
            edges.Clear();
        

        foreach (Point it in places)
            queue.Add(new Event(it, true));
        
        queue = orderQueue(queue);

        /**********************DEBUG**************/
        //foreach (var it in queue)
          //  Debug.Log(it.y);
        /**********************DEBUG**************/

        Event e;


        while (queue.Count != 0)
        {
            e = queue.Last();

            queue.Remove(e);

            ly = e.point.y;

            if (e.e)
            {
                Debug.Log("Site Event");
                InsertParabola(e.point);
            }
            else
            {
                Debug.Log("Circle Event");
                RemoveParabola(e);
            }

        }

        FinishEdge(root);

        foreach (Edges it in edges)
        {
            if (it.neighbour != null)
            {
                it.start = it.neighbour.end;
                it.neighbour = null;
            }
        }

        return edges;
        
    }
    public void InsertParabola(Point p)
    {

        if (root==null)
        {

            root = new Parabola(p);
            return;
        }
      

        if (root.isLeaf && (root.site.y - p.y < 0.01)) // degenerate case - both bottoms at the same height
        {

            Point fp = root.site;
            root.isLeaf = false;

            root.SetLeft(new Parabola(fp));
            root.SetRight(new Parabola(p));

            Point s = new Point((p.x + fp.x) / 2, height, depth); // the beginning of the edge in the middle of the places


            if (p.x > fp.x)
                root.edge = new Edges(s, fp, p); // I decide which left which right
            else
                root.edge = new Edges(s, p, fp);

            edges.Add(root.edge);

            return;
        }

        Parabola par = GetParabolaByX(p.x);

        if (par.cEvent!=null)
        {
            deleted.Add(par.cEvent);
            par.cEvent = null;
        }

        float temp_y = GetY(par.site, p.x);
        Point start = new Point(p.x, temp_y, GetZ(par.site, p.x, temp_y));

        Edges el = new Edges(start, par.site, p);
        Edges er = new Edges(start, p, par.site);

        el.neighbour = er;
        edges.Add(el);

        // I represent a tree .. I insert a new dish
        par.edge = er;
        par.isLeaf = false;

        Parabola p0 = new Parabola(par.site);
        Parabola p1 = new Parabola(p);
        Parabola p2 = new Parabola(par.site);

        par.SetRight(p2);
        par.SetLeft(new Parabola());
        par.Left().edge = el;

        par.Left().SetLeft(p0);
        par.Left().SetRight(p1);

        CheckCircle(p0);
        CheckCircle(p2);
    }
    public void RemoveParabola(Event e)
    {
        Parabola p1 = e.arch;

        Parabola xl = p1.GetLeftParent(p1);
        Parabola xr = p1.GetRightParent(p1);

        Parabola p0 = xl.GetLeftChild(xl);
        Parabola p2 = xr.GetRightChild(xr);

        if (p0 == p2)
            Debug.LogError("error - the right and left parabolas have the same focus!");

        if (p0.cEvent!=null)
        { 
            deleted.Add(p0.cEvent); 
            p0.cEvent = null;
        }
        if (p2.cEvent!=null) 
        { 
            deleted.Add(p2.cEvent);
            p2.cEvent = null; 
        }

        float temp_y = GetY(p1.site, e.point.x);
        Point p = new Point(e.point.x, temp_y, GetZ(p1.site, e.point.x, temp_y));

        xl.edge.end = p;
        xr.edge.end = p;

        Parabola higher = null;
        Parabola par = p1;

        while (par != root)
        {
            par = par.parent;

            if (par == xl) 
                higher = xl;

            if (par == xr) 
                higher = xr;
        }

        higher.edge = new Edges(p, p0.site, p2.site);
        edges.Add(higher.edge);

        Parabola gparent = p1.parent.parent;

        if (p1.parent.Left() == p1)
        {
            if (gparent.Left() == p1.parent)
                gparent.SetLeft(p1.parent.Right());

            if (gparent.Right() == p1.parent)
                gparent.SetRight(p1.parent.Right());
        }
        else
        {
            if (gparent.Left() == p1.parent) 
                gparent.SetLeft(p1.parent.Left());

            if (gparent.Right() == p1.parent) 
                gparent.SetRight(p1.parent.Left());
        }

        CheckCircle(p0);
        CheckCircle(p2);
    }
    public void FinishEdge(Parabola n)
    {
        if (n.isLeaf) 
        {
            n = null;
            return;
        }

        float mx;

        if (n.edge.direction.x > 0.0) //right
            mx = Mathf.Max(width, n.edge.start.x);//+10
        else 
            mx = Mathf.Min(0.0f, n.edge.start.x);//-10

        Point end = new Point(mx, mx * n.edge.f + n.edge.g,0);
        n.edge.end = end;

        FinishEdge(n.Left());
        FinishEdge(n.Right());
        n = null;
    }
    public Parabola GetParabolaByX(float xx)
    {
        Parabola par = root;
        float x = 0.0f;

        while (!par.isLeaf) // I walk through the tree until I come across a suitable leaf
        {
            x = GetXOfEdge(par, ly);
            if (x > xx) par = par.Left();
            else par = par.Right();
        }
        return par;
    }
    public float GetXOfEdge(Parabola par, float y)
    {
        Parabola left = par.GetLeftChild(par);
        Parabola right = par.GetRightChild(par);

        Point p = left.site;
        Point r = right.site;

        //Eq parabola: f(x) = a * x^2 + b * x + c

        //calculate a,b,c left side

        float dp = 2.0f * (p.y - y);
        float a1 = 1.0f / dp;
        float b1 = -2.0f * p.x / dp;
        float c1 = y + dp / 4 + p.x * p.x / dp;

        //calculate a,b,c right side

        dp = 2.0f * (r.y - y);
        float a2 = 1.0f / dp;
        float b2 = -2.0f * r.x / dp;
        float c2 = y + dp / 4 + r.x * r.x / dp;

        //calculate a,b,c parabola

        float a = a1 - a2;
        float b = b1 - b2;
        float c = c1 - c2;

        //calculate x = -b +- sqrt(b^2 - 4 * a * c) / 2 * a

        float disc = b * b - 4 * a * c;
        float x1 = (-b + Mathf.Sqrt(disc)) / (2 * a);
        float x2 = (-b - Mathf.Sqrt(disc)) / (2 * a);

        float ry;

        if (p.y < r.y) 
            ry = Mathf.Max(x1, x2);
        else 
            ry = Mathf.Min(x1, x2);

        return ry;
    }

    public float GetY(Point p, float x) // focus, x-coordinates
    {
        //Eq parabola: f(x) = a * x^2 + b * x + c

        float dp = 2 * (p.y - ly);
        float a1 = 1 / dp;
        float b1 = -2 * p.x / dp;
        float c1 = ly + dp / 4 + p.x * p.x / dp;

        return (a1 * x * x + b1 * x + c1);
    }
    public float GetZ(Point p, float x, float y) // focus, x-coordinates
    {
        //Eq paraboloid: z = x^2/a^2 + y^2/b^2
        //If a = b is a circular paraboloid
        float dp = 2 * (p.y - ly);
        float a1 = 1 / dp;
        //float b1 = -2 * p.x / dp;
        float b1 = a1;

        return (((x * x) / (a1 * a1)) + ((y * y) / (b1 * b1)));
    }

    public void CheckCircle(Parabola b)
    {
        Parabola a = null, c = null;

        Parabola lp = b.GetLeftParent(b);
        Parabola rp = b.GetRightParent(b);

        if (lp != null)
            a = lp.GetLeftChild(lp);

        if(rp!=null)
            c = rp.GetRightChild(rp);

        if (a == null || c == null || a.site == c.site)
            return;

        Point s = null;
        s = GetEdgeIntersection(lp.edge, rp.edge);

        if (s == null) 
            return;

        float dx = a.site.x - s.x;
        float dy = a.site.y - s.y;

        float d = Mathf.Sqrt((dx * dx) + (dy * dy));

        if (s.y - d >= ly)  
            return; 

        Event e = new Event(new Point(s.x, s.y - d,0), false);
        b.cEvent = e;
        e.arch = b;
        queue.Add(e);
    }

    public Point GetEdgeIntersection(Edges a, Edges b)
    {
        // intersection between edge a and edge b, means that a.y = b.y, a.x = b.x, and a.z = b.z
        // Eq of the plane: ax + by + cz + d = 0

        Vector3 r = Vector3.Cross(a.n, b.n);

        // z = 0
        //float y = (a.d*b.n.x-b.d*a.n.x)/(b.n.y*a.n.x-a.n.y*b.n.x);

        //float x = (-a.n.y * y - a.d) / a.n.x;

        // (x,y,z)+t*r line of intersection
        
        float x = (b.g - a.g) / (a.f - b.f);
        float y = a.f * x + a.g;

        if ((x - a.start.x) / a.direction.x < 0) 
            return null;

        if ((y - a.start.y) / a.direction.y < 0) 
            return null;

        if ((x - b.start.x) / b.direction.x < 0) 
            return null;
        if ((y - b.start.y) / b.direction.y < 0) 
            return null;

        Point p = new Point(x, y, 0);
        return p;
    }
    public List<Event> orderQueue(List<Event> q)
    {
        q = q.OrderBy(w => w.y).ToList();
        
        return q;
    }
}
