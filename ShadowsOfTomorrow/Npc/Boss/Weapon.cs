using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Weapon : IUpdateAndDraw
    {
        public Point Location { get => hitbox.Location; set => hitbox.Location = value; }
        protected readonly Texture2D texture;
        protected Rectangle hitbox;
        protected readonly Game1 game;

        public Weapon(Game1 game, string path, Point size, Point location)
        {
            hitbox = new(location, size);
            texture = game.Content.Load<Texture2D>(path);
            this.game = game;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, hitbox.Location.ToVector2(), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9f);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (hitbox.Intersects(game.player.HitBox))
                game.player.OnHit();
        }
    }
}
