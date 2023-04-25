using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class BranchWall : Weapon
    {
        private float speed = 2.0f;

        public BranchWall(Game1 game, string path, Vector2 location, Point size) : base(game, path, location)
        {
            Location = location - new Vector2(size.X, 0);
            hitbox = new(Location.ToPoint(), size);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, hitbox, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.91f);
        }

        public override void Update(GameTime gameTime)
        {
            if (hitbox.Intersects(game.player.HitBox))
                game.player.Die();

            if (Location.X >= 45 * 48)
                speed = 3;
            if (Location.X >= 80 * 48)
                speed = 2.4f;
            if (Location.X >= 150 * 48)
                speed = 3;
            if (Location.X >= 200 * 48)
                speed = 2.2f;

            Location += new Vector2(speed, 0);
        }

        internal void Move()
        {
            Location += new Vector2(3, 0);
        }
    }
}
