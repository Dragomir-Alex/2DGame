using _2DGame.ExternalLibraries;
using _2DGame.LayerData;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Layers
{
    public class DetailLayer : Layer, Drawable
    {
        public Texture? LayerTexture { get; set; }
        public Sprite? LayerSprite { get; set; }
        public float XOffset { get; set; }
        public float YOffset { get; set; }
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        private float XCameraOffset { get; set; }
        private float YCameraOffset { get; set; }

        public DetailLayer() : base()
        {
            TileIDs = null;
            LayerTilemap = null;
            XOffset = 0f;
            YOffset = 0f;
            RepeatX = false;
            RepeatY = false;
            XCameraOffset = 0f;
            YCameraOffset = 0f;
        }

        public DetailLayer(string tilesetFilename, TileData tileIDs) : base()
        {
            Initialize(tilesetFilename, tileIDs);
        }

        public void Initialize(string tilesetFilename, TileData tileIDs)
        {
            TileIDs = tileIDs;

            TileWidth = tileIDs.Width();
            TileHeight = tileIDs.Height();
            Width = Tilemap.TILE_SIZE * tileIDs.Width();
            Height = Tilemap.TILE_SIZE * tileIDs.Height();

            LayerTilemap = new Tilemap(tilesetFilename);
            LayerTilemap.Load(tileIDs, false);

            RenderTexture renderTexture = new RenderTexture(LayerTilemap.PixelWidth, LayerTilemap.PixelHeight);
            renderTexture.Clear(new Color(0, 0, 0, 0));
            renderTexture.Draw(LayerTilemap.Vertices, new RenderStates(LayerTilemap.Tileset));
            renderTexture.Display();

            LayerTexture = new Texture(renderTexture.Texture);
        }

        public void InitializeSprite()
        {
            if (LayerTexture != null)
            {
                LayerTexture.Smooth = true;
                LayerTexture.Repeated = true;

                LayerSprite = new Sprite(LayerTexture);

                int TextureRectWidth, TextureRectHeight;

                if (RepeatX)
                {
                    TextureRectWidth = 3 * (int)LayerTexture.Size.X * (int)Math.Ceiling((double)Game.DEFAULT_WINDOW_WIDTH / LayerTexture.Size.X);
                }
                else
                {
                    TextureRectWidth = (int)LayerTexture.Size.X;
                }

                if (RepeatY)
                {
                    TextureRectHeight = 3 * (int)LayerTexture.Size.Y * (int)Math.Ceiling((double)Game.DEFAULT_WINDOW_HEIGHT / LayerTexture.Size.Y);
                }
                else
                {
                    TextureRectHeight = (int)LayerTexture.Size.Y;
                }

                LayerSprite.TextureRect = new IntRect(0, 0, TextureRectWidth, TextureRectHeight);
                LayerSprite.Position = new Vector2f(XOffset - TextureRectWidth / 3, YOffset - TextureRectHeight / 3);
            }
        }

        public override void Update(View camera)
        {
            if (LayerSprite != null)
            {
                UtilityFunctions.Move(LayerSprite, XCameraOffset, YCameraOffset);
                Vector2f pos = LayerSprite.Position;

                if (RepeatX && (pos.X >= 0 || pos.X <= -(LayerSprite.TextureRect.Width / 3) * 2))
                    LayerSprite.Position = new Vector2f(-(LayerSprite.TextureRect.Width / 3), pos.Y);
                if (RepeatY && (pos.Y >= 0 || pos.Y <= -(LayerSprite.TextureRect.Height / 3) * 2))
                    LayerSprite.Position = new Vector2f(pos.X, -(LayerSprite.TextureRect.Width / 3));

                UtilityFunctions.Move(LayerSprite, -XCameraOffset, -YCameraOffset);

                XCameraOffset = (int)(-XSpeed * camera.Center.X / (Game.DEFAULT_WINDOW_WIDTH / 2));
                YCameraOffset = (int)(-YSpeed * camera.Center.Y / (Game.DEFAULT_WINDOW_HEIGHT / 2));

                UtilityFunctions.Move(LayerSprite, AutoXSpeed, AutoYSpeed);
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (LayerSprite != null)
            {
                UtilityFunctions.Move(LayerSprite, XCameraOffset, YCameraOffset);
                target.Draw(LayerSprite);
                UtilityFunctions.Move(LayerSprite, -XCameraOffset, -YCameraOffset);
            }
        }
    }
}
