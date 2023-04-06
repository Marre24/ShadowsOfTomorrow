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
        SpriteFont font;

        public Boss(Game1 game) 
        {
            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, dialogue.GetMessage(), new(1000, 1000), Color.White);

        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
