using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShadowsOfTomorrow
{
    public class Boss : Speech, IUpdateAndDraw
    {
        readonly SpriteFont font;
        readonly Texture2D texture;

        public Boss(Game1 game) 
        {
            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");
            texture = game.Content.Load<Texture2D>("Sprites/Bosses/BoneTurtle");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, dialogue.GetMessage(), new(1000, 1000), Color.White);
            spriteBatch.Draw(texture, new Vector2(1000, 700), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
