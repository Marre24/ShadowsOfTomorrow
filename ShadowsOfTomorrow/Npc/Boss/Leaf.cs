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
        private readonly List<Leaf> leaves;

        public Leaf(Game1 game, string path, Vector2 location, Vector2 constantSpeed, List<Leaf> leaves) : base(game, path, location)
        {
            this.constantSpeed = constantSpeed;
            this.leaves = leaves;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (hitbox.Intersects(game.player.HitBox))
                leaves.Remove(this);

            Location += constantSpeed;
        }
    }
}
