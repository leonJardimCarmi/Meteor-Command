using UnityEngine;

// Builds the city buildings in code, as stand-ins for real building models. A building is a tower made of boxes
// stacked in tiers, with an antenna on the roof, and every style is a different skyline.
//
// The window texture (drawn by Tools/GenerateCityTextures.py) holds four tiles side by side. One tile covers one
// unit of building width and one unit of building height, with 8 columns and 12 rows of windows, so the sizes
// below are counted in columns and rows and the windows are never stretched. The top of a tile has a neon line,
// which lands on the top edge of every tier, and the dark strip at the left edge of a tile gives a plain wall
// colour for roofs and antennas.
//
// A building stands on the floor of a unit cube centred on the origin, like the cube it replaces, but it may be
// taller. It stands a little behind the play plane, so meteors and explosions are drawn in front of it.
public static class BuildingMesh
{
    // How far behind the play plane the buildings stand, in units of the building cube.
    public const float DepthShift = 0.3f;
    public const int StyleCount = 6;

    private const int TileCount = 4;
    private const float TilePixels = 512f;
    private const float TextureWidth = TileCount * TilePixels;
    private const float TextureHeight = 576f;
    private const float Floor = -0.5f;
    private const float Column = 1f / 8f;
    private const float Row = 1f / 12f;
    private const float MastWidth = 0.04f;
    private const float TipWidth = 0.07f;
    private const float TipHeight = 0.04f;

    private static readonly Vector2 WallPixel = new Vector2(3f, 24f);
    private static readonly Vector2 NeonPixel = new Vector2(256f, 8f);

    // The four sides of a box, as the direction each one faces.
    private static readonly Vector3[] Facings = { Vector3.back, Vector3.right, Vector3.forward, Vector3.left };

    private enum Surface
    {
        Windows,
        Wall,
        Neon,
        Pyramid
    }

    // One box of a building: where it stands, how big it is, and what its surface looks like. A pyramid is a
    // pointed roof that rises from its base to a tip. The values are in units of the building cube.
    private readonly struct Tier
    {
        public readonly float CenterX;
        public readonly float CenterZ;
        public readonly float Width;
        public readonly float Depth;
        public readonly float Bottom;
        public readonly float Height;
        public readonly Surface Surface;

        public Tier(float centerX, float centerZ, float width, float depth, float bottom, float height, Surface surface)
        {
            CenterX = centerX;
            CenterZ = centerZ;
            Width = width;
            Depth = depth;
            Bottom = bottom;
            Height = height;
            Surface = surface;
        }
    }

    // The six skylines, from the first city to the last. Positions and sizes are in columns and rows.
    private static readonly Tier[][] Styles =
    {
        // A stepped tower, narrower at every step.
        new[]
        {
            Tower(0f, 0f, 8f, 4f, 0f, 6f),
            Tower(0f, 0f, 6f, 3f, 6f, 4f),
            Tower(0f, 0f, 4f, 2f, 10f, 3f),
            Mast(0f, 0f, 13f, 2f),
            Tip(0f, 0f, 15f)
        },

        // A tall slab with a lower wing beside it and machinery on its roof.
        new[]
        {
            Tower(-1.5f, 0f, 5f, 4f, 0f, 12f),
            Box(-2f, 0f, 2f, 2f, 12f, 1f),
            Mast(-2f, 0f, 13f, 3f),
            Tip(-2f, 0f, 16f),
            Tower(2.5f, 0f, 3f, 3f, 0f, 5f)
        },

        // Twin towers joined by a bridge.
        new[]
        {
            Tower(-2.5f, 0f, 3f, 3f, 0f, 16f),
            Mast(-2.5f, 0f, 16f, 2f),
            Tip(-2.5f, 0f, 18f),
            Tower(2.5f, 0f, 3f, 3f, 0f, 10f),
            Box(2.5f, 0f, 1f, 1f, 10f, 1f),
            Tower(0f, 0f, 2f, 2f, 5f, 1f)
        },

        // A wide low block with a tank and an antenna on the roof.
        new[]
        {
            Tower(0f, 0f, 8f, 4f, 0f, 8f),
            Tower(-1f, 0f, 5f, 3f, 8f, 2f),
            Box(2.5f, 0f, 2f, 2f, 8f, 1f),
            Box(-1f, 0f, 1.5f, 1.5f, 10f, 1.5f),
            Mast(-3f, 0f, 10f, 2f),
            Tip(-3f, 0f, 12f)
        },

        // A slim tower on a wide base, with a tall mast.
        new[]
        {
            Tower(0f, 0f, 8f, 4f, 0f, 3f),
            Tower(0f, 0f, 4f, 3f, 3f, 12f),
            Tower(0f, 0f, 3f, 2f, 15f, 1f),
            Mast(0f, 0f, 16f, 3f),
            Tip(0f, 0f, 19f)
        },

        // A tower with a pointed roof.
        new[]
        {
            Tower(0f, 0f, 8f, 4f, 0f, 4f),
            Tower(0f, 0f, 6f, 3f, 4f, 3f),
            Tower(0f, 0f, 4f, 2f, 7f, 3f),
            Pyramid(0f, 0f, 4f, 2f, 10f, 3f),
            Tip(0f, 0f, 13f)
        }
    };

