using System.Collections.Generic;
using UnityEngine;

// Helpers for meshes built in code with flat shading: every triangle gets its own three vertices, so its
// shading is not smoothed with its neighbours. A triangle is turned to face the direction it is given, so
// the callers never have to think about the winding order.
public static class FlatMesh
{
    public static void AddTriangle(List<Vector3> vertices, List<int> triangles, Vector3 a, Vector3 b, Vector3 c, Vector3 facing)
    {
        if (Vector3.Dot(Vector3.Cross(b - a, c - a), facing) < 0f)
        {
            (b, c) = (c, b);
        }

        int start = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
    }

    public static void AddQuad(List<Vector3> vertices, List<int> triangles, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 facing)
    {
        AddTriangle(vertices, triangles, a, b, c, facing);
        AddTriangle(vertices, triangles, a, c, d, facing);
    }

    public static Mesh Build(string meshName, List<Vector3> vertices, List<int> triangles)
    {
        Mesh mesh = new Mesh { name = meshName };
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
