using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Incremental 
{
    public List<Point> points;
    public List<Tetrahedron> triangulation;

    private bool debug;

    public Incremental(List<Point> p, Vector3 obj_pos, Vector3 obj_scale, bool d)
    {
        points = p;
        debug = d;
        triangulation = new List<Tetrahedron>();
        BowyerWatson( obj_pos,  obj_scale);
    }
    public void BowyerWatson(Vector3 obj_pos, Vector3 obj_scale)
    {
        //1.Create a triangle that contains all the points to be processed.
        //2.Add a point to the triangulation.
        //3.Find all the triangles whose circumcircle contains the new point.
        //4.Remove these triangles.
        //5.Connect the vertices of the remaining hole to the new point.
        //6.Repeat 2 - 5 until all points are processed.
        //7.Remove all triangles that are connected to the original triangle.


        Vector3 s = (obj_scale/2) * 8 + obj_pos;
                     
        Tetrahedron s_t = new Tetrahedron(new Point(2 * obj_pos.x - s.x, -s.y / 2, -s.z), new Point(s.x, -s.y / 2, -s.z), new Point(obj_pos.x, -s.y / 2, s.z), new Point(obj_pos.x, s.y, obj_pos.z));
        triangulation.Add(s_t);

        foreach (var it in points)
        {
            var badTetrahedron = FindBadTetrahedron(it, triangulation); //if point is inside sphere, then that tetrahedron is bad
            var polygon = FindHoleBoundaries(badTetrahedron); // find the boundary of the polygonal hole

            triangulation.RemoveAll(o => badTetrahedron.Contains(o));

            foreach (var f in polygon)
            {
                Tetrahedron tetra = new Tetrahedron(it, f.Point1, f.Point2, f.Point3);
                triangulation.Add(tetra);           
            }
            polygon.Clear();
        }

        foreach (Tetrahedron tetra in triangulation.ToList())
        {

            foreach(Face f in tetra.faces)
            {
                if (CheckFace(f, s_t))
                {
                    triangulation.Remove(tetra);
                    break;
                }
            }

        }

        if (debug)
        {
            foreach (Tetrahedron tetra in triangulation)
            {
                foreach (Face f in tetra.faces)
                {
                    /************************DEBUG************************/
                    Debug.DrawLine(f.Point1.getPoint(), f.Point2.getPoint(), new Color(0, 0, 1), 1200f);
                    Debug.DrawLine(f.Point1.getPoint(), f.Point3.getPoint(), new Color(0, 0, 1), 1200f);
                    Debug.DrawLine(f.Point2.getPoint(), f.Point3.getPoint(), new Color(0, 0, 1), 1200f);
                    /************************DEBUG************************/
                }

            }
        }
    }

    public List<Tetrahedron> FindBadTetrahedron(Point p, List<Tetrahedron> t)
    {
        List<Tetrahedron> badT = new List<Tetrahedron>();

        foreach (var it in t)
        {
            if (it.IsPointInsideSphere(p))
            {
                it.isBad = true;
                badT.Add(it);
            }

        }

        return badT;
    }

    public List<Face> FindHoleBoundaries(List<Tetrahedron> badT)
    {
        var faces = new List<Face>();
        bool n = false;

        // for each tetrahedron in bad tetrahedron list
        foreach (Tetrahedron t in badT)
        {
            // for each face of the tetrahedron
            foreach (Face f in t.faces)
            {
                n = false;
                // check if the face is shared with another tetrahedron in bad tetrahedron list 
                foreach (Tetrahedron t2 in badT)
                {
                    //if it's the same tetrahedron
                    if (t.circumcenter.getPoint().Equals(t2.circumcenter.getPoint()))
                        break;
                    

                    foreach (Face f2 in t2.faces)
                    {
                        if (f.IsEqual(f2))
                        {
                            n = true;
                            break;
                        }
                    }
                }
                if (!n)
                    faces.Add(f);
            }
        }
        return faces;
    }

    public bool CheckFace(Face f, Tetrahedron s_t)
    {
        for(int i = 0; i < 4; i++)
        {
            if (f.Point1 == s_t.vertices[i] || f.Point2 == s_t.vertices[i] || f.Point3 == s_t.vertices[i] )
                return true;
        }

        return false;
    }

    

}