    private static readonly Mesh[] _built = new Mesh[StyleCount];

    public static Mesh Style(int index)
    {
        int style = Mathf.Abs(index) % StyleCount;

        if (_built[style] == null)
        {
            _built[style] = Build(style);
        }

        return _built[style];
    }

    // A box with windows on its sides. All the sizes below are counted in columns and rows.
    private static Tier Tower(float x, float z, float width, float depth, float bottom, float height)
    {
        return new Tier(x * Column, z * Column, width * Column, depth * Column, bottom * Row, height * Row, Surface.Windows);
    }

    // A plain box without windows, for machinery on the roof.
    private static Tier Box(float x, float z, float width, float depth, float bottom, float height)
    {
        return new Tier(x * Column, z * Column, width * Column, depth * Column, bottom * Row, height * Row, Surface.Wall);
    }

    private static Tier Pyramid(float x, float z, float width, float depth, float bottom, float height)
    {
        return new Tier(x * Column, z * Column, width * Column, depth * Column, bottom * Row, height * Row, Surface.Pyramid);
    }

    private static Tier Mast(float x, float z, float bottom, float height)
    {
        return new Tier(x * Column, z * Column, MastWidth, MastWidth, bottom * Row, height * Row, Surface.Wall);
    }

    // The glowing light on the top of a mast or a roof.
    private static Tier Tip(float x, float z, float bottom)
    {
        return new Tier(x * Column, z * Column, TipWidth, TipWidth, bottom * Row, TipHeight, Surface.Neon);
    }

    private static Mesh Build(int style)
    {
        // Neighbouring styles use different tiles, so neighbouring buildings light their windows differently.
        int tile = style * 3 % TileCount;
        FlatMeshBuilder builder = new FlatMeshBuilder();

        foreach (Tier tier in Styles[style])
        {
            AddTier(builder, tier, tile);
        }

        return builder.ToMesh("Building " + style);
    }

    private static void AddTier(FlatMeshBuilder builder, Tier tier, int tile)
    {
        float bottom = Floor + tier.Bottom;
        float top = bottom + tier.Height;
        Vector3 center = new Vector3(tier.CenterX, 0f, DepthShift + tier.CenterZ);

        for (int side = 0; side < Facings.Length; side++)
        {
            if (tier.Surface == Surface.Pyramid)
            {
                AddPyramidSide(builder, tier, tile, center, side, bottom, top);
            }
            else
            {
                AddSide(builder, tier, tile, center, side, bottom, top);
            }
        }

        if (tier.Surface != Surface.Pyramid)
        {
            AddRoof(builder, tier, tile, center, top);
        }
    }

