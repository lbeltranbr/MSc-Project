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
        BowyerWatson( obj_pos,  obj_scale);
    }
    public void BowyerWatson(Vector3 obj_pos, Vector3 obj_scale)
    {


        //Range(-transform.localScale.x / 2, transform.localScale.x / 2) + transform.position.x

        Vector3 s = (obj_scale/2) * 4 + obj_pos;
        /*GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph.transform.Translate(new Vector3(s.x, -s.y, s.z));
        sph.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        GameObject sph2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph2.transform.Translate(new Vector3(s.x, -s.y, -s.z));
        sph2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        GameObject sph3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph3.transform.Translate(new Vector3(-s.x*2, -s.y, obj_pos.z));
        sph3.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);*/


        //1. Create tetrahedra that contains all points

        Tetrahedron t = new Tetrahedron(new Point(obj_pos.x/2,s.y,obj_pos.z/2), new Point(s.x, -s.y, s.z), new Point(s.x, -s.y, -s.z), new Point(-s.x * 2, -s.y, obj_pos.z));
        triangulation.Add(t);

        foreach (var it in points)
        {
            var badTriangles = FindBadTriangles(it, triangulation);
            var polygon = FindHoleBoundaries(badTriangles);

            foreach (var triangle in badTriangles)
            {
                foreach (var vertex in triangle.vertices)
                {
                    vertex.adjacentTriangles.Remove(triangle);
                }
            }

            triangulation.RemoveAll(o => badTriangles.Contains(o));

            foreach (var edge in polygon.Where(possibleEdge => possibleEdge.Point1 != it && possibleEdge.Point2 != it))
            {
                var tetra = new Tetrahedron(it, edge.Point1, edge.Point2, edge.Point3);
                triangulation.Add(tetra);
            }
        }


    }

    public List<Tetrahedron> FindBadTriangles(Point p, List<Tetrahedron> t)
    {
        List<Tetrahedron> badt = new List<Tetrahedron>();

        foreach (var it in t)
        {
            if (it.IsPointInsideSphere(p))
                badt.Add(it);
        }

        return badt;
    }

    public List<Edge_incremental> FindHoleBoundaries(List<Tetrahedron> badTriangles)
    {
        var edges = new List<Edge_incremental>();

        foreach (var triangle in badTriangles)
        {
            edges.Add(new Edge_incremental(triangle.vertices[0], triangle.vertices[1], triangle.vertices[2]));
            edges.Add(new Edge_incremental(triangle.vertices[0], triangle.vertices[2], triangle.vertices[3]));
            edges.Add(new Edge_incremental(triangle.vertices[0], triangle.vertices[3], triangle.vertices[1]));
            edges.Add(new Edge_incremental(triangle.vertices[1], triangle.vertices[3], triangle.vertices[2]));
           
        }
        var grouped = edges.GroupBy(o => o);
        var boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
        return boundaryEdges.ToList();
    }



}
