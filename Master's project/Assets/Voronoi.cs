using System.Collections.Generic;
using UnityEngine;

public class Voronoi : MonoBehaviour
{
    public int pointsAmount;
    public GameObject container;
    public bool grayScale;

    private List<Vector4> points;
    private List<Vector4> color_points;
    void Start()
    {
        getRegions();
    }
    private void Update()
    {
        
        if(grayScale)
            gameObject.GetComponent<Renderer>().material.SetInt("_change",1);
        else
            gameObject.GetComponent<Renderer>().material.SetInt("_change",0);

    }

    public void getRegions()
    {
        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
        gameObject.GetComponent<Renderer>().material.SetInt("_pointsAmount", pointsAmount);
    
        points = new List<Vector4>();
        color_points = new List<Vector4>();

        for (int i = 0; i < pointsAmount; i++)
        {
            points.Add(new Vector4(Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2)+transform.position.x, Random.Range(-transform.localScale.y / 2, transform.localScale.y/2)+transform.position.y, Random.Range(-transform.localScale.z / 2, transform.localScale.z / 2)+transform.position.z, 0));
            color_points.Add(new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Renderer>().material.SetColor("_Color", color_points[i]);
            sphere.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
            sphere.transform.position = points[i];
            sphere.name = "Sphere" + i.ToString();
            sphere.transform.parent = container.transform;

        }

        gameObject.GetComponent<Renderer>().material.SetVectorArray("_pointPos", points);
        gameObject.GetComponent<Renderer>().material.SetVectorArray("_colorPoint", color_points);
    }

}
