using _2DGame.ExternalLibraries;
using _2DGame.LayerData;
using NetTopologySuite.Triangulate;
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
    public class SpriteLayer : Layer, Drawable, IDestroyable
    {
        public uint[,] EventData { get; private set; }
        public SpriteLayer() : base()
        {
            TileIDs = null;
            LayerTilemap = null;
            EventData = null;
        }

        public SpriteLayer(string tilesetFilename, TileData tileIDs) : base()
        {
            Initialize(tilesetFilename, tileIDs);
        }

        public void Initialize(string tilesetFilename, TileData tileIDs)
        {
            TileIDs = tileIDs;
            LayerTilemap = new Tilemap(tilesetFilename);
            LayerTilemap.Load(tileIDs, true);

            TileWidth = tileIDs.Width();
            TileHeight = tileIDs.Height();
            Width = Tilemap.TILE_SIZE * tileIDs.Width();
            Height = Tilemap.TILE_SIZE * tileIDs.Height();
        }

        public override void Update(View view)
        {
            // Do nothing
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (IsVisible)
            {
                LayerTilemap.Draw(target, states);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            EventData = new uint[0, 0];
        }
    }
}
