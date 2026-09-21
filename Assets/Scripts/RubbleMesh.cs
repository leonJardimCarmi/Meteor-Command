using System.Collections.Generic;
using UnityEngine;

// Builds a pile of rubble in code, as a stand-in for a real rubble model: broken walls and a broken
// corner column still standing in a heap of chunks and fallen slabs. The pile sits on the floor of a unit
// cube centred on the origin, like the city cube it replaces, so the city can swap its mesh and keep its own
// scale and position. The pieces come from a fixed random seed, so every ruin has the same shape.
// The mesh is built once and shared.
public static class RubbleMesh
{
    private const float Floor = -0.5f;
    private const int Seed = 4;
    private const int ChunkCount = 30;
    private const int SlabCount = 5;
    private const int DebrisCount = 10;
    private const float PileRadius = 0.42f;
    private const float DebrisRadius = 0.62f;

    // One box of the pile. Angles are the pitch, yaw and roll of the box, and TopDrop lowers each of the four
    // top corners by its own amount, which breaks the top edge.
    private readonly struct Piece
    {
        public readonly Vector3 FloorPoint;
        public readonly Vector3 Size;
        public readonly Vector3 Angles;
        public readonly Vector4 TopDrop;

        public Piece(Vector3 floorPoint, Vector3 size, Vector3 angles, Vector4 topDrop)
        {
            FloorPoint = floorPoint;
            Size = size;
            Angles = angles;
            TopDrop = topDrop;
        }
    }

    // The parts of the building that are still standing: a back wall, a left wall and a corner column with
    // broken tops, a low stub of wall, and a fallen slab that leans against the back wall.
    private static readonly Piece[] Standing =
    {
        new Piece(new Vector3(-0.08f, Floor, 0.32f), new Vector3(0.68f, 0.8f, 0.08f), new Vector3(0f, 3f, -2f), new Vector4(0.05f, 0.36f, 0.36f, 0.05f)),
        new Piece(new Vector3(-0.38f, Floor, 0f), new Vector3(0.08f, 0.68f, 0.56f), new Vector3(0f, -3f, 3f), new Vector4(0.34f, 0.34f, 0f, 0f)),
        new Piece(new Vector3(0.30f, Floor, -0.22f), new Vector3(0.14f, 0.7f, 0.14f), new Vector3(-4f, 25f, 7f), new Vector4(0.06f, 0.2f, 0f, 0.1f)),
        new Piece(new Vector3(0.22f, Floor, 0.12f), new Vector3(0.32f, 0.32f, 0.07f), new Vector3(0f, 14f, 0f), new Vector4(0.12f, 0f, 0f, 0.12f)),
        new Piece(new Vector3(-0.10f, Floor + 0.02f, 0.24f), new Vector3(0.28f, 0.04f, 0.24f), new Vector3(-30f, 8f, 0f), Vector4.zero)
    };

    private static Mesh _shared;

    public static Mesh Shared
    {
        get
        {
            if (_shared == null)
            {
                _shared = Build();
            }

            return _shared;
        }
    }

    private static Mesh Build()
    {
        System.Random random = new System.Random(Seed);
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (Piece piece in Standing)
        {
            AddPiece(vertices, triangles, piece);
        }

        for (int i = 0; i < ChunkCount; i++)
        {
            AddPiece(vertices, triangles, RandomChunk(random));
        }

        for (int i = 0; i < SlabCount; i++)
        {
            AddPiece(vertices, triangles, RandomSlab(random));
        }

        for (int i = 0; i < DebrisCount; i++)
        {
            AddPiece(vertices, triangles, RandomDebris(random));
        }

        return FlatMesh.Build("Rubble", vertices, triangles);
    }

    // A small block lying in the pile, turned and tilted at random, with a slightly broken top.
    private static Piece RandomChunk(System.Random random)
    {
        Vector3 size = new Vector3(Next(random, 0.12f, 0.34f), Next(random, 0.08f, 0.22f), Next(random, 0.12f, 0.34f));
        Vector3 angles = new Vector3(Next(random, -22f, 22f), Next(random, 0f, 360f), Next(random, -22f, 22f));

        return new Piece(PointOnFloor(random, 0f, PileRadius, 0.12f), size, angles, RandomTopDrop(random, size.y * 0.5f));
    }

    // A flat piece of floor or roof that fell and lies almost level.
    private static Piece RandomSlab(System.Random random)
    {
        Vector3 size = new Vector3(Next(random, 0.3f, 0.44f), Next(random, 0.03f, 0.05f), Next(random, 0.22f, 0.32f));
        Vector3 angles = new Vector3(Next(random, -8f, 8f), Next(random, 0f, 360f), Next(random, -8f, 8f));

        return new Piece(PointOnFloor(random, 0f, PileRadius, 0.02f), size, angles, Vector4.zero);
    }

    // A small stone that was thrown clear of the pile.
    private static Piece RandomDebris(System.Random random)
    {
        Vector3 size = new Vector3(Next(random, 0.05f, 0.11f), Next(random, 0.04f, 0.09f), Next(random, 0.05f, 0.11f));
        Vector3 angles = new Vector3(Next(random, -25f, 25f), Next(random, 0f, 360f), Next(random, -25f, 25f));

        return new Piece(PointOnFloor(random, PileRadius, DebrisRadius, 0f), size, angles, Vector4.zero);
    }

    // A random spot on the floor between two distances from the centre, lifted a little so that some pieces
    // rest on others.
    private static Vector3 PointOnFloor(System.Random random, float minDistance, float maxDistance, float maxLift)
    {
        float angle = Next(random, 0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Next(random, minDistance * minDistance, maxDistance * maxDistance));

        return new Vector3(Mathf.Cos(angle) * distance, Floor + Next(random, 0f, maxLift), Mathf.Sin(angle) * distance);
    }

    private static Vector4 RandomTopDrop(System.Random random, float maxDrop)
    {
        return new Vector4(Next(random, 0f, maxDrop), Next(random, 0f, maxDrop), Next(random, 0f, maxDrop), Next(random, 0f, maxDrop));
    }

    private static float Next(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    // A box standing on the floor. The rotation turns and leans it around its base. The underside is never
    // seen, so it is left out. The corners are numbered around the box: front left, front right, back right,
    // back left (the camera looks along +Z, so +Z is the back).
    private static void AddPiece(List<Vector3> vertices, List<int> triangles, Piece piece)
    {
        Quaternion rotation = Quaternion.Euler(piece.Angles);
        Vector3[] bottom = new Vector3[4];
        Vector3[] top = new Vector3[4];

        for (int corner = 0; corner < 4; corner++)
        {
            float x = (corner == 1 || corner == 2 ? 0.5f : -0.5f) * piece.Size.x;
            float z = (corner >= 2 ? 0.5f : -0.5f) * piece.Size.z;

            bottom[corner] = piece.FloorPoint + rotation * new Vector3(x, 0f, z);
            top[corner] = piece.FloorPoint + rotation * new Vector3(x, piece.Size.y - piece.TopDrop[corner], z);
        }

        Vector3 center = piece.FloorPoint + rotation * new Vector3(0f, piece.Size.y * 0.5f, 0f);
        FlatMesh.AddQuad(vertices, triangles, top[0], top[1], top[2], top[3], rotation * Vector3.up);

        for (int side = 0; side < 4; side++)
        {
            int next = (side + 1) % 4;
            Vector3 outward = (bottom[side] + bottom[next] + top[side] + top[next]) / 4f - center;

            FlatMesh.AddQuad(vertices, triangles, bottom[side], bottom[next], top[next], top[side], outward);
        }
    }
}
