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
    public class DeadScreen : IUpdateAndDraw
    {
        private readonly Texture2D buttonDownTexture;
        private readonly Texture2D buttonUpTexture;
        private readonly SpriteFont font;
        private Rectangle buttonHitBox;
        private readonly Rectangle window;
        private readonly Game1 game;
        private readonly Player player;
        private bool buttonIsUp;


        public DeadScreen(Game1 game, Player player)
        {
            window = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            buttonDownTexture = game.Content.Load<Texture2D>("UI/ButtonDown");
            buttonUpTexture = game.Content.Load<Texture2D>("UI/ButtonUp");
            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");

            this.game = game;
            this.player = player;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (buttonIsUp)
                spriteBatch.Draw(buttonUpTexture, buttonHitBox, Color.White);
            else
                spriteBatch.Draw(buttonDownTexture, buttonHitBox, Color.White);
            spriteBatch.DrawString(font, "Restart", buttonHitBox.Location.ToVector2() + new Point(70, 20).ToVector2(), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            player.camera.Follow(window.Center);

            var mouseState = Mouse.GetState();
            if (!buttonHitBox.Contains(mouseState.Position))
            {
                buttonIsUp = true;
                buttonHitBox = new(window.Center - new Point(120, -200), new(46 * 6, 14 * 6));
                return;
            }
            buttonIsUp = false;
            buttonHitBox = new(window.Center - new Point(120, -206), new(46 * 6, 13 * 6));
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                game.mapManager.GoToSpawnPoint(player.LastSpawnPoint);
                player.Reset();
            }
        }
    }
}
