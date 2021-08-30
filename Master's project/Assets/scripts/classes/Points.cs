using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Point
{
    public float x, y, z;
    public int id;
    public List<Tetrahedron> tetrahedrons;
    public List<VoronoiTessellation> faces;
    public List<Point> neighbours;
    public Point(float xpos, float ypos, float zpos)
    {
        x = xpos;
        y = ypos;
        z = zpos;
        tetrahedrons = new List<Tetrahedron>();
    }
    public Point(float xpos, float ypos, float zpos, int i)
    {
        x = xpos;
        y = ypos;
        z = zpos;
        id = i;
        tetrahedrons = new List<Tetrahedron>();

    }
    public Point(Vector3 p)
    {
        x = p.x;
        y = p.y;
        z = p.z;
        tetrahedrons = new List<Tetrahedron>();

    }
    public Point(Vector3 p, int i)
    {
        x = p.x;
        y = p.y;
        z = p.z;
        id = i;
        tetrahedrons = new List<Tetrahedron>();

    }

    public Vector3 getPoint()
    {
        return new Vector3(x, y, z);
    }
    public void SetP(Vector3 p)
    {
        x = p.x;
        y = p.y;
        z = p.z;
    }
    public void AddTetrahedron(Tetrahedron t)
    {
        tetrahedrons.Add(t);
    }
    public void RemoveTetrahedron(Tetrahedron t)
    {
        for (int i = 0; i < tetrahedrons.Count; i++)
        {
            if (tetrahedrons[i].Equals(t))
            {
                tetrahedrons.RemoveAt(i);
                break;
            }
        }
    }

    public void CalculateVoronoiCell()
    {
        neighbours = new List<Point>();
        faces = new List<VoronoiTessellation>();

        //Find neighburing vertices
        foreach (Tetrahedron t in tetrahedrons)
        {
            neighbours.Add(t.vertices[0]);
            neighbours.Add(t.vertices[1]);
            neighbours.Add(t.vertices[2]);
            neighbours.Add(t.vertices[3]);
            
        }
        neighbours = neighbours.Distinct().ToList();
        neighbours.Remove(this);


        foreach (Point n in neighbours)
        {
            //Calculate bisecting plane between the two vertices
            Plane halfplane = Maths.CalcMiddlePlane(n, this);
           

            List<Point> VoronoiVertices = new List<Point>();

            //Find all the vertices that are on the plane
            foreach (Tetrahedron t in tetrahedrons)
            {
                if (Mathf.Abs(halfplane.GetDistanceToPoint(t.circumcenter.getPoint())) < 0.01)
                {
                    VoronoiVertices.Add(t.circumcenter);
                }
            }

            faces.Add(new VoronoiTessellation(VoronoiVertices, halfplane));
        }

    }

}
