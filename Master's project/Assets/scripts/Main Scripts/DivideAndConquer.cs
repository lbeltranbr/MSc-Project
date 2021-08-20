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
    private int count_id = 0;
    public DivideAndConquer(List<Point> points, Vector3 pos, Vector3 scale, bool d)
    {
        a = new Plane();
        
        triangulation = new List<Tetrahedron>();

        obj_scale = scale;
        obj_pos = pos;

        debug = d;

        List<Face> AFL = new List<Face>();

        DeWall(points, AFL, 0);
    }
    public void DeWall(List<Point> points, List<Face> AFL, int axis)
    {
        List<Face> AFLa = new List<Face>();
        List<Face> AFL1 = new List<Face>();
        List<Face> AFL2 = new List<Face>();

        SetPlane(points,axis);
        PointPartition(points);

        if (AFL.Count == 0)
        {
            Tetrahedron t = MakeFirstSimplex(points);
            AFL.Add(t.faces[0]);
            AFL.Add(t.faces[1]);
            AFL.Add(t.faces[2]);
            AFL.Add(t.faces[3]);
            t.id = count_id;
            count_id++;
            triangulation.Add(t);

        }

        foreach (Face f in AFL)
        {
            if (a.GetSide(f.Point1.getPoint()) != a.GetSide(f.Point2.getPoint()) || a.GetSide(f.Point1.getPoint()) != a.GetSide(f.Point3.getPoint()))
            {
                Debug.Log("Face " + f.Point1.id.ToString() + f.Point2.id.ToString() + f.Point3.id.ToString() + " is intersected by a");
                AFLa.Add(f);
            }
            else
            {
                if (a.GetSide(f.vertices[0].getPoint()))
                {
                    AFL1.Add(f);
                    Debug.Log("Face " + f.Point1.id.ToString() + f.Point2.id.ToString() + f.Point3.id.ToString() + " is in AFL1");
                }
                else
                {
                    AFL2.Add(f);
                    Debug.Log("Face " + f.Point1.id.ToString() + f.Point2.id.ToString() + f.Point3.id.ToString() + " is in AFL2");

                }
            }
        }

        while (AFLa.Count != 0)
        {
            Face f = AFLa[0];
            AFLa.Remove(f);

            Tetrahedron t = MakeSimplex(f, points);
            /*if(t==null)
                Debug.Log("tetrahedron is null");*/
            
            if (t != null)
            {
                Debug.Log("tetrahedron is not null");

                if (isInTriangulation(t))
                {
                    Debug.LogError("Cyclic Tetrahedra Creation");
                    return;
                }
                t.id = count_id;
                triangulation.Add(t);
                if (debug)
                    t.DrawTetra();
                count_id++;
                foreach (Face f2 in t.faces)
                {
                    if (!f.IsEqual(f2))
                    {
                        if (a.GetSide(f2.Point1.getPoint())!= a.GetSide(f2.Point2.getPoint())|| a.GetSide(f2.Point1.getPoint()) != a.GetSide(f2.Point3.getPoint()))
                        {
                            Debug.Log("Face " + f2.Point1.id.ToString() + f2.Point2.id.ToString() + f2.Point3.id.ToString() + " is intersected by a");
                            Update(f2, AFLa);
                        }
                        else
                        {
                            if (a.GetSide(f2.vertices[0].getPoint()))
                            {
                                Debug.Log("Face " + f2.Point1.id.ToString() + f2.Point2.id.ToString() + f2.Point3.id.ToString() + " is in AFL1");
                                Update(f2, AFL1);
                            }
                            else
                            {
                                Debug.Log("Face " + f2.Point1.id.ToString() + f2.Point2.id.ToString() + f2.Point3.id.ToString() + " is in AFL2");
                                Update(f2, AFL2);
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("Calling DeWall again or exit");

        if (axis == 0)
        {
            if (AFL1.Count != 0)
                DeWall(p1, AFL1,1);
            if (AFL2.Count != 0)
                DeWall(p2, AFL2,1);
        }
        if (axis == 1)
        {
            if (AFL1.Count != 0)
                DeWall(p1, AFL1, 2);
            if (AFL2.Count != 0)
                DeWall(p2, AFL2, 2);
        }
        if (axis == 2)
        {
            if (AFL1.Count != 0)
                DeWall(p1, AFL1, 0);
            if (AFL2.Count != 0)
                DeWall(p2, AFL2, 0);
        }
    }

    public void SetPlane(List<Point> points, int axis)
    {
        List<Point> plist = points.ToList();
        plist.OrderBy(o => o.x);

        float max_x = plist[plist.Count/2].x;
        float min_x = plist[plist.Count/2 - 1].x;

        plist.OrderBy(o => o.y);

        float max_y = plist[plist.Count/2].y;
        float min_y = plist[plist.Count/2 - 1].y;

        plist.OrderBy(o => o.z);

        float max_z = plist[plist.Count/2].z;
        float min_z = plist[plist.Count/2 - 1].z;


        Vector3 p = new Vector3(((max_x + min_x) / 2f), ((max_y + min_y) / 2f), ((max_z + min_z) / 2f));
        plane_point = p;
        if (axis == 0)
            a.SetNormalAndPosition(new Vector3(1f, 0, 0), p);
        if (axis == 1)
            a.SetNormalAndPosition(new Vector3(0, 1f, 0), p);
        if (axis == 2)
            a.SetNormalAndPosition(new Vector3(0, 0, 1f), p);
        Debug.Log("Alpha: " + "m= " + p + " p= " + a.normal);

    }
    public void PointPartition(List<Point> points)
    {
        p1 = new List<Point>();
        p2 = new List<Point>();

        foreach (Point p in points)
        {
            if (a.GetSide(p.getPoint()))
            {
                Debug.Log("Point " + p.id.ToString() + " is in P1");
                p.inP1 = true;
                p1.Add(p); //positive side of the plane
            }
            else
            {
                Debug.Log("Point " + p.id.ToString() + " is in P2");

                p.inP1 = false;
                p2.Add(p); //negative side of the plane
            }
        }
    }

    public Tetrahedron MakeFirstSimplex(List<Point> points)
    {
        Point point1, point2, point3;
        float dist = 10000000000;
        int index = -1;
        float rad = 0;

        //*************POINT 1*************//
        // Selects the point nearest to the plane a in negative side 
        foreach (Point p in p2)
        {
            float p_dist = Mathf.Abs(a.GetDistanceToPoint(p.getPoint()));
            if (p_dist < dist)
            {
                index = p2.IndexOf(p);
                dist = p_dist;
            }
        }
        point1 = p2[index];
        Debug.Log("first point " + point1.id);

        //*************POINT 2*************//
        // Selects the point nearest to the first point on the other side of the plane
        index = -1;

        dist = 10000000000;
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
        Debug.Log("second point " + point2.id);


        index = 0;

        //*************POINT 3*************//
        // Selects the point with the min circumradius
     
        Vector3 ab = point2.getPoint() - point1.getPoint();
        Vector3 bc = points[0].getPoint() - point2.getPoint();
        Vector3 ca = point1.getPoint() - points[0].getPoint();

        rad = 10000000000;

        foreach (Point p in points)
        {
               
            if (!point1.Equals(p) && !point2.Equals(p))
            {
                bc = p.getPoint() - point2.getPoint();
                ca = point1.getPoint() - p.getPoint();

                float p_rad = GetCircumradius(ab.magnitude, bc.magnitude, ca.magnitude);
                if (p_rad < rad)
                {
                    index = points.IndexOf(p);
                    rad = p_rad;
                }
            }
              
        }
        point3 = points[index];
        Debug.Log("third point " + point3.id);


        //*************POINT 4*************//

        Plane pl = new Plane(point1.getPoint(), point2.getPoint(), point3.getPoint());
        Debug.Log("Plane normal: " + pl.normal.ToString());

        Line l = Maths.CalcLine(point1, point2, point3);
        Debug.Log("line for 4th point: " + l.start.getPoint().ToString() + "+ t " + l.direction.getPoint().ToString());
        //float enter = 0.0f;

        rad = 1000000000;
        index = -1;

        foreach (Point point in points)
        {
            if (!point.Equals(point1) && !point.Equals(point2) && !point.Equals(point3))
            {
                Plane halfplane = Maths.CalcMiddlePlane(point, point1);
                Debug.Log("halfplane for points " + point1.id.ToString() + "-" + point.id.ToString() + " m= " + Maths.GetPointofMiddlePlane(point, point1).ToString() + " p= " + halfplane.normal.ToString());

                Point c = Maths.LinePlaneIntersection(l, Maths.GetPointofMiddlePlane(point, point1), halfplane.normal);
                if (c == null)
                    Debug.Log("intersection between halfplane and line does not exist");
                if (c!=null)
                {
                    Debug.Log("center point is " + c.getPoint().ToString());

                    float p_rad = Vector3.Distance(c.getPoint(), point.getPoint());

                    if (p_rad < rad)
                    {
                        index = points.IndexOf(point);
                        rad = p_rad;
                    }
                }

            }
        }
        Debug.Log("4th point " + points[index].id);
        Debug.Log(" face: " + point1.id.ToString() + point2.id.ToString() + point3.id.ToString());

        if (!pl.GetSide(points[index].getPoint()))
        {
            Debug.Log("4th point is in left side");

            Vector3 i = point1.getPoint();
            int id = point1.id;
            point1.SetP(point2.getPoint());
            point1.id = point2.id;
            point2.SetP(i);
            point2.id = id;
            Debug.Log("invert face: " + point1.id.ToString() + point2.id.ToString() + point3.id.ToString());

        }

        if (index == -1)
            return null;
        else
        {
            Debug.Log("tetrahedron: " + point2.id.ToString() + point1.id.ToString() + point3.id.ToString() + points[index].id.ToString());
            return new Tetrahedron(point2, point1, point3, points[index]);

        }

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
        Debug.Log("Make simplex for face: " + f.Point1.id.ToString() + f.Point2.id.ToString() + f.Point3.id.ToString());
        Plane pl = new Plane(f.Point1.getPoint(), f.Point2.getPoint(), f.Point3.getPoint());
        Debug.Log("Plane normal: " + pl.normal.ToString());

        Line l = Maths.CalcLine(f.Point1, f.Point2, f.Point3);
        Debug.Log("line for face " + f.Point1.id.ToString() + f.Point2.id.ToString() + f.Point3.id.ToString()+" : " + l.start.getPoint().ToString() + "+ t " + l.direction.getPoint().ToString());

        //float enter = 0.0f;

        float rad = 1000000000;
        int index = -1;

        foreach (Point point in p)
        {
            if (!point.Equals(f.Point1)&& !point.Equals(f.Point2) && !point.Equals(f.Point3) && !pl.GetSide(point.getPoint()))
            {
                Debug.Log("point "+ point.id.ToString() + " is in the right side of the 3 points plane ");

                Plane halfplane = Maths.CalcMiddlePlane(point, f.Point1);

                Debug.Log("halfplane for points " + f.Point1.id.ToString() + "-" + point.id.ToString() + " m= " + Maths.GetPointofMiddlePlane(point, f.Point1).ToString() + " p= " + halfplane.normal.ToString());

                Point c = Maths.LinePlaneIntersection(l, Maths.GetPointofMiddlePlane(point, f.Point1), halfplane.normal);
                if (c == null)
                    Debug.Log("intersection between halfplane and line does not exist");
                if (c!=null)
                {
                    Debug.Log("center point is " + c.getPoint().ToString());

                    float p_rad = Vector3.Distance(c.getPoint(), point.getPoint());

                    if (!pl.GetSide(c.getPoint()))
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
        {
            Debug.Log("4rth point is NULL");
            return null;
        }
        else
        {
            Debug.Log("tetrahedron: " + f.Point1.id.ToString() + f.Point2.id.ToString() + f.Point3.id.ToString() + p[index].id.ToString());
            return new Tetrahedron(f.Point1, f.Point2, f.Point3, p[index]);
        }
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

    public bool IntersectPositive(Plane p, Face f)
    {
        if (p.GetSide(f.Point1.getPoint()))
            return true;
        else
            return false;
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

            if (count == 4)
                return true;
        }

        return false;
    }

}
