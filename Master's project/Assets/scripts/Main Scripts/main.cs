using System.Collections.Generic;
using UnityEngine;


public class main : MonoBehaviour
{
    // Start is called before the first frame update
    public int pointsAmount;
    public GameObject container;
    public bool debug;
    public bool random;

    public bool cut;

    [HideInInspector]
    public int index;

    private DivideAndConquer dewall;
    private Incremental incremental;

    private List<Point> points;
    private List<Vector4> color_points;

    private GameObject[] s;


    void Start()
    {

        ExecuteCode();

    }

    public void ExecuteCode()
    {
        clearContainer();

        getPoints();
        List<Tetrahedron> t = new List<Tetrahedron>();
       

        switch (index)
        {
            case 0:

                incremental = new Incremental(points, gameObject.GetComponent<BoxCollider>().center + transform.position, gameObject.GetComponent<BoxCollider>().size, debug);
                t = incremental.triangulation;
                break;
            case 1:
                dewall = new DivideAndConquer(points, gameObject.GetComponent<BoxCollider>().center + transform.position, gameObject.GetComponent<BoxCollider>().size, debug);
                t = dewall.triangulation;
                break;
        }


        if (cut)
        {
            Destroy(gameObject);
        }
    }
    public void getPoints()
    {
       
        points = new List<Point>();
        color_points = new List<Vector4>();

        if (!random)
        {
            points.Add(new Point(0.5f, 0.95f, 0, 0));
            points.Add(new Point(-0.1f, 0.8f, -0.1f, 1));
            points.Add(new Point(0.3f, 0.6f, 0.1f, 2));
            points.Add(new Point(-0.4f, 0.1f, 0.4f, 3));
            points.Add(new Point(0, 0.2f, -0.4f, 4)); /*
            points.Add(new Point(-0.47f, 0.9f, -0.3f, 0));
            points.Add(new Point(-0.19f, 0.41f, 0.21f, 1));
            points.Add(new Point(0.02f, 0.83f, 0.3f, 2));
            points.Add(new Point(0.04f, 0.01f, -0.43f, 3));
            points.Add(new Point(-0.01f, 0.01f, -0.5f, 4));*/
            pointsAmount = 5;
        }


        for (int i = 0; i < pointsAmount; i++)
        {
            if (random)
                points.Add(new Point(Random.Range(-gameObject.GetComponent<BoxCollider>().size.x / 2, gameObject.GetComponent<BoxCollider>().size.x / 2) + gameObject.GetComponent<BoxCollider>().center.x + transform.position.x,
                    Random.Range(-gameObject.GetComponent<BoxCollider>().size.y / 2, gameObject.GetComponent<BoxCollider>().size.y / 2) + gameObject.GetComponent<BoxCollider>().center.y + transform.position.y,
                    Random.Range(-gameObject.GetComponent<BoxCollider>().size.z / 2, gameObject.GetComponent<BoxCollider>().size.z / 2) + gameObject.GetComponent<BoxCollider>().center.z + transform.position.z, i));

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

    public void clearContainer()
    {
        while (container.transform.childCount > 0)
        {
            DestroyImmediate(container.transform.GetChild(0).gameObject);
        }


    }
}

