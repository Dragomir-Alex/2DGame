using _2DGame.ExternalLibraries;
using _2DGame.LayerData;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public abstract class Layer : Drawable
    {
        public float XSpeed { get; set; }
        public float YSpeed { get; set; }
        public float AutoXSpeed { get; set; }
        public float AutoYSpeed { get; set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public int TileWidth { get; protected set; }
        public int TileHeight { get; protected set; }
        public TileData? TileIDs { get; set; }
        public Tilemap LayerTilemap { get; set; }

        public Layer()
        {
            XSpeed = 1f;
            YSpeed = 1f;
            AutoXSpeed = 0f;
            AutoYSpeed = 0f;
            Width = 0f;
            Height = 0f;
            TileWidth = 0;
            TileHeight = 0;
        }

        public abstract void Update(View view);
        public abstract void Draw(RenderTarget target, RenderStates states);

        //public abstract void Draw(GameLoop gameLoop);
    }
}