    private static void AddSide(FlatMeshBuilder builder, Tier tier, int tile, Vector3 center, int side, float bottom, float top)
    {
        GetSideEdges(tier, center, side, out Vector3 left, out Vector3 right);
        Vector3 up = Vector3.up;

        GetSideUv(tier, tile, right - left, out Vector2 uvBottomLeft, out Vector2 uvBottomRight, out Vector2 uvTopRight, out Vector2 uvTopLeft);
        builder.AddQuad(left + up * bottom, right + up * bottom, right + up * top, left + up * top,
            uvBottomLeft, uvBottomRight, uvTopRight, uvTopLeft, Facings[side]);
    }

    private static void AddPyramidSide(FlatMeshBuilder builder, Tier tier, int tile, Vector3 center, int side, float bottom, float top)
    {
        GetSideEdges(tier, center, side, out Vector3 left, out Vector3 right);
        Vector3 tip = center + Vector3.up * top;
        Vector2 wall = PixelUv(tile, WallPixel);

        builder.AddTriangle(left + Vector3.up * bottom, right + Vector3.up * bottom, tip, wall, wall, wall, Facings[side] + Vector3.up);
    }

    private static void AddRoof(FlatMeshBuilder builder, Tier tier, int tile, Vector3 center, float top)
    {
        Vector3 halfWidth = Vector3.right * (tier.Width * 0.5f);
        Vector3 halfDepth = Vector3.forward * (tier.Depth * 0.5f);
        Vector3 roofCenter = center + Vector3.up * top;
        Vector2 uv = PixelUv(tile, tier.Surface == Surface.Neon ? NeonPixel : WallPixel);

        builder.AddQuad(roofCenter - halfWidth - halfDepth, roofCenter + halfWidth - halfDepth,
            roofCenter + halfWidth + halfDepth, roofCenter - halfWidth + halfDepth, uv, uv, uv, uv, Vector3.up);
    }

    // The bottom corners of one side of a box, as seen from outside: the left one and the right one.
    private static void GetSideEdges(Tier tier, Vector3 center, int side, out Vector3 left, out Vector3 right)
    {
        Vector3 facing = Facings[side];
        bool runsAlongX = side % 2 == 0;
        float sideWidth = runsAlongX ? tier.Width : tier.Depth;
        float distance = (runsAlongX ? tier.Depth : tier.Width) * 0.5f;
        Vector3 across = Vector3.Cross(Vector3.up, -facing) * (sideWidth * 0.5f);
        Vector3 sideCenter = center + facing * distance;

        left = sideCenter - across;
        right = sideCenter + across;
    }

    // Windows are mapped so that the top of the side is the top of a tile, where the neon line is.
    private static void GetSideUv(Tier tier, int tile, Vector3 sideVector, out Vector2 bottomLeft, out Vector2 bottomRight, out Vector2 topRight, out Vector2 topLeft)
    {
        if (tier.Surface != Surface.Windows)
        {
            Vector2 plain = PixelUv(tile, tier.Surface == Surface.Neon ? NeonPixel : WallPixel);
            bottomLeft = plain;
            bottomRight = plain;
            topRight = plain;
            topLeft = plain;
            return;
        }

        float u0 = tile * TilePixels / TextureWidth;
        float u1 = u0 + sideVector.magnitude / TileCount;
        float v0 = 1f - tier.Height;

        bottomLeft = new Vector2(u0, v0);
        bottomRight = new Vector2(u1, v0);
        topRight = new Vector2(u1, 1f);
        topLeft = new Vector2(u0, 1f);
    }

    // The position of one pixel of a tile in the texture.
    private static Vector2 PixelUv(int tile, Vector2 pixel)
    {
        return new Vector2((tile * TilePixels + pixel.x + 0.5f) / TextureWidth, 1f - (pixel.y + 0.5f) / TextureHeight);
    }
}
