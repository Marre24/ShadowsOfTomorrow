using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Screen = System.Windows.Forms.Screen;
using TiledSharp;
using System.Windows.Forms.Design;

namespace ShadowsOfTomorrow
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Dom här instanserna tar är de fyra klasser som tar hand om vad som ska göras och när

        //Spelaren fokuserar på allt spelare relaterade grejer
        public Player Player { get; private set; }

        //Mapmanagern uppdaterar och hanterar allt som läses in i .tmx filerna
        public MapManager MapManager { get; private set; }

        //windowmanagern uppdaterar det som inte är mapp relaterat men som fyller upp skärmen
        public WindowManager WindowManager { get; private set; }

        //musicmanagern spelar upp ljudeffekter samt 
        public MusicManager MusicManager { get; private set; }

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
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            Player = new(this);
            MapManager = new(this);
            WindowManager = new(this);
            MusicManager = new(this);


            MapManager.AddMaps();
            MapManager.GoToSpawnPoint(1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            //updaterar det som behövs

            if (Player.CurrentAction == Action.Ended || Player.CurrentAction == Action.Dead || Player.CurrentAction == Action.InMainMenu || Player.CurrentAction == Action.ChangingKeybinds || Player.CurrentAction == Action.Paused || Player.CurrentAction == Action.ChangingVolyme)
            {
                WindowManager.Update(gameTime);
                return;
            }

            if (Player.CurrentAction == Action.Talking)
                WindowManager.Update(gameTime);

            MapManager.Update(gameTime);
            Player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Ritar det som behövs

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: Player.camera.Transform, sortMode: SpriteSortMode.FrontToBack);

            if (Player.CurrentAction == Action.Ended || Player.CurrentAction == Action.Dead || Player.CurrentAction == Action.InMainMenu || Player.CurrentAction == Action.ChangingKeybinds || Player.CurrentAction == Action.Paused || Player.CurrentAction == Action.ChangingVolyme)
            {
                if ((Player.CurrentAction == Action.Paused || Player.CurrentAction == Action.ChangingKeybinds || Player.CurrentAction == Action.ChangingVolyme) && Player.LastSpawnPoint != 0)
                {
                    WindowManager.Draw(_spriteBatch);
                    MapManager.Draw(_spriteBatch);
                    Player.Draw(_spriteBatch);
                    _spriteBatch.End();
                    return;
                }

                GraphicsDevice.Clear(Color.Black);
                WindowManager.Draw(_spriteBatch);
                _spriteBatch.End();
                return;
            }

            MapManager.Draw(_spriteBatch);
            
            Player.Draw(_spriteBatch);

            if (Player.CurrentAction == Action.Talking)
                WindowManager.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}