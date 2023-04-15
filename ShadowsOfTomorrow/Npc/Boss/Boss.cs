using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ShadowsOfTomorrow
{
    public class Boss : Speech, IUpdateAndDraw
    {
        public Rectangle HitBox { get => new(location, new(texture.Width, texture.Height)); }

        private readonly Game1 game;
        private Point location;
        readonly Texture2D texture;

        private int health = 10;
        internal bool wasKilled;

        public Boss(Game1 game, string name, Point location) : base(name)
        {
            texture = game.Content.Load<Texture2D>("Sprites/Bosses/TreevorLeaf_x3");
            this.game = game;
            this.location = location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location.ToVector2(), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            if (health <= 0)
            {
                wasKilled = true;
                return;
            }


            if (game.player.HitBox.Intersects(HitBox) && Keyboard.GetState().IsKeyDown(Keys.K))
            {
                game.windowManager.SetDialogue(Dialogue);
                game.player.CurrentAction = Action.Talking;
            }
        }

        internal void OnHit()
        {
            health--;
        }
    }
}
