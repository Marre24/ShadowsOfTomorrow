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
        public Vector2 Location { get => hitbox.Location.ToVector2(); set => hitbox.Location = value.ToPoint(); }
        public Rectangle HitBox => hitbox;
        public Point Size => texture.Bounds.Size;

        protected readonly Texture2D texture;
        protected Rectangle hitbox;
        protected readonly Game1 game;
        public bool hasHit = false;

        public Weapon(Game1 game, string path, Vector2 location)
        {
            texture = game.Content.Load<Texture2D>(path);
            hitbox = new(location.ToPoint(), texture.Bounds.Size);
            this.game = game;
            Location = location;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Location, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9f);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (hitbox.Intersects(game.player.HitBox))
            {
                hasHit = true;
                game.player.OnHit();
            }
        }
    }
}
