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
    public abstract class Layer : Drawable, IDisposable
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
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LayerTilemap.Dispose();
                TileIDs = null;
                XSpeed = 0;
                YSpeed = 0;
                AutoXSpeed = 0;
                AutoYSpeed = 0;
                Width = 0;
                Height = 0;
                TileWidth = 0;
                TileHeight = 0;
            }
        }
    }
}
