using _2DGame.Entities;
using _2DGame.Entities.Players;
using _2DGame.Layers;
using _2DGame.Utility;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2DGame
{
    public class Level
    {
        private bool isLoaded;
        private bool isInitialized;
        public LayerList Layers { get; set; }
        public string TrackFilename { get; private set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2i TileStartPosition { get; private set; }
        public GameEntityManager GameEntityManager { get; set; }

        public Level()
        {
            Layers = new LayerList();
            TrackFilename = string.Empty;
            Width = 0;
            Height = 0;
            TileStartPosition = new();
            GameEntityManager = new();
            isLoaded = false;
            isInitialized = false;
        }

        public void LoadData(string mapFilename, string entityDataFilename, string layerDataFilename)
        {
            if (!isLoaded)
            {
                Layers.Load(mapFilename, layerDataFilename);
                GameEntityManager.Load(entityDataFilename);
                isLoaded = true;
            }
            else
            {
                GameEntityManager.Reload();
            }
        }

        public void Initialize(string trackFilename)
        {
            if (!isInitialized)
            {
                TrackFilename = trackFilename;

                Layers.Initialize(TextureManager.TilesetTexture);
                Width = Layers[LayerList.PRIMARY_LAYER].Width;
                Height = Layers[LayerList.PRIMARY_LAYER].Height;

                isInitialized = true;
            }

            GameEntityManager.Initialize();
            SoundManager.SetCurrentTrack(TrackFilename);
            TileStartPosition = GameEntityManager.PlayerStartTileCoordinates;
        }

        public void Update(Player player, GameLoop gameLoop)
        {
            Layers.Update(player, gameLoop);

            if (player.IsSpawningProjectile)
            {
                GameEntityManager.GameEntities.Add(new PlayerProjectile(new Vector2f(player.Position.X + PlayerProjectile.PROJECTILE_X_OFFSET * (player.CurrentDirection == Animation.IAnimated.Direction.Right ? 1f : -1f), player.Position.Y + PlayerProjectile.PROJECTILE_Y_OFFSET), player.CurrentDirection));
                SoundManager.PlaySound("Shoot");
            }

            if (player.HasFinishedLevel)
            {
                gameLoop.CurrentState = GameLoop.GameState.GameOver;
            }

            GameEntityManager.Update(this, player, gameLoop);
        }
    }
}
