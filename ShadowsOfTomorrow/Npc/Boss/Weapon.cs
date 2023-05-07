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
        public Color[] TextureData
        {
            get
            {
                Color[] color = new Color[texture.Width * texture.Height];
                texture.GetData(color);
                return color;
            }
        }

        protected readonly Texture2D texture;
        protected Rectangle hitbox;
        protected readonly Game1 game;
        public bool hasHitSomeone = false;

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
            if (game.player.animationManager.CurrentCropTexture == null || !game.player.HitBox.Intersects(hitbox))
                return;
            game.player.animationManager.CurrentCropTexture.GetData(game.player.TextureData);
            texture.GetData(TextureData);

            if (HasIntersectingPixels(game.player.HitBox, game.player.TextureData, HitBox, TextureData))
            {
                hasHitSomeone = true;
                game.player.OnHit();
            }
        }

        public static bool HasIntersectingPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // GET BOUNDS
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // GET COLORS FROM CURRENT POINT
                    Color colorA = dataA[(x - rectangleA.Left) + (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) + (y - rectangleB.Top) * rectangleB.Width];

                    // IF BOTH NOT TRANSPARENT, INTERSECTION = true
                    if (colorA.A != 0 && colorB.A != 0)
                        return true;
                }
            }

            return false;
        }
    }
}
