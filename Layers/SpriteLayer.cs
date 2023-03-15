using _2DGame.LayerData;
using _2DGame.LevelData;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public class SpriteLayer : Drawable
    {
        public uint[,] TileIDs { get; set; }
        public Tilemap LayerTilemap { get; set; }
        public Mask? LayerMask { get; set; }

        public SpriteLayer(string tilesetFilename, string maskFilename, uint[,] tileIDs)
        {
            TileIDs = tileIDs;
            LayerTilemap = new Tilemap(tilesetFilename);
            LayerTilemap.Load(tileIDs);
            LayerMask = new Mask(maskFilename);
            LayerMask.Load(this);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            LayerTilemap.Draw(target, states);
        }
    }
}
