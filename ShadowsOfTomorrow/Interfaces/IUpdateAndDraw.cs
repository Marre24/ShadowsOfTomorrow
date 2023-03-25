using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public interface IUpdateAndDraw
    {
        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch);

    }
}
