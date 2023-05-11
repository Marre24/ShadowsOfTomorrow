using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Screen = System.Windows.Forms.Screen;

namespace ShadowsOfTomorrow
{
    public class EndingScreen : IUpdateAndDraw
    {
        private readonly Texture2D buttonDownTexture;
        private readonly Texture2D buttonUpTexture;
        private readonly SpriteFont font;
        private readonly string endingText;
        private Rectangle buttonHitBox;
        private readonly Rectangle window;
        private readonly Game1 game;
        private readonly Player player;
        private bool buttonIsUp;


        public EndingScreen(Game1 game, Player player, Boss boss)
        {
            window = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            if (boss.wasKilled)
                endingText = "You have killed the boss";
            else
                endingText = "You persuaded the boss to coexist";


            buttonDownTexture = game.Content.Load<Texture2D>("UI/ButtonDown");
            buttonUpTexture = game.Content.Load<Texture2D>("UI/ButtonUp");
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            
            this.game = game;
            this.player = player;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, endingText, window.Center.ToVector2() - new Vector2(200, 0), Color.White);

            if (buttonIsUp)
                spriteBatch.Draw(buttonUpTexture, buttonHitBox, Color.White);
            else
                spriteBatch.Draw(buttonDownTexture, buttonHitBox, Color.White);
            spriteBatch.DrawString(font, "Exit", buttonHitBox.Location.ToVector2() + new Point(70, 20).ToVector2(), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);

        }

        public void Update(GameTime gameTime)
        {
            player.camera.Follow(window.Center);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                game.Exit();

            var mouseState = Mouse.GetState();
            if (!buttonHitBox.Contains(mouseState.Position))
            {
                buttonIsUp = true;
                buttonHitBox = new (window.Center - new Point(120, -200), new(46 * 6, 14 * 6));
                return;
            }
            buttonIsUp = false;
            buttonHitBox = new(window.Center - new Point(120, -206), new(46 * 6, 13 * 6));
            if (mouseState.LeftButton == ButtonState.Pressed)
                game.Exit();
        }
    }
}
