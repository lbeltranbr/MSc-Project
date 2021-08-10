using UnityEngine;
using System.Collections.Generic;


public class test : MonoBehaviour
{
    public float a;

    private void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Debug.Log(vertices.Length);
        Vector3[] normals = mesh.normals;

        List<Vector3> v = new List<Vector3>();
       
        foreach (var i in vertices)
        {
            if (!v.Contains(i))
                v.Add(i);
        }
        foreach (var i in v)
            Debug.Log(i);
    }
   

}
