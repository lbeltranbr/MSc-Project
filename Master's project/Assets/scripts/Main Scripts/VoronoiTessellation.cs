using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiTessellation 
{
    public static List<Plane> CalculateVCell(List<Tetrahedron> triangulation)
    {
        List<Plane> cutP = new List<Plane>();

        /*foreach (Tetrahedron tetra in triangulation)
        {
            verts.Add(tetra.circumcenter.getPoint());

            Debug.Log(tetra.vertices[0].id.ToString() + tetra.vertices[1].id.ToString() + tetra.vertices[2].id.ToString() + tetra.vertices[3].id.ToString());
            Debug.Log(tetra.id);
            Debug.Log(tetra.circumcenter.getPoint());

            List<Tetrahedron> n = tetra.getNeighbors(triangulation);


            foreach (var i in n)
            {
                //Debug.Log(i.vertices[0].id.ToString() + i.vertices[1].id.ToString() + i.vertices[2].id.ToString() + i.vertices[3].id.ToString());

                Plane p = new Plane();
                Vector3 dir = (i.circumcenter.getPoint() - tetra.circumcenter.getPoint()).normalized;
                float z = -(dir.x + dir.y) / dir.z;
                Vector3 p_normal = new Vector3(1, 1, z);
                p.SetNormalAndPosition(dir, i.circumcenter.getPoint());
                cutP.Add(p);
                /*if(debug)
                    Debug.DrawLine(tetra.circumcenter.getPoint(), i.circumcenter.getPoint(), new Color(0, 1, 0), 1200f);
            }
            if (n.Count < 4)
            {
                for (int i = 0; i < tetra.nosharedfaces.Count; i++)
                {
                    int index = tetra.nosharedfaces[i];
                    float x = (tetra.faces[index].Point1.x + tetra.faces[index].Point2.x + tetra.faces[index].Point3.x) / 3;
                    float y = (tetra.faces[index].Point1.y + tetra.faces[index].Point2.y + tetra.faces[index].Point3.y) / 3;
                    float z = (tetra.faces[index].Point1.z + tetra.faces[index].Point2.z + tetra.faces[index].Point3.z) / 3;

                    Plane p = new Plane();
                    Vector3 centre = new Vector3(x, y, z);

                    Debug.Log("c" + centre);

                    Vector3 dir = (centre - tetra.circumcenter.getPoint()).normalized;
                    float z2 = -(dir.x + dir.y) / dir.z;
                    Debug.Log("z" + z2);

                    p.SetNormalAndPosition(dir, centre);
                    cutP.Add(p);
                   

                }
            }

        }*/

        return cutP;
    }
}
