using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola
{
    public bool isLeaf;
    public Point site;
    public Edges edge;
    public Event cEvent;
    public Parabola parent;

    private Parabola left, right;

    public Parabola()
    {
        site = null;
        isLeaf = true;
        cEvent = null;
        edge = null;
        parent = null;
    }
    public Parabola(Point s)
    {
        site = s;
        isLeaf = true;
        cEvent = null;
        edge = null;
        parent = null;
    }
    public void SetLeft( Parabola p)
    {
        left = p;
        p.parent = this;
    }
    public void SetRight(Parabola p)
    {
        right = p;
        p.parent = this;
    }

    public Parabola GetLeft(Parabola p)
    {
        return GetLeftChild(GetLeftParent(p));
    }
    public Parabola GetRight(Parabola p)
    {
        return GetRightChild(GetRightParent(p));
    }
    public Parabola GetLeftParent(Parabola p)
    {
        Parabola par = p.parent;
        Parabola pLast = p;
        while (par.left == pLast)
        {
            if (par.parent==null) return null;
            pLast = par;
            par = par.parent;
        }
        return par;
    }
    public Parabola GetRightParent(Parabola p)
    {
        Parabola par = p.parent;
        Parabola pLast = p;
        while (par.right == pLast)
        {
            if (par.parent == null) return null;
            pLast = par;
            par = par.parent;
        }
        return par;
    }
    public Parabola GetLeftChild(Parabola p)
    {
        if (p==null) return null;
        Parabola par = p.left;
        while (!par.isLeaf) 
            par = par.right;
        return par;
    }
    public Parabola GetRightChild(Parabola p)
    {
        if (p == null) return null;
        Parabola par = p.right;
        while (!par.isLeaf)
            par = par.left;
        return par;
    }
}
