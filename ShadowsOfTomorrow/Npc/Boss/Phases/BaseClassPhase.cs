using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public abstract class BaseClassPhase : IUpdateAndDraw
    {
        public abstract void Draw(SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);

    }
}
