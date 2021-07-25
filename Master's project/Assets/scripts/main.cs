using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;

public class main : MonoBehaviour
{
    // Start is called before the first frame update
    public int pointsAmount;
    public GameObject container;
    public bool debug;
    public bool DivideAndConquer;
    public bool IncrementalAlgorithm;

    private DivideAndConquer dewall;
    private Incremental incremental;

    private List<Point> points;
    private List<Vector4> color_points;

    private GameObject[] slices;
    private List<Plane> cutP;

    void Start()
    {
       
        getPoints();
        cutP = new List<Plane>();

        if (DivideAndConquer)
        {
            IncrementalAlgorithm = false;
            dewall = new DivideAndConquer(points, transform.position, transform.localScale, debug);
            List<Tetrahedron> t = dewall.triangulation;
        }
        if (IncrementalAlgorithm)
        {
            DivideAndConquer = false;
            incremental = new Incremental(points, transform.position, transform.localScale, debug);
        }
        //incremental.CalculateVCell();
        //slices = Slicer.Slice(incremental.cutP[0], gameObject);

        /* foreach (var i in incremental.cutP)
             slices = Slicer.Slice(i, gameObject);*/

        //Destroy(gameObject);

        //sweep = new Sweep(points, transform.position, transform.localScale, debug);


    }

    // Update is called once per frame
   

    public void getPoints()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
      

        points = new List<Point>();
        color_points = new List<Vector4>();
/*
        points.Add(new Point(0.5f, 0.95f, 0));
        points.Add(new Point(-0.1f, 0.8f, -0.1f));
        points.Add(new Point(0.3f, 0.6f, 0.1f));
        points.Add(new Point(-0.4f, 0.1f, 0.4f));
        points.Add(new Point(0, 0.2f, -0.4f));
*/
        for (int i = 0; i < pointsAmount; i++)
        {
            
            points.Add(new Point(Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2) + transform.position.x, Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2) + transform.position.y, Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2) + transform.position.z));

            if (debug)
            {
                color_points.Add(new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
                renderPoints(i);
            }

        }

    }

    public void renderPoints(int i)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<SphereCollider>().enabled = false;

        sphere.GetComponent<Renderer>().material.SetColor("_Color", color_points[i]);
        sphere.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        sphere.transform.position = points[i].getPoint();
        sphere.name = "Sphere" + i.ToString();
        sphere.transform.parent = container.transform;
    }

    public void CalculateVCell(List<Tetrahedron> triangulation)
    {

        foreach (Tetrahedron tetra in triangulation)
        {
            List<Tetrahedron> n = tetra.getNeighbors(triangulation);
            /************************DEBUG************************/
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<SphereCollider>().enabled = false;

            sphere.transform.position = tetra.circumcenter.getPoint();
            //sphere.transform.localScale = new Vector3(Mathf.Sqrt(tetra.circumradius_2), Mathf.Sqrt(tetra.circumradius_2), Mathf.Sqrt(tetra.circumradius_2));
            sphere.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
            /************************DEBUG************************/


            foreach (var i in n)
            {
                Plane p = new Plane();
                Vector3 dir = (i.circumcenter.getPoint() - tetra.circumcenter.getPoint()).normalized;

                p.SetNormalAndPosition(dir, tetra.circumcenter.getPoint());
                cutP.Add(p);
                Debug.DrawLine(tetra.circumcenter.getPoint(), i.circumcenter.getPoint(), new Color(1, 0, 0), 1200f);
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
                    Vector3 dir = (centre - tetra.circumcenter.getPoint()).normalized;
                    p.SetNormalAndPosition(dir, centre);
                    cutP.Add(p);

                    Debug.DrawRay(tetra.circumcenter.getPoint(), dir, new Color(0, 1, 0), 1200f);

                }
            }

        }


    }
}

