using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Leaf : Weapon
    {
        private readonly Vector2 constantSpeed;

        public Leaf(Game1 game, string path, Point size, Point location, Vector2 constantSpeed) : base(game, path, size, location)
        {
            this.constantSpeed = constantSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Location += constantSpeed.ToPoint();
        }
    }
}
