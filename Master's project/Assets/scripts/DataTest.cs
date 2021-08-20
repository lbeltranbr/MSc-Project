using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataTest : MonoBehaviour
{
    private DivideAndConquer dewall;
    private Incremental incremental;

    private List<Point> points;

    private GameObject[] s;
    private List<Plane> cutP;

    private List<Vector4> verts;

    void Start()
    {
        string path = Application.dataPath + "/" + gameObject.name + ".txt";
        if (!File.Exists(path))
            File.WriteAllText(path, gameObject.name + "\n\n");


    }

    // Update is called once per frame5
    void Update()
    {
        
    }
}
