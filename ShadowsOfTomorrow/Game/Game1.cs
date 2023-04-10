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


            mapManager.Add(new(this, "StartMap", mapManager));
            mapManager.Add(new(this, "SecondMap", mapManager));
            mapManager.GoToSpawnPoint("1");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
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

            

            mapManager.Draw(_spriteBatch);
            player.Draw(_spriteBatch);
            if (player.CurrentAction == Action.Talking)
                windowManager.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}