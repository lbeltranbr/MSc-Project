using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Incremental 
{
    public List<Point> points;
    public List<Tetrahedron> triangulation;

    public Incremental(List<Point> p, Vector3 obj_pos, Vector3 obj_scale)
    {
        points = p;
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
        /*GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph.transform.Translate(new Vector3(2 * obj_pos.x - s.x, -s.y / 2, -s.z));
        sph.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        GameObject sph2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph2.transform.Translate(new Vector3(s.x, -s.y/2, -s.z));
        sph2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        GameObject sph3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph3.transform.Translate(new Vector3(obj_pos.x, -s.y/2, s.z));
        sph3.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        GameObject sph4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph4.transform.Translate(new Vector3(obj_pos.x, s.y, obj_pos.z));
        sph4.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);*/
              
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

                /************************DEBUG************************
                Debug.DrawLine(it.getPoint(), f.Point1.getPoint(), new Color(1, 0, 0), 60f);
                Debug.DrawLine(it.getPoint(), f.Point2.getPoint(), new Color(1, 0, 0), 60f);
                Debug.DrawLine(f.Point1.getPoint(), f.Point2.getPoint(), new Color(1, 0, 0), 60f);
                Debug.DrawLine(f.Point1.getPoint(), f.Point3.getPoint(), new Color(1, 0, 0), 60f);
                Debug.DrawLine(f.Point2.getPoint(), f.Point3.getPoint(), new Color(1, 0, 0), 60f);
                Debug.DrawLine(f.Point3.getPoint(), it.getPoint(), new Color(1, 0, 0), 60f);
                /************************DEBUG************************/
            }
        }
    
        foreach (Tetrahedron tetra in triangulation.ToList())
        {
            foreach(Face f in tetra.faces)
            {
                if(CheckFace(f,s_t))
                   triangulation.Remove(tetra);
            }

        }


        foreach (Tetrahedron tetra in triangulation)
        {
            foreach(Face f in tetra.faces)
            {
                /************************DEBUG************************/
                Debug.DrawLine(f.Point1.getPoint(), f.Point2.getPoint(), new Color(0, 0, 1), 60f);
                Debug.DrawLine(f.Point1.getPoint(), f.Point3.getPoint(), new Color(0, 0, 1), 60f);
                Debug.DrawLine(f.Point2.getPoint(), f.Point3.getPoint(), new Color(0, 0, 1), 60f);
                /************************DEBUG************************/
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

        foreach (Tetrahedron t in badT)
        {
            foreach(Face f in t.faces)
            {
                if(f.right == null || !f.left.isBad  || !f.right.isBad)
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
