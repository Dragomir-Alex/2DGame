using _2DGame.Entities;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TransformableHitbox2D;

namespace _2DGame.LayerData
{
    public class Tilemap : Drawable, IDisposable
    {
        public const uint TILE_SIZE = 32;
        //public enum MaskShape { Square } // More TBA maybe, too lazy atm
        public Dictionary<(int, int), Hitbox> TileHitboxData { get; }
        public VertexArray Vertices { get; set; }
        public Texture? Tileset { get; set; }
        public uint PixelWidth { get; private set; }
        public uint PixelHeight { get; private set; }

        public bool Load(TileData tileIDs, bool initializeHitboxDictionary)
        {
            if (tileIDs == null)
                return false;

            int width = tileIDs.Width();
            int height = tileIDs.Height();

            PixelWidth = (uint)width * TILE_SIZE;
            PixelHeight = (uint)height * TILE_SIZE;

            Vertices.PrimitiveType = PrimitiveType.Quads;
            Vertices.Resize((uint)(width * height * 4));

            for (uint i = 0; i < width; ++i)
            {
                for (uint j = 0; j < height; ++j)
                {
                    int tileNumber = tileIDs.GetTile(((int, int))(i, j));

                    if (tileNumber == -1)
                        continue;

                    if (initializeHitboxDictionary) // Initialize hitbox dictionary
                    {
                        Vector2[] vector2Arr = new Vector2[] { new Vector2(0, 0), new Vector2(TILE_SIZE, 0), new Vector2(TILE_SIZE, TILE_SIZE), new Vector2(0, TILE_SIZE), new Vector2(0, 0) };
                        Hitbox hitbox = new Hitbox(vector2Arr);
                        TransformableHitbox2D.Transform transform = new();
                        transform.Position = new Vector2(i * TILE_SIZE, j * TILE_SIZE);

                        hitbox.Transform(transform);
                        TileHitboxData.Add(((int)j, (int)i), hitbox);
                    }

                    uint tileX = (uint)(tileNumber % (Tileset.Size.X / TILE_SIZE));
                    uint tileY = (uint)(tileNumber / (Tileset.Size.X / TILE_SIZE));

                    uint index = (uint)((i + j * width) * 4);

                    Vertices[index + 0] = new Vertex(new Vector2f(i * TILE_SIZE, j * TILE_SIZE), new Vector2f(tileX * TILE_SIZE, tileY * TILE_SIZE));
                    Vertices[index + 1] = new Vertex(new Vector2f((i + 1) * TILE_SIZE, j * TILE_SIZE), new Vector2f((tileX + 1) * TILE_SIZE, tileY * TILE_SIZE));
                    Vertices[index + 2] = new Vertex(new Vector2f((i + 1) * TILE_SIZE, (j + 1) * TILE_SIZE), new Vector2f((tileX + 1) * TILE_SIZE, (tileY + 1) * TILE_SIZE));
                    Vertices[index + 3] = new Vertex(new Vector2f(i * TILE_SIZE, (j + 1) * TILE_SIZE), new Vector2f(tileX * TILE_SIZE, (tileY + 1) * TILE_SIZE));
                }
            }

            return true;
        }

        public Tilemap(string tilesetFilename)
        {
            Tileset = new Texture(TextureManager.TILESETS_PATH + tilesetFilename);
            Vertices = new VertexArray();
            TileHitboxData = new Dictionary<(int, int), Hitbox>();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Tileset;
            target.Draw(Vertices, states);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Vertices.Dispose();
                TileHitboxData.Clear();
                Tileset.Dispose();
                PixelWidth = 0;
                PixelHeight = 0;
            }
        }
    }
}
