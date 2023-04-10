using Microsoft.Xna.Framework;
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

        private Point location;

        readonly Texture2D texture;

        public Boss(Game1 game, string name, Point location) : base(name)
        {
            texture = game.Content.Load<Texture2D>("Sprites/Bosses/BoneTurtle");
            this.location = location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location.ToVector2(), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            

        }

        
    }
}
