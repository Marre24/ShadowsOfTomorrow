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
        readonly List<string> menuOptions = new()
        {
            "Start Game",
            "Keybinds",
            "Volyme",
            "Exit",
        };
        private readonly Game1 game;
        private int index = 0;
        private KeyboardState oldState = Keyboard.GetState();

        public StartScreen(Game1 game)
        {
            Point size = new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            window = new(Point.Zero, size);
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            this.game = game;
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();

            game.player.camera.Follow(Point.Zero);

            if (state.IsKeyDown(game.player.Keybinds.SelectText) && oldState.IsKeyUp(game.player.Keybinds.SelectText))
            {
                switch (index)
                {
                    case 0:
                        game.mapManager.GoToSpawnPoint(1);
                        game.player.CurrentAction = Action.Standing;
                        break;
                    case 1:
                        game.player.CurrentAction = Action.ChangingKeybinds;
                        break;
                    case 2:
                        break;
                    case 3:
                        game.Exit();
                        break;
                    default:
                        break;
                }

                return;
            }

            if (state.IsKeyDown(game.player.Keybinds.DialogueDown) && oldState.IsKeyUp(game.player.Keybinds.DialogueDown))
                index++;

            else if (state.IsKeyDown(game.player.Keybinds.DialogueUp) && oldState.IsKeyUp(game.player.Keybinds.DialogueUp))
                index--;

            if (index > menuOptions.Count - 1)
                index = 0;
            if (index < 0)
                index = menuOptions.Count - 1;

            oldState = state;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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
