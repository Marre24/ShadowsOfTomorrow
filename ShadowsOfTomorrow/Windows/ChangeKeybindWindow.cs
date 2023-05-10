using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Screen = System.Windows.Forms.Screen;

namespace ShadowsOfTomorrow
{
    public class ChangeKeybindWindow
    {
        private Rectangle window;
        private readonly SpriteFont font;
        private readonly Texture2D texture;
        private readonly Game1 game;
        private readonly Keybinds keybinds;
        private readonly StartScreen startScreen;
        private readonly PausScreen pausScreen;
        private int index = 0;
        private KeyboardState oldState = Keyboard.GetState();
        private bool checkingForInput = false;
        Point size = new(400, 600);

        public ChangeKeybindWindow(Game1 game, Keybinds keybinds, StartScreen startScreen, PausScreen pausScreen)
        {
            window = new(game.player.camera.Window.Center + new Point(- size.X / 2, -100 - size.Y / 2), size);
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            texture = game.Content.Load<Texture2D>("UI/DialogueBox_x3");
            this.game = game;
            this.keybinds = keybinds;
            this.startScreen = startScreen;
            this.pausScreen = pausScreen;
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();
            window = new(game.player.camera.Window.Center + new Point(-size.X / 2, -100 - size.Y / 2), size);

            if (game.player.LastSpawnPoint == 0)
                game.player.camera.Follow(Point.Zero);

            if (state.IsKeyDown(keybinds.SelectText) && oldState.IsKeyUp(keybinds.SelectText) && index == keybinds.AllKeys.Count)
            {
                if (game.player.LastSpawnPoint == 0)
                    game.player.CurrentAction = Action.InMainMenu;
                else
                    game.player.CurrentAction = Action.Paused;

                startScreen.SetOldState(state);
                pausScreen.SetOldState(state);

            }

            if (state.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter) || checkingForInput)
            {
                List<Keys> kys = state.GetPressedKeys().ToList();
                kys.Remove(Keys.Enter);
                kys.Remove(Keys.Escape);
                if (kys.Count <= 0)
                {
                    checkingForInput = true;
                    return;
                }
                Keys keyboardKey = kys.First();
                string dicKey = keybinds.AllKeys.ElementAt(index).Key;

                if (keybinds.DialogueKeys.ContainsKey(dicKey))
                {
                    if (!keybinds.DialogueKeys.ContainsValue(keyboardKey))
                    {
                        if (dicKey == "DialogueDown")
                            keybinds.DialogueDown = keyboardKey;
                        if (dicKey == "DialogueUp")
                            keybinds.DialogueUp = keyboardKey;
                        if (dicKey == "SelectText")
                            keybinds.SelectText = keyboardKey;

                        checkingForInput = false;
                    }
                    return;
                }

                if (!keybinds.MovementKeys.ContainsValue(keyboardKey))
                {
                    if (dicKey == "RightKey")
                        keybinds.RightKey = keyboardKey;
                    if (dicKey == "LeftKey")
                        keybinds.LeftKey = keyboardKey;
                    if (dicKey == "CrouchKey")
                        keybinds.CrouchKey = keyboardKey;
                    if (dicKey == "JumpKey")
                        keybinds.JumpKey = keyboardKey;
                    if (dicKey == "TalkKey")
                        keybinds.TalkKey = keyboardKey;
                    if (dicKey == "AttackKey")
                        keybinds.AttackKey = keyboardKey;
                    if (dicKey == "AccelerateKey")
                        keybinds.AccelerateKey = keyboardKey;
                    checkingForInput = false;
                }

                return;
            }

            if (state.IsKeyDown(keybinds.DialogueDown) && oldState.IsKeyUp(keybinds.DialogueDown))
                index++;

            else if (state.IsKeyDown(keybinds.DialogueUp) && oldState.IsKeyUp(keybinds.DialogueUp))
                index--;

            if (index > keybinds.AllKeys.Count)
                index = 0;
            if (index < 0)
                index = keybinds.AllKeys.Count;

            oldState = state;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, window, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
            if (index == keybinds.AllKeys.Count)
                spriteBatch.DrawString(font, "Back", window.Location.ToVector2() + new Vector2(window.Size.X - 95, window.Size.Y - 80), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            else
                spriteBatch.DrawString(font, "Back", window.Location.ToVector2() + new Vector2(window.Size.X - 100, window.Size.Y - 80), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);

            for (int i = 0; i < keybinds.AllKeys.Count; i++)
            {
                spriteBatch.DrawString(font, keybinds.AllKeys.ElementAt(i).Key, window.Location.ToVector2() + new Vector2(30, i * 40 + 50), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
                if (i == index)
                    spriteBatch.DrawString(font, keybinds.AllKeys.ElementAt(i).Value.ToString(), window.Location.ToVector2() + new Vector2(245, i * 40 + 50), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
                else
                    spriteBatch.DrawString(font, keybinds.AllKeys.ElementAt(i).Value.ToString(), window.Location.ToVector2() + new Vector2(240, i * 40 + 50), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            }
        }

    }
}
