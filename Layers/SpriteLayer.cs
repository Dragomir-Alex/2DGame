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
    public class SpriteLayer : Layer, Drawable
    {
        public TileData? TileIDs { get; set; }
        public Tilemap LayerTilemap { get; set; }

        public SpriteLayer() : base()
        {
            TileIDs = null;
            LayerTilemap = null;
        }

        public SpriteLayer(string tilesetFilename, TileData tileIDs, bool initializeHitboxDirectory) : base()
        {
            Initialize(tilesetFilename, tileIDs, initializeHitboxDirectory);
        }

        public void Initialize(string tilesetFilename, TileData tileIDs, bool initializeHitboxDirectory)
        {
            TileIDs = tileIDs;
            LayerTilemap = new Tilemap(tilesetFilename);
            LayerTilemap.Load(tileIDs, initializeHitboxDirectory);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            LayerTilemap.Draw(target, states);
        }
    }
}
