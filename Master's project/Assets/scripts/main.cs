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

    private Sweep sweep;
    private Incremental incremental;

    private List<Point> points;
    private List<Vector4> color_points;

    private GameObject[] slices;

    void Start()
    {
       
        getPoints();

        //incremental = new Incremental(points, transform.position, transform.localScale,debug);
        //incremental.CalculateVCell();
        //slices = Slicer.Slice(incremental.cutP[0], gameObject);

        /* foreach (var i in incremental.cutP)
             slices = Slicer.Slice(i, gameObject);*/

        //Destroy(gameObject);

        //sweep = new Sweep(points, transform.position, transform.localScale, debug);
        delaunay d = new delaunay();
        d.getEdges(points, 1, 1, 1);
       
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

        points.Add(new Point(0, 0.95f, 0));
        points.Add(new Point(-0.1f, 0.8f, -0.1f));
        points.Add(new Point(0.3f, 0.6f, 0.1f));
        points.Add(new Point(-0.4f, 0.1f, 0.4f));
        points.Add(new Point(0, 0.2f, -0.4f));

        for (int i = 0; i < pointsAmount; i++)
        {
            
            //points.Add(new Point(Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2) + transform.position.x, Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2) + transform.position.y, Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2) + transform.position.z));

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

    
}

