using System.Collections.Generic;
using UnityEngine;

public enum MeshSide
{
    Positive = 0,
    Negative = 1
}
public class SliceData
{
    private Mesh pSideM;
    private List<Vector3> pSideV;
    private List<int> pSideT;
    private List<Vector2> pSideUvs;
    private List<Vector3> pSideN;

    private Mesh nSideM;
    private List<Vector3> nSideV;
    private List<int> nSideT;
    private List<Vector2> nSideUvs;
    private List<Vector3> nSideN;

    private readonly List<Vector3> pointsP;
    private Plane plane;
    private Mesh mesh;
    public Mesh PositiveSideMesh
    {
        get
        {
            if (pSideM == null)
                pSideM = new Mesh();

            SetMeshData(MeshSide.Positive);
            return pSideM;
        }
    }
    public Mesh NegativeSideMesh
    {
        get
        {
            if (nSideM == null)
                nSideM = new Mesh();

            SetMeshData(MeshSide.Negative);

            return nSideM;
        }
    }

    public SliceData(Plane p, Mesh m)
    {
        pSideT = new List<int>();
        pSideV = new List<Vector3>();
        nSideT = new List<int>();
        nSideV = new List<Vector3>();
        pSideUvs = new List<Vector2>();
        nSideUvs = new List<Vector2>();
        pSideN = new List<Vector3>();
        nSideN = new List<Vector3>();
        pointsP = new List<Vector3>();
        plane = p;
        mesh = m;

        ComputeNewMeshes();
    }

    private void AddTrianglesNormalAndUvs(MeshSide side, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
    {
        if (side == MeshSide.Positive)
        {
            AddTrianglesNormalsAndUvs(ref pSideV, ref pSideT, ref pSideN, ref pSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
        }
        else
        {
            AddTrianglesNormalsAndUvs(ref nSideV, ref nSideT, ref nSideN, ref nSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
        }
    }
    private void AddTrianglesNormalsAndUvs(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
    {
        int tri1Index = vertices.IndexOf(vertex1);

        if (addFirst)
            ShiftTriangleIndeces(ref triangles);

        //If a the vertex already exists we just add a triangle reference to it, if not add the vert to the list and then add the tri index
        if (tri1Index > -1 && shareVertices)
            triangles.Add(tri1Index);
        else
        {
            if (normal1 == null)
                normal1 = ComputeNormal(vertex1, vertex2, vertex3);

            int? i = null;
            if (addFirst)
                i = 0;

            AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex1, (Vector3)normal1, uv1, i);
        }

        int tri2Index = vertices.IndexOf(vertex2);

        if (tri2Index > -1 && shareVertices)
            triangles.Add(tri2Index);
        else
        {
            if (normal2 == null)
                normal2 = ComputeNormal(vertex2, vertex3, vertex1);

            int? i = null;

            if (addFirst)
                i = 1;

            AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex2, (Vector3)normal2, uv2, i);
        }

        int tri3Index = vertices.IndexOf(vertex3);

        if (tri3Index > -1 && shareVertices)
            triangles.Add(tri3Index);
        else
        {
            if (normal3 == null)
                normal3 = ComputeNormal(vertex3, vertex1, vertex2);

            int? i = null;
            if (addFirst)
                i = 2;

            AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex3, (Vector3)normal3, uv3, i);
        }
    }
    private void AddVertNormalUv(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> triangles, Vector3 vertex, Vector3 normal, Vector2 uv, int? index)
    {
        if (index != null)
        {
            int i = (int)index;
            vertices.Insert(i, vertex);
            uvs.Insert(i, uv);
            normals.Insert(i, normal);
            triangles.Insert(i, i);
        }
        else
        {
            vertices.Add(vertex);
            normals.Add(normal);
            uvs.Add(uv);
            triangles.Add(vertices.IndexOf(vertex));
        }
    }
    private void ShiftTriangleIndeces(ref List<int> triangles)
    {
        for (int j = 0; j < triangles.Count; j += 3)
        {
            triangles[j] += +3;
            triangles[j + 1] += 3;
            triangles[j + 2] += 3;
        }
    }
    private void JoinPointsAlongPlane()
    {
        Vector3 halfway = GetHalfwayPoint(out float distance);

        for (int i = 0; i < pointsP.Count; i += 2)
        {
            Vector3 firstVertex;
            Vector3 secondVertex;

            firstVertex = pointsP[i];
            secondVertex = pointsP[i + 1];

            Vector3 normal3 = ComputeNormal(halfway, secondVertex, firstVertex);
            normal3.Normalize();

            var direction = Vector3.Dot(normal3, plane.normal);

            if (direction > 0)
            {
                AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
            }
            else
            {
                AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
            }
        }
    }
    private Vector3 GetHalfwayPoint(out float distance)
    {
        if (pointsP.Count > 0)
        {
            Vector3 firstPoint = pointsP[0];
            Vector3 furthestPoint = Vector3.zero;
            distance = 0f;

            foreach (Vector3 point in pointsP)
            {
                float currentDistance = 0f;
                currentDistance = Vector3.Distance(firstPoint, point);

                if (currentDistance > distance)
                {
                    distance = currentDistance;
                    furthestPoint = point;
                }
            }

            return Vector3.Lerp(firstPoint, furthestPoint, 0.5f);
        }
        else
        {
            distance = 0;
            return Vector3.zero;
        }
    }
    private void SetMeshData(MeshSide side)
    {
        if (side == MeshSide.Positive)
        {
            pSideM.vertices = pSideV.ToArray();
            pSideM.triangles = pSideT.ToArray();
            pSideM.normals = pSideN.ToArray();
            pSideM.uv = pSideUvs.ToArray();
        }
        else
        {
            nSideM.vertices = nSideV.ToArray();
            nSideM.triangles = nSideT.ToArray();
            nSideM.normals = nSideN.ToArray();
            nSideM.uv = nSideUvs.ToArray();
        }
    }
    private void ComputeNewMeshes()
    {
        int[] meshTriangles = mesh.triangles;
        Vector3[] meshVerts = mesh.vertices;
        Vector3[] meshNormals = mesh.normals;
        Vector2[] meshUvs = mesh.uv;

        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            //We need the verts in order so that we know which way to wind our new mesh triangles.
            Vector3 vert1 = meshVerts[meshTriangles[i]];
            int vert1Index = System.Array.IndexOf(meshVerts, vert1);
            Vector2 uv1 = meshUvs[vert1Index];
            Vector3 normal1 = meshNormals[vert1Index];
            bool vert1Side = plane.GetSide(vert1);

            Vector3 vert2 = meshVerts[meshTriangles[i + 1]];
            int vert2Index = System.Array.IndexOf(meshVerts, vert2);
            Vector2 uv2 = meshUvs[vert2Index];
            Vector3 normal2 = meshNormals[vert2Index];
            bool vert2Side = plane.GetSide(vert2);

            Vector3 vert3 = meshVerts[meshTriangles[i + 2]];
            bool vert3Side = plane.GetSide(vert3);
            int vert3Index = System.Array.IndexOf(meshVerts, vert3);
            Vector3 normal3 = meshNormals[vert3Index];
            Vector2 uv3 = meshUvs[vert3Index];

            //All verts are on the same side
            if (vert1Side == vert2Side && vert2Side == vert3Side)
            {
                //Add the relevant triangle
                MeshSide side = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
                AddTrianglesNormalAndUvs(side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
            }
            else
            {
                //we need the two points where the plane intersects the triangle.
                Vector3 intersection1;
                Vector3 intersection2;

                Vector2 intersection1Uv;
                Vector2 intersection2Uv;

                MeshSide side1 = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
                MeshSide side2 = (vert1Side) ? MeshSide.Negative : MeshSide.Positive;

                //vert 1 and 2 are on the same side
                if (vert1Side == vert2Side)
                {
                    //Cast a ray from v2 to v3 and from v3 to v1 to get the intersections                       
                    intersection1 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection1Uv);
                    intersection2 = GetRayPlaneIntersectionPointAndUv(vert3, uv3, vert1, uv1, out intersection2Uv);

                    //Add the positive or negative triangles
                    AddTrianglesNormalAndUvs(side1, vert1, null, uv1, vert2, null, uv2, intersection1, null, intersection1Uv, false, false);
                    AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, false, false);

                    AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, false, false);

                }
                //vert 1 and 3 are on the same side
                else if (vert1Side == vert3Side)
                {
                    //Cast a ray from v1 to v2 and from v2 to v3 to get the intersections                       
                    intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                    intersection2 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection2Uv);

                    //Add the positive triangles
                    AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, vert3, null, uv3, false, false);
                    AddTrianglesNormalAndUvs(side1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, vert3, null, uv3, false, false);

