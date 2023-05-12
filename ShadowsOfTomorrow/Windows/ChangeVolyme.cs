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
    public class ChangeVolyme : IUpdateAndDraw
    {
        private Rectangle window;
        private Vector2 upperBarLocation;
        private Vector2 lowerBarLocation;
        private Vector2 upperLeafLocation;
        private Vector2 lowerLeafLocation;
        private readonly SpriteFont font;
        private readonly Texture2D texture;
        private readonly Texture2D barTexture;
        private readonly Texture2D leaf;
        private readonly Game1 game;
        private readonly StartScreen startScreen;
        private readonly PausScreen pausScreen;
        private KeyboardState oldState;
        Point size = new(420, 600);
        int index = 0;

        public ChangeVolyme(Game1 game, StartScreen startScreen, PausScreen pausScreen)
        {
            window = new(game.Player.camera.Window.Center + new Point(-size.X / 2, -100 - size.Y / 2), size);
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            texture = game.Content.Load<Texture2D>("UI/ChangeBox_x3");
            barTexture = game.Content.Load<Texture2D>("UI/Bar_x3");
            leaf = game.Content.Load<Texture2D>("Sprites/Bosses/Leaf_x3");
            this.game = game;
            this.startScreen = startScreen;
            this.pausScreen = pausScreen;
            upperBarLocation = window.Center.ToVector2() + new Vector2(-200, -100);
            lowerBarLocation = window.Center.ToVector2() + new Vector2(-200, 100);
        }

        public void Update(GameTime gameTime)
        {
            upperBarLocation = window.Center.ToVector2() + new Vector2(-200, -100);
            lowerBarLocation = window.Center.ToVector2() + new Vector2(-200, 100);

            upperLeafLocation = new(upperBarLocation.X + game.MusicManager.SoundEffectsVolume * 300, upperBarLocation.Y);
            lowerLeafLocation = new(lowerBarLocation.X + MusicManager.MusicVolume * 300, lowerBarLocation.Y);

            KeyboardState state = Keyboard.GetState();
            window = new(game.Player.camera.Window.Center + new Point(-size.X / 2, -100 - size.Y / 2), size);

            if (game.Player.LastSpawnPoint == 0)
                game.Player.camera.Follow(Point.Zero);

            const float increment = 0.05f;

            if (state.IsKeyDown(game.Player.Keybinds.RightKey) && oldState.IsKeyUp(game.Player.Keybinds.RightKey) && index == 0 && game.MusicManager.SoundEffectsVolume <= 1 - increment)
                game.MusicManager.SoundEffectsVolume += increment;
            if (state.IsKeyDown(game.Player.Keybinds.LeftKey) && oldState.IsKeyUp(game.Player.Keybinds.LeftKey) && index == 0 && game.MusicManager.SoundEffectsVolume >= increment)
                game.MusicManager.SoundEffectsVolume -= increment;
            if (state.IsKeyDown(game.Player.Keybinds.RightKey) && oldState.IsKeyUp(game.Player.Keybinds.RightKey) && index == 1 && MusicManager.MusicVolume <= 1 - increment)
                MusicManager.MusicVolume += increment;
            if (state.IsKeyDown(game.Player.Keybinds.LeftKey) && oldState.IsKeyUp(game.Player.Keybinds.LeftKey) && index == 1 && MusicManager.MusicVolume > increment)
                MusicManager.MusicVolume -= increment;


            if (state.IsKeyDown(game.Player.Keybinds.SelectText) && oldState.IsKeyUp(game.Player.Keybinds.SelectText) && index == 2)
            {
                if (game.Player.LastSpawnPoint == 0)
                    game.Player.CurrentAction = Action.InMainMenu;
                else
                    game.Player.CurrentAction = Action.Paused;

                startScreen.SetOldState(state);
                pausScreen.SetOldState(state);
            }

            if (state.IsKeyDown(game.Player.Keybinds.DialogueDown) && oldState.IsKeyUp(game.Player.Keybinds.DialogueDown))
                index++;

            else if (state.IsKeyDown(game.Player.Keybinds.DialogueUp) && oldState.IsKeyUp(game.Player.Keybinds.DialogueUp))
                index--;

            if (index > 2)
                index = 0;
            if (index < 0)
                index = 2;

            oldState = state;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, window, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.92f);
            spriteBatch.Draw(barTexture, upperBarLocation, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.93f);
            spriteBatch.Draw(barTexture, lowerBarLocation, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.93f);
            spriteBatch.Draw(leaf, upperLeafLocation, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.94f);
            spriteBatch.Draw(leaf, lowerLeafLocation, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.94f);

            if (index == 0)
                spriteBatch.DrawString(font, "Sound effect volyme", upperBarLocation + new Vector2(5, -50), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.93f);
            else
                spriteBatch.DrawString(font, "Sound effect volyme", upperBarLocation + new Vector2(0, -50), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.93f);

            if (index == 1)
                spriteBatch.DrawString(font, "Music volyme", lowerBarLocation + new Vector2(5, -50), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.93f);
            else
                spriteBatch.DrawString(font, "Music volyme", lowerBarLocation + new Vector2(0, -50), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.93f);

            if (index == 2)
                spriteBatch.DrawString(font, "Back", window.Location.ToVector2() + new Vector2(window.Size.X - 90, window.Size.Y - 80), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.93f);
            else
                spriteBatch.DrawString(font, "Back", window.Location.ToVector2() + new Vector2(window.Size.X - 95, window.Size.Y - 80), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.93f);
        }
    }
}
