using _2DGame.LayerData;
using _2DGame.Layers;
using _2DGame.Utility;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.LevelData
{
    public class Mask
    {
        public static class MaskValues
        {
            public const bool SOLID = true;
            public const bool NON_SOLID = false;
        }

        public Sprite MaskSprite;
        public Image MaskImage { get; private set; }

        public Mask(string maskFilename)
        {
            MaskImage = new Image(TextureManager.TILESETS_PATH + maskFilename);
        }

        public void Load(SpriteLayer spriteLayer)
        {
            uint levelWidth = (uint)spriteLayer.TileIDs.GetLength(1);
            uint levelHeight = (uint)spriteLayer.TileIDs.GetLength(0);

            uint maskWidth = (uint)spriteLayer.TileIDs.GetLength(1) * Tilemap.TILE_SIZE;
            uint maskHeight = (uint)spriteLayer.TileIDs.GetLength(0) * Tilemap.TILE_SIZE;

            Color[,] maskPixels = new Color[maskWidth, maskHeight];

            for (uint i = 0; i < levelWidth; ++i)
            {
                for (uint j = 0; j < levelHeight; ++j)
                {
                    uint tileNumber = spriteLayer.TileIDs[j, i];

                    uint maskX = tileNumber % (MaskImage.Size.X / Tilemap.TILE_SIZE);
                    uint maskY = tileNumber / (MaskImage.Size.X / Tilemap.TILE_SIZE);

                    for (uint k = i * Tilemap.TILE_SIZE; k < i * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE; ++k)
                    {
                        for (uint l = j * Tilemap.TILE_SIZE; l < j * Tilemap.TILE_SIZE + Tilemap.TILE_SIZE; ++l)
                        {
                            uint xPixel = maskX * Tilemap.TILE_SIZE + k - i * Tilemap.TILE_SIZE;
                            uint yPixel = maskY * Tilemap.TILE_SIZE + l - j * Tilemap.TILE_SIZE;

                            if (MaskImage.GetPixel(xPixel, yPixel).A == 255)
                                maskPixels[k, l] = Color.Black;
                        }
                    }
                }
            }

            Image maskFullImage = new Image(maskPixels);
            Texture maskTexture = new Texture(maskFullImage);
            MaskSprite = new Sprite(maskTexture);
        }
    }
}
