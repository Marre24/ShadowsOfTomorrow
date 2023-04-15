using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Screen = System.Windows.Forms.Screen;
using TiledSharp;
using System.Windows.Forms.Design;

namespace ShadowsOfTomorrow
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Player player;
        public MapManager mapManager;
        public WindowManager windowManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.Bounds.Height;
            _graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.Bounds.Width;
            _graphics.ApplyChanges();

            player = new(this);
            windowManager = new(this);
            mapManager = new(this);


            mapManager.Add(new(this, "LandingSite", mapManager));
            mapManager.Add(new(this, "LearnControllsMap", mapManager));
            mapManager.Add(new(this, "CrashSite", mapManager));
            mapManager.Add(new(this, "LearnMelee", mapManager));
            mapManager.Add(new(this, "RunFromBranches", mapManager));
            mapManager.Add(new(this, "PlantCity", mapManager));
            mapManager.Add(new(this, "BossRoom", mapManager));
            mapManager.GoToSpawnPoint("13");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (player.CurrentAction == Action.Ended)
            {
                windowManager.Update(gameTime);
                return;
            }

            if (player.CurrentAction == Action.Talking)
                windowManager.Update(gameTime);

            mapManager.Update(gameTime);
            player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: player.camera.Transform);

            if (player.CurrentAction == Action.Ended)
            {
                windowManager.Draw(_spriteBatch);
                _spriteBatch.End();
                return;
            }

            mapManager.Draw(_spriteBatch);
            player.Draw(_spriteBatch);
            if (player.CurrentAction == Action.Talking)
                windowManager.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}