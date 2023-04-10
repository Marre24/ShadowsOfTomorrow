using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Npc : Speech, IUpdateAndDraw
    {
        public Rectangle HitBox { get => new(position.ToPoint(), new(texture.Width, texture.Height)); }

        private readonly Texture2D texture;
        private readonly Vector2 position;
        private readonly Player player;

        public Npc(Vector2 position, Player player, string name) : base(name)
        {
            this.position = position;
            this.player = player;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            if (player.HitBox.Intersects(HitBox) && Keyboard.GetState().IsKeyDown(Keys.K)) 
            {
            }
        }

    }
}
