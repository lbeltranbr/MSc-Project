using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideAndConquer 
{

    /*
    Compute the convex hull of the given points, P (or use the verts themselves)

    Compute the cutting plane, a (take the average of all numbers along an axis)

    Divide the input points, P, along a into two subsets: P1 and P2

    Choose the point in P1 and P2 closest to the cutting plane, p1 and p2

    Choose the point in P, p3, that makes triangle p1-p2-p3 with the smallest circumcircle

    Add all edges not present into their corresponding Active-Face-List, AFL, (else remove the duplicate)

    For each face in the AFL that intersects the cutting plane, use it to build a new minimum triangle

    Continue until no more edges remain intersecting a

    Repeat the process recursively, subdividing P1 and P2 further until all AFL's are empty.

    */

    public Plane a;
    public List<Point> p1, p2;
    public List<Face> AFLa, AFL1, AFL2;

    private bool debug;

    public DivideAndConquer(List<Point> points, Vector3 obj_pos, Vector3 obj_scale, bool d)
    {
        a = new Plane();
        p1 = new List<Point>();
        p2 = new List<Point>();

        AFLa = new List<Face>();
        AFL1 = new List<Face>();
        AFL2 = new List<Face>();

        debug = d;

        List<Face> AFL = new List<Face>();

        DeWall(points, AFL, obj_pos, obj_scale);
    }
    public void DeWall(List<Point> points, List<Face> AFL, Vector3 obj_pos, Vector3 obj_scale)
    {
        SetPlane(obj_pos, obj_scale);
        PointPartition(points);

        if (AFL.Count == 0)
        {
            Face f = MakeFirstSimplex(points);
            AFL.Add(f);
        }

        AFLa.Clear();
        AFL1.Clear();
        AFL2.Clear();

        float enter = 0.0f;

        foreach (Face f in AFL)
        {
            Vector3 ab = f.Point2.getPoint() - f.Point1.getPoint();
            Vector3 bc = f.Point3.getPoint() - f.Point2.getPoint();
            Vector3 ca = f.Point1.getPoint() - f.Point3.getPoint();
            Ray r1 = new Ray(f.Point1.getPoint(), ab);
            Ray r2 = new Ray(f.Point2.getPoint(), bc);
            Ray r3 = new Ray(f.Point3.getPoint(), ca);

            if (a.Raycast(r1, out enter) || a.Raycast(r2, out enter) || a.Raycast(r3, out enter))
                AFLa.Add(f);

            foreach(Point p in f.vertices)
            {
                if (p.inP1&&!AFL1.Contains(f))
                    AFL1.Add(f);
                if (!p.inP1&&!AFL2.Contains(f))
                    AFL2.Add(f);
            }
        }

        while (AFLa.Count != 0)
        {
            Face f = AFLa[0];
            AFLa.Remove(f);

            MakeSimplex(f, points);
        }


    }

    public void SetPlane(Vector3 obj_pos, Vector3 obj_scale)
    {
        Vector3 p = new Vector3((obj_scale.x / 2f) + obj_pos.x, obj_pos.y, (obj_scale.z / 2f) + obj_pos.z);

        if (obj_scale.x >= obj_scale.z)
            a.SetNormalAndPosition(new Vector3(1f, 0, 0), p);
        else
            a.SetNormalAndPosition(new Vector3(0, 0, 1f), p);
    }
    public void PointPartition(List<Point> points)
    {
        foreach(Point p in points)
        {
            if (a.GetSide(p.getPoint()))
            {
                p.inP1 = true;
                p1.Add(p); //positive side of the plane
            }
            else
            {
                p.inP1 = false;
                p2.Add(p); //negative side of the plane
            }
        }
    }

    public Face MakeFirstSimplex(List<Point> points)
    {
        Point point1, point2, point3;
        float dist = a.GetDistanceToPoint(points[0].getPoint());
        int index = 0;
        float rad = 0;
        // Selects the point nearest to the plane a
        foreach(Point p in points)
        {
            float p_dist = a.GetDistanceToPoint(p.getPoint());
            if (p_dist < dist)
            {
                index = points.IndexOf(p);
                dist = p_dist;
            }
        }
        point1 = new Point(points[index].getPoint());

        // Selects the point nearest to the first point on the other side of the plane

        if (point1.inP1)
        {
            dist = Vector3.Distance(point1.getPoint(), p2[0].getPoint());
            foreach (Point p in p2)
            {
                float p_dist = Vector3.Distance(point1.getPoint(), p.getPoint());
                if (p_dist < dist)
                {
                    index = p2.IndexOf(p);
                    dist = p_dist;
                }
            }
            point2 = new Point(p2[index].getPoint());

        }
        else
        {
            dist = Vector3.Distance(point1.getPoint(), p1[0].getPoint());
            foreach (Point p in p1)
            {
                float p_dist = Vector3.Distance(point1.getPoint(), p.getPoint());
                if (p_dist < dist)
                {
                    index = p1.IndexOf(p);
                    dist = p_dist;
                }
            }
            point2 = new Point(p1[index].getPoint());

        }

        // Selects the point with the min circumradius
        if (point1.inP1)
        {
            Vector3 ab = point2.getPoint() - point1.getPoint();
            Vector3 bc = p2[0].getPoint() - point2.getPoint();
            Vector3 ca = point1.getPoint() - p2[0].getPoint();

            rad = GetCircumradius(ab.magnitude, bc.magnitude, ca.magnitude);

            foreach (Point p in p2)
            {
                bc = p.getPoint() - point2.getPoint();
                ca = point1.getPoint() - p.getPoint();

                float p_rad = GetCircumradius(ab.magnitude, bc.magnitude, ca.magnitude); 
                if (p_rad < rad)
                {
                    index = p2.IndexOf(p);
                    rad = p_rad;
                }
            }
            point3 = new Point(p2[index].getPoint());

        }
        else
        {
            Vector3 ab = point2.getPoint() - point1.getPoint();
            Vector3 bc = p1[0].getPoint() - point2.getPoint();
            Vector3 ca = point1.getPoint() - p1[0].getPoint();

            rad = GetCircumradius(ab.magnitude, bc.magnitude, ca.magnitude);

            foreach (Point p in p1)
            {
                bc = p.getPoint() - point2.getPoint();
                ca = point1.getPoint() - p.getPoint();

                float p_rad = GetCircumradius(ab.magnitude, bc.magnitude, ca.magnitude);
                if (p_rad < rad)
                {
                    index = p1.IndexOf(p);
                    rad = p_rad;
                }
            }
            point3 = new Point(p1[index].getPoint());

        }

        return new Face(point1, point2, point3, 0);
    }

    public Vector3 calculateCircumcenter(Point point1, Point point2, Point point3)
    {
        Vector3 bc = point3.getPoint() - point2.getPoint();
        Vector3 ca = point1.getPoint() - point3.getPoint();
        Vector3 ab = point2.getPoint() - point1.getPoint();
        float a = bc.sqrMagnitude;
        float b = ca.sqrMagnitude;
        float c = ab.sqrMagnitude;

        return (a * (b + c - a) * point1.getPoint() + b * (c + a - b) * point2.getPoint() + c * (a + b - c) * point3.getPoint()) / (a * (b + c - a) + b * (c + a - b) + c * (a + b - c));
    }

    public float GetCircumradius( float dist_ab,  float dist_bc,  float dist_ca)
    {
        return ((dist_ab* dist_bc * dist_ca) / Mathf.Sqrt(((dist_ab + dist_bc + dist_ca)*(dist_bc + dist_ca - dist_ab)*(dist_ca + dist_ab - dist_bc)*(dist_ab + dist_bc - dist_ca))));
    }

    public void MakeSimplex(Face f, List<Point> p)
    {

    }
}
