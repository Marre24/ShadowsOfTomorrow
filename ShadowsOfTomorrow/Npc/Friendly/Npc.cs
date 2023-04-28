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
        public Rectangle HitBox { 
            get 
            {
                if (big)
                    return new(position.ToPoint(), new(bigTexture.Width, bigTexture.Height));
                else
                    return new(position.ToPoint(), new(smallTexture.Width, smallTexture.Height));
            } 
        }

        private readonly Texture2D bigTexture;
        private readonly Texture2D smallTexture;
        private readonly Game1 game;
        private readonly Vector2 position;
        private readonly Player player;
        private readonly bool big;

        public Npc(Game1 game, Vector2 position, Player player, string name, bool big) : base(name, false)
        {
            bigTexture = game.Content.Load<Texture2D>("Sprites/Npc/PlantPerson_x3");
            smallTexture = game.Content.Load<Texture2D>("Sprites/Npc/PlantBaby_x3");
            this.game = game;
            this.position = position;
            this.player = player;
            this.big = big;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (big)
                spriteBatch.Draw(bigTexture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9f);
            else
                spriteBatch.Draw(smallTexture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9f);
        }

        public void Update(GameTime gameTime)
        {
            if (player.HitBox.Intersects(HitBox) && Keyboard.GetState().IsKeyDown(Keys.K)) 
            {
                game.windowManager.SetDialogue(dialogue, null);
                game.player.CurrentAction = Action.Talking;
            }
        }

    }
}
