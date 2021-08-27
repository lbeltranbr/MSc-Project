using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Incremental
{
    public List<Point> points;
    public List<Tetrahedron> triangulation;
    public float time;

    private bool debug;
    private string path;

    public Incremental(List<Point> p, Vector3 obj_pos, Vector3 obj_scale, bool d)
    {
        points = p;
        debug = d;
        triangulation = new List<Tetrahedron>();
        BowyerWatson(obj_pos, obj_scale);
    }
    public Incremental(List<Point> p, Vector3 obj_pos, Vector3 obj_scale, bool d, string pth)
    {
        points = p;
        debug = d;
        triangulation = new List<Tetrahedron>();
        BowyerWatson(obj_pos, obj_scale);
        path = pth;
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


        Vector3 s = (obj_scale / 2) * 8 + obj_pos;

        Tetrahedron s_t = new Tetrahedron(new Point(2 * obj_pos.x - s.x, -s.y / 2, -s.z), new Point(s.x, -s.y / 2, -s.z), new Point(obj_pos.x, -s.y / 2, s.z), new Point(obj_pos.x, s.y, obj_pos.z));
        triangulation.Add(s_t);
        //s_t.DrawTetra();
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

            tetra.id = triangulation.FindIndex(0, o => o.Equals(tetra));
            foreach (Face f in tetra.faces)
            {
                if (CheckFace(f, s_t))
                {
                    triangulation.Remove(tetra);
                    tetra.removeFromVertices();
                    break;
                }
            }

        }


        if (debug)
        {
            foreach (Tetrahedron tetra in triangulation)
            {
                tetra.DrawTetra();
                Debug.Log("tetrahedron: " + tetra.vertices[0].id.ToString() + tetra.vertices[1].id.ToString() + tetra.vertices[2].id.ToString() + tetra.vertices[3].id.ToString());

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
                it.removeFromVertices();

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
                    //if(n) break;
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
        for (int i = 0; i < 4; i++)
        {
            if (f.Point1 == s_t.vertices[i] || f.Point2 == s_t.vertices[i] || f.Point3 == s_t.vertices[i])
                return true;
        }

        return false;
    }



}
