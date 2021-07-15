using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron
{
    public Point[] vertices = new Point[4];
    public Point circumcenter;
    public float circumradius_2;
    public List<Face> faces = new List<Face>();
    public bool isBad = false;

    public Tetrahedron(Point p1, Point p2, Point p3, Point p4)
    {
        vertices[0] = p1;
        vertices[1] = p2;
        vertices[2] = p3;
        vertices[3] = p4;

        CalculateCircumcenter();
        CalculateCircumradius();

        vertices[0].adjacentTetrahedrons.Add(this);
        vertices[1].adjacentTetrahedrons.Add(this);
        vertices[2].adjacentTetrahedrons.Add(this);
        vertices[3].adjacentTetrahedrons.Add(this);

        faces.Add(new Face(p1, p2, p3, this));
        faces.Add(new Face(p2, p3, p4, this));
        faces.Add(new Face(p3, p4, p1, this));
        faces.Add(new Face(p4, p1, p2, this));

        /************************DEBUG************************/
        Debug.DrawLine(p1.getPoint(), p2.getPoint(), new Color(0, 0, 0),60f);
        Debug.DrawLine(p1.getPoint(), p3.getPoint(), new Color(0, 0, 0),60f);
        Debug.DrawLine(p2.getPoint(), p3.getPoint(), new Color(0, 0, 0),60f);
        Debug.DrawLine(p2.getPoint(), p4.getPoint(), new Color(0, 0, 0),60f);
        Debug.DrawLine(p3.getPoint(), p4.getPoint(), new Color(0, 0, 0),60f);
        Debug.DrawLine(p4.getPoint(), p1.getPoint(), new Color(0, 0, 0),60f);
        /************************DEBUG************************/

    }
    public void CalculateCircumcenter()
    {
        Matrix4x4 mat_a = new Matrix4x4(new Vector4(vertices[0].x, vertices[1].x, vertices[2].x, vertices[3].x), new Vector4(vertices[0].y, vertices[1].y, vertices[2].y, vertices[3].y), new Vector4(vertices[0].z, vertices[1].z, vertices[2].z, vertices[3].z), new Vector4(1, 1, 1, 1));
        float a = mat_a.determinant;

        float c1 = vertices[0].x * vertices[0].x + vertices[0].y * vertices[0].y + vertices[0].z * vertices[0].z;
        float c2 = vertices[1].x * vertices[1].x + vertices[1].y * vertices[1].y + vertices[1].z * vertices[1].z;
        float c3 = vertices[2].x * vertices[2].x + vertices[2].y * vertices[2].y + vertices[2].z * vertices[2].z;
        float c4 = vertices[3].x * vertices[3].x + vertices[3].y * vertices[3].y + vertices[3].z * vertices[3].z;

        Matrix4x4 mat_dx = new Matrix4x4(new Vector4(c1, c2, c3, c4), new Vector4(vertices[0].y, vertices[1].y, vertices[2].y, vertices[3].y), new Vector4(vertices[0].z, vertices[1].z, vertices[2].z, vertices[3].z), new Vector4(1, 1, 1, 1));
        float dx = mat_dx.determinant;
        Matrix4x4 mat_dy = new Matrix4x4(new Vector4(c1, c2, c3, c4), new Vector4(vertices[0].x, vertices[1].x, vertices[2].x, vertices[3].x), new Vector4(vertices[0].z, vertices[1].z, vertices[2].z, vertices[3].z), new Vector4(1, 1, 1, 1));
        float dy = mat_dy.determinant;
        Matrix4x4 mat_dz = new Matrix4x4(new Vector4(c1, c2, c3, c4), new Vector4(vertices[0].x, vertices[1].x, vertices[2].x, vertices[3].x), new Vector4(vertices[0].y, vertices[1].y, vertices[2].y, vertices[3].y), new Vector4(1, 1, 1, 1));
        float dz = mat_dz.determinant;

        circumcenter = new Point(dx / (2 * a), dy / (2 * a), dz / (2 * a));
      
    }
    public void CalculateCircumradius()
    {
        circumradius_2 = (vertices[0].x - circumcenter.x) * (vertices[0].x - circumcenter.x) + (vertices[0].y - circumcenter.y) * (vertices[0].y - circumcenter.y) + (vertices[0].z - circumcenter.z) * (vertices[0].z - circumcenter.z);
    }

    public bool IsPointInsideSphere(Point p)
    {
        float d_2 = (p.x - circumcenter.x) * (p.x - circumcenter.x) + (p.y - circumcenter.y) * (p.y - circumcenter.y) + (p.z - circumcenter.z) * (p.z - circumcenter.z);            
        return d_2 < circumradius_2;
    }

    //Find neighbours of the tetrahedon
    public List<Tetrahedron> getNeighbors()
    {
        List<Tetrahedron> neighbors = new List<Tetrahedron>();

        foreach (Face f in faces)
        {
            if (f.left == this && f.right != null)
                neighbors.Add(f.right);
            else if (f.right == this && f.left != null)
                neighbors.Add(f.left);
        }
        return neighbors;
    }

}
