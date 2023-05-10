using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class PausScreen : IUpdateAndDraw
    {
        readonly Texture2D pixel;
        readonly Texture2D texture;
        Rectangle textSquare;
        private readonly SpriteFont font;
        readonly List<string> menuOptions = new()
        {
            "Resume",
            "Keybinds",
            "Volyme",
            "Exit Game",
        };
        private readonly Game1 game;
        private readonly Camera camera;
        private int index = 0;
        private KeyboardState oldState = Keyboard.GetState();
        Point size = new(201, 300);

        public PausScreen(GraphicsDevice graphicsDevice, Camera camera, Game1 game)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.Black });
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            texture = game.Content.Load<Texture2D>("UI/PausBox_x3");
            textSquare = new(camera.Window.Center, size);
            this.camera = camera;
            this.game = game;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, camera.Window, null, Color.White * 0.3f, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
            
            spriteBatch.Draw(texture, textSquare, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.91f);

            for (int i = 0; i < menuOptions.Count; i++)
            {
                if (i == index)
                    spriteBatch.DrawString(font, menuOptions[i], textSquare.Center.ToVector2() + new Vector2(-55, i * 40 - 75), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.92f);
                else
                    spriteBatch.DrawString(font, menuOptions[i], textSquare.Center.ToVector2() + new Vector2(-60, i * 40 - 75), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.92f);
            }
        }

        public void Update(GameTime gameTime)
        {
            textSquare = new(camera.Window.Center - new Point(size.X / 2, size.Y / 2), size);
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(game.player.Keybinds.SelectText) && oldState.IsKeyUp(game.player.Keybinds.SelectText))
            {
                switch (index)
                {
                    case 0:
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

            else if (state.IsKeyDown(game.player.Keybinds.SelectText) && oldState.IsKeyUp(game.player.Keybinds.SelectText))
                index--;

            if (index > menuOptions.Count - 1)
                index = 0;
            if (index < 0)
                index = menuOptions.Count - 1;

            oldState = state;
        }

        internal void SetOldState(KeyboardState state)
        {
            index = 0;
            oldState = state;
        }
    }
}
