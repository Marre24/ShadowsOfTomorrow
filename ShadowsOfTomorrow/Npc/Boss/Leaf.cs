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
        private Vector2 constantSpeed;
        private readonly Boss boss;

        public Rectangle Hitbox => hitbox;

        public Leaf(Game1 game, string path, Vector2 location, Vector2 constantSpeed, Boss boss) : base(game, path, location)
        {
            this.constantSpeed = constantSpeed;
            this.boss = boss;
        }

        public void HitLeaf(Vector2 newSpeed)
        {
            constantSpeed = newSpeed;   
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (game.Player.playerAttacking.Hitbox.Intersects(hitbox) && game.Player.CurrentAction == Action.Attacking)
            {
                if (game.Player.Facing == Facing.Right)
                    HitLeaf(new(5, 0));
                else
                    HitLeaf(new(-5, 0));
            }

            if (boss.HitBox.Intersects(HitBox))
                if (HasIntersectingPixels(hitbox, TextureData, boss.HitBox, boss.TextureData))
                {
                    hasHitSomeone = true;
                    boss.GetHitByLeaf();
                }

            base.Update(gameTime);
            Location += constantSpeed;
        }
    }
}
