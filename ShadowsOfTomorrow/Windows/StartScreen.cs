using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screen = System.Windows.Forms.Screen;
using Microsoft.Xna.Framework.Input;

namespace ShadowsOfTomorrow
{
    public class StartScreen
    {
        private Rectangle window;
        private readonly SpriteFont font;
        private readonly SpriteFont titleFont;
        readonly List<string> menuOptions = new()
        {
            "Start Game",
            "Key binds",
            "Volume",
            "Exit",
        };
        private readonly Game1 game;
        readonly StartCutScene startCutScene;
        private int index = 0;
        private KeyboardState oldState = Keyboard.GetState();

        public StartScreen(Game1 game)
        {
            Point size = new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            window = new(Point.Zero, size);
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            titleFont = game.Content.Load<SpriteFont>("Fonts/TitleFont");
            this.game = game;
            startCutScene = new(game, game.Player.camera, game.Player);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            game.Player.camera.Follow(Point.Zero);

            if (!startCutScene.HaveEnded)
            {
                startCutScene.Update(gameTime);
                return;
            }

            if (state.IsKeyDown(game.Player.Keybinds.SelectText) && oldState.IsKeyUp(game.Player.Keybinds.SelectText))
            {
                switch (index)
                {
                    case 0:
                        if (game.Player.LastSpawnPoint == 0)
                            game.MapManager.GoToSpawnPoint(1);
                        else
                            game.MapManager.GoToSpawnPoint(game.Player.LastSpawnPoint);
                        game.Player.CurrentAction = Action.Standing;
                        break;
                    case 1:
                        game.Player.CurrentAction = Action.ChangingKeybinds;
                        break;
                    case 2:
                        game.Player.CurrentAction = Action.ChangingVolyme;
                        break;
                    case 3:
                        game.Exit();
                        break;
                    default:
                        break;
                }

                return;
            }

            if (state.IsKeyDown(game.Player.Keybinds.DialogueDown) && oldState.IsKeyUp(game.Player.Keybinds.DialogueDown))
                index++;

            else if (state.IsKeyDown(game.Player.Keybinds.DialogueUp) && oldState.IsKeyUp(game.Player.Keybinds.DialogueUp))
                index--;

            if (index > menuOptions.Count - 1)
                index = 0;
            if (index < 0)
                index = menuOptions.Count - 1;

            oldState = state;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!startCutScene.HaveEnded)
            {
                startCutScene.Draw(spriteBatch);
                return;
            }

            spriteBatch.DrawString(titleFont, "Shadows Of Tomorrow", game.Player.camera.Window.Center.ToVector2() - new Vector2(titleFont.MeasureString("Shadows Of Tomorrow").X / 2, 250), Color.White);
            
            for (int i = 0; i < menuOptions.Count; i++)
            {
                if (i == index)
                    spriteBatch.DrawString(font, menuOptions[i], window.Location.ToVector2() + new Vector2(-98, i * 40), Color.Red);
                else
                    spriteBatch.DrawString(font, menuOptions[i], window.Location.ToVector2() + new Vector2(-100, i * 40), Color.White);
            }
        }

        internal void SetOldState(KeyboardState state)
        {
            index = 0;
            oldState = state;
        }
    }
}
