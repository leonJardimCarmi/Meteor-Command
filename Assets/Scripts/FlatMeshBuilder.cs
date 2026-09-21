using System.Collections.Generic;
using UnityEngine;

// Collects the triangles of a mesh that is built in code with flat shading: every triangle gets its own three
// vertices, so its shading is not smoothed with its neighbours. A triangle is turned to face the direction it is
// given, so the callers never have to think about the winding order.
public class FlatMeshBuilder
{
    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly List<int> _triangles = new List<int>();

    public void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 facing)
    {
        AddTriangle(a, b, c, Vector2.zero, Vector2.zero, Vector2.zero, facing);
    }

    public void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector3 facing)
    {
        if (Vector3.Dot(Vector3.Cross(b - a, c - a), facing) < 0f)
        {
            (b, c) = (c, b);
            (uvB, uvC) = (uvC, uvB);
        }

        int start = _vertices.Count;
        _vertices.Add(a);
        _vertices.Add(b);
        _vertices.Add(c);
        _uvs.Add(uvA);
        _uvs.Add(uvB);
        _uvs.Add(uvC);
        _triangles.Add(start);
        _triangles.Add(start + 1);
        _triangles.Add(start + 2);
    }

    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 facing)
    {
        AddTriangle(a, b, c, facing);
        AddTriangle(a, c, d, facing);
    }

    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD, Vector3 facing)
    {
        AddTriangle(a, b, c, uvA, uvB, uvC, facing);
        AddTriangle(a, c, d, uvA, uvC, uvD, facing);
    }

    public Mesh ToMesh(string meshName)
    {
        Mesh mesh = new Mesh { name = meshName };
        mesh.SetVertices(_vertices);
        mesh.SetUVs(0, _uvs);
        mesh.SetTriangles(_triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
