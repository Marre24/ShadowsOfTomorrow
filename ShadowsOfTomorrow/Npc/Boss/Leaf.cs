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
        public Rectangle Hitbox => hitbox;

        public Leaf(Game1 game, string path, Vector2 location, Vector2 constantSpeed) : base(game, path, location)
        {
            this.constantSpeed = constantSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hasHit)
                return;
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (hasHit)
                return;

            base.Update(gameTime);
            Location += constantSpeed;
        }
    }
}
