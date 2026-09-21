using UnityEngine;

// Builds a small low poly missile mesh in code: a pointed nose, a round body, a narrower tail and four fins.
// The missile points along +Z and is centred on the origin. The mesh is built once and shared.
public static class MissileMesh
{
    private const int Sides = 8;
    private const float FinSpan = 0.36f;

    // Profile of the body as (radius, position along Z), from the tail to the tip.
    private static readonly Vector2[] Profile =
    {
        new Vector2(0.09f, -0.80f),
        new Vector2(0.13f, -0.72f),
        new Vector2(0.16f, -0.55f),
        new Vector2(0.16f, 0.30f),
        new Vector2(0.13f, 0.48f),
        new Vector2(0.09f, 0.62f),
        new Vector2(0.04f, 0.74f),
        new Vector2(0f, 0.80f)
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
        FlatMeshBuilder builder = new FlatMeshBuilder();

        AddBody(builder);
        AddFins(builder);

        return builder.ToMesh("Missile");
    }

    private static void AddBody(FlatMeshBuilder builder)
    {
        for (int ring = 0; ring < Profile.Length - 1; ring++)
        {
            for (int side = 0; side < Sides; side++)
            {
                Vector3 a = RingPoint(Profile[ring], side);
                Vector3 b = RingPoint(Profile[ring], side + 1);
                Vector3 c = RingPoint(Profile[ring + 1], side + 1);
                Vector3 d = RingPoint(Profile[ring + 1], side);
                Vector3 outward = (a + b + c + d) / 4f - new Vector3(0f, 0f, (a.z + c.z) / 2f);

                builder.AddQuad(a, b, c, d, outward);
            }
        }
    }

    // Four flat fins around the tail. Each is drawn from both sides, so it is visible from any angle.
    private static void AddFins(FlatMeshBuilder builder)
    {
        for (int fin = 0; fin < 4; fin++)
        {
            Vector3 radial = Quaternion.AngleAxis(fin * 90f, Vector3.forward) * Vector3.right;
            Vector3 sideways = Vector3.Cross(Vector3.forward, radial);

            Vector3 rootFront = radial * 0.15f + new Vector3(0f, 0f, -0.45f);
            Vector3 rootBack = radial * 0.15f + new Vector3(0f, 0f, -0.78f);
            Vector3 tip = radial * FinSpan + new Vector3(0f, 0f, -0.82f);

            builder.AddTriangle(rootFront, rootBack, tip, sideways);
            builder.AddTriangle(rootFront, rootBack, tip, -sideways);
        }
    }

    private static Vector3 RingPoint(Vector2 profilePoint, int side)
    {
        float angle = side * Mathf.PI * 2f / Sides;
        return new Vector3(Mathf.Cos(angle) * profilePoint.x, Mathf.Sin(angle) * profilePoint.x, profilePoint.y);
    }
}