                    AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, intersection2, null, intersection2Uv, false, false);
                }
                //Vert1 is alone
                else
                {
                    //Cast a ray from v1 to v2 and from v1 to v3 to get the intersections                       
                    intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                    intersection2 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert3, uv3, out intersection2Uv);

                    AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, false, false);

                    AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, vert3, null, uv3, false, false);
                    AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, false, false);
                }

                //Add the newly created points on the plane.
                pointsP.Add(intersection1);
                pointsP.Add(intersection2);
            }
        }

        JoinPointsAlongPlane();


    }
    private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
    {
        float distance = GetDistanceRelativeToPlane(vertex1, vertex2, out Vector3 pointOfIntersection);
        uv = InterpolateUvs(vertex1Uv, vertex2Uv, distance);
        return pointOfIntersection;
    }
    private float GetDistanceRelativeToPlane(Vector3 vertex1, Vector3 vertex2, out Vector3 pointOfintersection)
    {
        Ray ray = new Ray(vertex1, (vertex2 - vertex1));
        plane.Raycast(ray, out float distance);
        pointOfintersection = ray.GetPoint(distance);
        return distance;
    }
    private Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
    {
        Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
        return uv;
    }
    private Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 side1 = vertex2 - vertex1;
        Vector3 side2 = vertex3 - vertex1;

        Vector3 normal = Vector3.Cross(side1, side2);

        return normal;
    }
    private List<Vector3> FlipNormals(List<Vector3> currentNormals)
    {
        List<Vector3> flippedNormals = new List<Vector3>();

        foreach (Vector3 normal in currentNormals)
        {
            flippedNormals.Add(-normal);
        }

        return flippedNormals;
    }


}
