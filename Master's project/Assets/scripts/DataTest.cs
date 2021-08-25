using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
public class DataTest : MonoBehaviour
{
    public GameObject low, medium, high;
    public int points1, points2, points3, points4;

    private DivideAndConquer dewall;
    private Incremental incremental;

    private List<Point> points;
    private StreamWriter writer;
    private Stopwatch st;
    void Start()
    {
        RunCode();
    }

    public void RunCode()
    {
        st = new Stopwatch();
        string path = Application.dataPath + "/" + gameObject.name + ".txt";
        File.Delete(path);
        writer = new StreamWriter(path, true);

        writer.WriteLine("TIME IS MEASURED IN MILISECONDS\n");
        writer.WriteLine("LOW POLY:\t" + low.GetComponent<MeshFilter>().mesh.triangles.Length);
        writer.WriteLine("MED POLY:\t" + medium.GetComponent<MeshFilter>().mesh.triangles.Length);
        writer.WriteLine("HIGH POLY:\t" + high.GetComponent<MeshFilter>().mesh.triangles.Length);
        writer.WriteLine("\n");


        CalculateTriangulation(points1);
        CalculateTriangulation(points2);
        CalculateTriangulation(points3);
        CalculateTriangulation(points4);


        writer.Close();


    }
    public void CalculateTriangulation(int p)
    {
        /***************LOW***************/

        st.Start();
        getPoints(p, low);
        st.Stop();
        writer.WriteLine("Time to generate " + p + " points low:\t" + st.Elapsed);
        st.Reset();

        st.Start();
        incremental = new Incremental(points, low.GetComponent<BoxCollider>().center + low.transform.position, low.GetComponent<BoxCollider>().size, false);
        st.Stop();
        writer.WriteLine("Time to generate low incremental:\t" + st.Elapsed);
        st.Reset();

        st.Start();
        dewall = new DivideAndConquer(points, low.GetComponent<BoxCollider>().center + low.transform.position, low.GetComponent<BoxCollider>().size, false);
        st.Stop();
        writer.WriteLine("Time to generate low d&c:\t" + st.Elapsed);
        st.Reset();
        writer.WriteLine("\n");

        /***************MED***************/

        st.Start();
        getPoints(p, medium);
        st.Stop();
        writer.WriteLine("Time to generate " + p + " points med:\t" + st.Elapsed);
        st.Reset();

        st.Start();
        incremental = new Incremental(points, medium.GetComponent<BoxCollider>().center + medium.transform.position, medium.GetComponent<BoxCollider>().size, false);
        st.Stop();
        writer.WriteLine("Time to generate medium incremental:\t" + st.Elapsed);
        st.Reset();

        st.Start();
        dewall = new DivideAndConquer(points, medium.GetComponent<BoxCollider>().center + medium.transform.position, medium.GetComponent<BoxCollider>().size, false);
        st.Stop();
        
        writer.WriteLine("Time to generate medium d&c:\t" + st.Elapsed);
        st.Reset();
        writer.WriteLine("\n");

        /***************HIGH***************/

        st.Start();
        getPoints(p, high);
        st.Stop();
        writer.WriteLine("Time to generate " + p + " points high:\t" + st.Elapsed);
        st.Reset();

        st.Start();
        incremental = new Incremental(points, high.GetComponent<BoxCollider>().center + high.transform.position, high.GetComponent<BoxCollider>().size, false);
        st.Stop();
        writer.WriteLine("Time to generate high incremental:\t" + st.Elapsed);
        st.Reset();

        st.Start();
        dewall = new DivideAndConquer(points, high.GetComponent<BoxCollider>().center + high.transform.position, high.GetComponent<BoxCollider>().size, false);
        st.Stop();
        writer.WriteLine("Time to generate high d&c:\t" + st.Elapsed);
        st.Reset();
        writer.WriteLine("\n");
    }
    public void getPoints(int pointsAmount, GameObject obj)
    {

        points = new List<Point>();

        for (int i = 0; i < pointsAmount; i++)
        {

            points.Add(new Point(Random.Range(-obj.GetComponent<BoxCollider>().size.x / 2, obj.GetComponent<BoxCollider>().size.x / 2) + obj.GetComponent<BoxCollider>().center.x + obj.transform.position.x,
                Random.Range(-obj.GetComponent<BoxCollider>().size.y / 2, obj.GetComponent<BoxCollider>().size.y / 2) + obj.GetComponent<BoxCollider>().center.y + obj.transform.position.y,
                Random.Range(-obj.GetComponent<BoxCollider>().size.z / 2, obj.GetComponent<BoxCollider>().size.z / 2) + obj.GetComponent<BoxCollider>().center.z + obj.transform.position.z, i));
        }

    }
}
