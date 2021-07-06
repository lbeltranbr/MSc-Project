using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour
{
    // Start is called before the first frame update
    public int pointsAmount;
    public GameObject container;

    private delaunay d;

    private List<Point> points;
    private List<Vector4> color_points;

    void Start()
    {
        getPoints();
        d = new delaunay();
        d.getEdges(points, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getPoints()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
      

        points = new List<Point>();
        color_points = new List<Vector4>();

        for (int i = 0; i < pointsAmount; i++)
        {
            
            points.Add(new Point(Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2) + transform.position.x, Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2) + transform.position.y, Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2) + transform.position.z));
            color_points.Add(new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
            renderPoints(i);
           

        }

    }

    public void renderPoints(int i)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Renderer>().material.SetColor("_Color", color_points[i]);
        sphere.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        sphere.transform.position = points[i].getPoint();
        sphere.name = "Sphere" + i.ToString();
        sphere.transform.parent = container.transform;
    }

}

