using _2DGame.LayerData;
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
        public SpriteLayer() : base()
        {
            TileIDs = null;
            LayerTilemap = null;
        }

        public SpriteLayer(Texture tileset, TileData tileIDs) : base()
        {
            Initialize(tileset, tileIDs);
        }

        public void Initialize(Texture tileset, TileData tileIDs)
        {
            TileIDs = tileIDs;
            LayerTilemap = new Tilemap(tileset);
            LayerTilemap.Load(tileIDs, true);

            TileWidth = tileIDs.Width();
            TileHeight = tileIDs.Height();
            Width = Tilemap.TILE_SIZE * tileIDs.Width();
            Height = Tilemap.TILE_SIZE * tileIDs.Height();
        }

        public override void Update(View view, float deltaTime, float timeUntilUpdate) { }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (IsVisible)
            {
                LayerTilemap.Draw(target, states);
            }
        }
    }
}
