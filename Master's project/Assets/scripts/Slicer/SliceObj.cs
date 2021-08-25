using UnityEngine;

public class SliceObj
{
    public static GameObject[] Slice(Plane plane, GameObject objectToCut)
    {
        //Get the current mesh and its verts and tris
        Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
        var a = mesh.GetSubMesh(0);
        SliceProp prop = objectToCut.GetComponent<SliceProp>();

        if (prop == null)
        {
            Debug.LogError("SliceProp script missing in the object");
            return null;
        }
        //Create left and right slice of hollow object
        SliceData slicesMeta = new SliceData(plane, mesh);

        GameObject positiveObject = CreateMeshGameObject(objectToCut);
        positiveObject.name = string.Format("{0}_positive", objectToCut.name);

        GameObject negativeObject = CreateMeshGameObject(objectToCut);
        negativeObject.name = string.Format("{0}_negative", objectToCut.name);

        var positiveSideMeshData = slicesMeta.PositiveSideMesh;
        var negativeSideMeshData = slicesMeta.NegativeSideMesh;

        positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
        negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

        SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, prop.useGravity, prop.isKinematic);
        SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, prop.useGravity, prop.isKinematic);

        return new GameObject[] { positiveObject, negativeObject };
    }
    private static GameObject CreateMeshGameObject(GameObject originalObject)
    {
        var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

        GameObject meshGameObject = new GameObject();
        SliceProp originalSliceProp = originalObject.GetComponent<SliceProp>();

        meshGameObject.AddComponent<MeshFilter>();
        meshGameObject.AddComponent<MeshRenderer>();
        SliceProp SliceProp = meshGameObject.AddComponent<SliceProp>();

        SliceProp.useGravity = originalSliceProp.useGravity;

        meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

        meshGameObject.transform.localScale = originalObject.transform.localScale;
        meshGameObject.transform.rotation = originalObject.transform.rotation;
        meshGameObject.transform.position = originalObject.transform.position;

        meshGameObject.tag = originalObject.tag;

        return meshGameObject;
    }
    private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity, bool isKinematic)
    {
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;

        var rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = useGravity;
        rb.isKinematic = isKinematic;

        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));

    }
}
