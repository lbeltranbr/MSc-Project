using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public List<Tetrahedron> triangulation;
    Vector3 obj_scale, obj_pos, plane_point;

    private bool debug;

    public DivideAndConquer(List<Point> points, Vector3 pos, Vector3 scale, bool d)
    {
        a = new Plane();
        
        triangulation = new List<Tetrahedron>();

        obj_scale = scale;
        obj_pos = pos;

        debug = d;

        List<Face> AFL = new List<Face>();

        DeWall(points, AFL);
    }
    public void DeWall(List<Point> points, List<Face> AFL)
    {
        List<Face> AFLa = new List<Face>();
        List<Face> AFL1 = new List<Face>();
        List<Face> AFL2 = new List<Face>();

        SetPlane(points);
        PointPartition(points);

        if (AFL.Count == 0)
        {
            Face f = MakeFirstSimplex(points);
            AFL.Add(f);
        }


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

            // if (LinePlaneIntersection(f.Point1.getPoint(), ab, plane_point, a.normal) || LinePlaneIntersection(f.Point2.getPoint(), bc, plane_point, a.normal) || LinePlaneIntersection(f.Point3.getPoint(), ca, plane_point, a.normal))
            //  AFLa.Add(f);

            if (f.vertices[0].inP1 && f.vertices[1].inP1 && f.vertices[2].inP1)
                AFL1.Add(f);
            if (!f.vertices[0].inP1 && !f.vertices[1].inP1 && !f.vertices[2].inP1)
                AFL2.Add(f);
        }

        while (AFLa.Count != 0)
        {
           
            Face f = AFLa[0];
            AFLa.Remove(f);

            Tetrahedron t = MakeSimplex(f, points);
            if (t != null && !isInTriangulation(t))
            {
                triangulation.Add(t);
                
                foreach (Face f2 in t.faces)
                {
                    if (!f.IsEqual(f2))
                    {
                        Vector3 ab = f2.Point2.getPoint() - f2.Point1.getPoint();
                        Vector3 bc = f2.Point3.getPoint() - f2.Point2.getPoint();
                        Vector3 ca = f2.Point1.getPoint() - f2.Point3.getPoint();
                         Ray r1 = new Ray(f.Point1.getPoint(), ab);
                         Ray r2 = new Ray(f.Point2.getPoint(), bc);
                         Ray r3 = new Ray(f.Point3.getPoint(), ca);

                        if (a.Raycast(r1, out enter) || a.Raycast(r2, out enter) || a.Raycast(r3, out enter))
                            Update(f2, AFLa);

                        if(f2.vertices[0].inP1 && f2.vertices[1].inP1 && f2.vertices[2].inP1)
                            Update(f2, AFL1);
                        if (!f2.vertices[0].inP1 && !f2.vertices[1].inP1 && !f2.vertices[2].inP1)
                            Update(f2, AFL2);

                    }
                
                }
            }
        }
        if (AFL1.Count != 0)
        {
        
            DeWall(p1, AFL1);

        }
        if (AFL2.Count != 0)
        {
            
            DeWall(p2, AFL2);

        }


    }

    public void SetPlane(List<Point> points)
    {

        float max_x = points.Max(o => o.x);
        float min_x = points.Min(o => o.x);

        float max_z = points.Max(o => o.z);
        float min_z = points.Min(o => o.z);

        Vector3 p = new Vector3(((max_x + min_x) / 2f) + obj_pos.x, obj_pos.y, ((max_z + min_z) / 2f) + obj_pos.z);
        plane_point = p;

        if (obj_scale.x >= obj_scale.z)
            a.SetNormalAndPosition(new Vector3(1f, 0, 0), p);
        else
            a.SetNormalAndPosition(new Vector3(0, 0, 1f), p);
    }
    public void PointPartition(List<Point> points)
    {
        p1 = new List<Point>();
        p2 = new List<Point>();

        foreach (Point p in points)
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
        float dist = Mathf.Abs(a.GetDistanceToPoint(points[0].getPoint()));
        int index = 0;
        float rad = 0;
        // Selects the point nearest to the plane a
        foreach(Point p in points)
        {
            float p_dist = Mathf.Abs(a.GetDistanceToPoint(p.getPoint()));
            if (p_dist < dist)
            {
                index = points.IndexOf(p);
                dist = p_dist;
            }
        }
        point1 = points[index];

        // Selects the point nearest to the first point on the other side of the plane
        index = 0;

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
            point2 = p2[index];

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
            point2 = p1[index];

        }
        index = 0;

        // Selects the point with the min circumradius
        if (point1.inP1)
        {
            Vector3 ab = point2.getPoint() - point1.getPoint();
            Vector3 bc = p2[0].getPoint() - point2.getPoint();
            Vector3 ca = point1.getPoint() - p2[0].getPoint();

            rad = 1000000;

            foreach (Point p in p2)
            {
               
                if (!point2.Equals(p))
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
              
            }
            point3 = p2[index];

        }
        else
        {
            Vector3 ab = point2.getPoint() - point1.getPoint();
            Vector3 bc = p1[0].getPoint() - point2.getPoint();
            Vector3 ca = point1.getPoint() - p1[0].getPoint();

            rad = 100000000;

            foreach (Point p in p1)
            {
                if (!point2.Equals(p))
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
            }
            point3 = p1[index];
        }

        /************************DEBUG************************/
        Debug.DrawLine(point1.getPoint(), point2.getPoint(), new Color(0, 0, 1), 1200f);
        Debug.DrawLine(point1.getPoint(), point3.getPoint(), new Color(0, 0, 1), 1200f);
        Debug.DrawLine(point2.getPoint(), point3.getPoint(), new Color(0, 0, 1), 1200f);

        /************************DEBUG************************/
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
        float a = dist_ab * dist_bc * dist_ca;
        float b = Mathf.Sqrt(((dist_ab + dist_bc + dist_ca) * (dist_bc + dist_ca - dist_ab) * (dist_ca + dist_ab - dist_bc) * (dist_ab + dist_bc - dist_ca)));

        return ((dist_ab* dist_bc * dist_ca) / Mathf.Sqrt(((dist_ab + dist_bc + dist_ca)*(dist_bc + dist_ca - dist_ab)*(dist_ca + dist_ab - dist_bc)*(dist_ab + dist_bc - dist_ca))));
    }

    public Tetrahedron MakeSimplex(Face f, List<Point> p)
    {
        Plane pl = new Plane(f.Point1.getPoint(), f.Point2.getPoint(), f.Point3.getPoint());

        Line l = Maths.CalcLine(f.Point1, f.Point2, f.Point3);
        float enter = 0.0f;

        float rad = 1000000000;
        int index = -1;

        foreach (Point point in p)
        {
            if (!point.Equals(f.Point1)&& !point.Equals(f.Point2) && !point.Equals(f.Point3))
            {
                Plane halfplane = Maths.CalcMiddlePlane(point, f.Point1);
                Ray r = new Ray(l.start.getPoint(), l.direction.getPoint());
                if (halfplane.Raycast(r, out enter))
                {
                    Vector3 c = r.GetPoint(enter);
                    float p_rad = Vector3.Distance(c, point.getPoint());

                    //Tetrahedron t = new Tetrahedron(f.Point1, f.Point2, f.Point3, point);
                    //float p_rad = t.circumradius_2;

                    if (!halfplane.GetSide(point.getPoint()))
                        p_rad = -p_rad;

                    if (p_rad < rad)
                    {
                        index = p.IndexOf(point);
                        rad = p_rad;
                    }
                }

            }

               
        }

        if (index == -1)
            return null;
        else
            return new Tetrahedron(f.Point1, f.Point2, f.Point3, p[index]);
    }

    public void Update(Face f, List<Face> f_list)
    {
        bool inList = false;

        foreach(Face face in f_list.ToList())
        {
            if (face.IsEqual(f))
            {
                inList = true;
                f_list.Remove(face);
                break;
            }
        }

        if (!inList)
            f_list.Add(f);
            
    }

    public bool isInTriangulation(Tetrahedron t)
    {
        int count = 0;

        foreach(Tetrahedron tet in triangulation)
        {
            count = 0;

            foreach (Face f in t.faces)
            {
                foreach (Face f2 in tet.faces)
                    if (f.IsEqual(f2))
                        count++;
            }
        }

        if (count == 4)
            return true;

        return false;
    }

}
