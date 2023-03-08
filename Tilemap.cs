using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame
{
    public class Tilemap : Transformable, Drawable
    {
        public const uint TILE_SIZE = 32;
        private VertexArray Vertices { get; set; }
        private Texture? Tileset { get; set; }

        public bool Load(uint[,] tileIDs)
        {
            uint width = (uint)tileIDs.GetLength(1);
            uint height = (uint)tileIDs.GetLength(0);

            Vertices.PrimitiveType = PrimitiveType.Quads;
            Vertices.Resize(width * height * 4);

            for (uint i = 0; i < width; ++i)
            {
                for (uint j = 0; j < height; ++j)
                {
                    uint tileNumber = tileIDs[j, i];

                    uint tileX = tileNumber % (Tileset.Size.X / TILE_SIZE);
                    uint tileY = tileNumber / (Tileset.Size.X / TILE_SIZE);

                    uint index = (i + j * width) * 4;

                    Vertices[index + 0] = new Vertex(new Vector2f(i * TILE_SIZE, j * TILE_SIZE), new Vector2f(tileX * TILE_SIZE, tileY * TILE_SIZE));
                    Vertices[index + 1] = new Vertex(new Vector2f((i + 1) * TILE_SIZE, j * TILE_SIZE), new Vector2f((tileX + 1) * TILE_SIZE, tileY * TILE_SIZE));
                    Vertices[index + 2] = new Vertex(new Vector2f((i + 1) * TILE_SIZE, (j + 1) * TILE_SIZE), new Vector2f((tileX + 1) * TILE_SIZE, (tileY + 1) * TILE_SIZE));
                    Vertices[index + 3] = new Vertex(new Vector2f(i * TILE_SIZE, (j + 1) * TILE_SIZE), new Vector2f(tileX * TILE_SIZE, (tileY + 1) * TILE_SIZE));

                    //Debug.WriteLine("Vertices[" + index + 0 + "] = new Vertex(new Vector2f(" + i * TILE_SIZE + ", " + j * TILE_SIZE + "), new Vector2f(" + tileX * TILE_SIZE + ", " + tileY * TILE_SIZE + "));");
                }
                    
            }
            return true;
        }

        public Tilemap(string tilesetFilename)
        {
            Tileset = new Texture(TextureManager.TILESETS_PATH + tilesetFilename);
            Vertices = new VertexArray();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Texture = Tileset;
            target.Draw(Vertices, states);
        }
    }
}
