using UnityEngine;

public class test : MonoBehaviour
{
    public float a;

    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;



        vertices[1] = vertices[2] + normals[1] * a;


        mesh.vertices = vertices;
    }

}
