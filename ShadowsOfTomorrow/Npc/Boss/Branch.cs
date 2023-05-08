using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    internal class Branch : Weapon
    {
        private double time;
        private readonly bool vertical;
        private bool returning;

        public Branch(Game1 game, string path, Vector2 location, bool vertical) : base(game, path, location)
        {

            if (vertical)
                hitbox = new(location.ToPoint() - new Point(48 * 12, 0), new(48 * 10, 48));
            else
                hitbox = new(location.ToPoint(), new(48, 48 * 12));
            this.vertical = vertical;
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, hitbox, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.92f);

            //base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (vertical)
            {
                if (hitbox.Right < 5 * 48)
                {
                    Location += new Vector2(3, 0);
                    time = gameTime.TotalGameTime.TotalSeconds;
                }

                if (hitbox.Right >= 5 * 48 && time + 1 < gameTime.TotalGameTime.TotalSeconds)
                    Location += new Vector2(30, 0);
                return;
            }

            if (hitbox.Top > 24 * 48 && !returning)
            {
                Location -= new Vector2(0, 10);
                time = gameTime.TotalGameTime.TotalSeconds;
            }

            if (!returning && hitbox.Top > 17 * 48 && hitbox.Top <= 24 * 48 && time + 1 < gameTime.TotalGameTime.TotalSeconds)
                Location -= new Vector2(0, 15);

            if (hitbox.Top <= 17 * 48)
                returning = true;

            if (returning)
                Location += new Vector2(0, 10);
        }
    }
}
