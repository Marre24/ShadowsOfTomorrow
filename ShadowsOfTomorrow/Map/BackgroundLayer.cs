using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class BackgroundLayer : IUpdateAndDraw
    {
        Rectangle Rec1 => new((int)position.X, (int)position.Y, texture.Width, texture.Height);
        Rectangle Rec2 => new((int)position2.X, (int)position2.Y, texture.Width, texture.Height);

        private Vector2 position;
        private Vector2 position2;
        private readonly Game1 game;
        private readonly Texture2D texture;
        private readonly float depth;
        private readonly float moveScale;
        private readonly float defaultMoveSpeed;

        //Skapar och hanterar ett specifikt lager så det blir en parralax effekt
        public BackgroundLayer(Game1 game, float depth, float moveScale, Vector2 position, string textureName, float defaultMoveSpeed = 0.0f)
        {
            texture = game.Content.Load<Texture2D>("Backgrounds/" + textureName);
            this.game = game;
            this.depth = depth;
            this.moveScale = moveScale;
            this.position = position;
            position2 = position;
            this.defaultMoveSpeed = defaultMoveSpeed;
        }

        public void Update(GameTime gameTime)
        {
            if (defaultMoveSpeed != 0)
            {
                position.X += defaultMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                position2.X += defaultMoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                CheckPosition(game.Player);

                return;
            }

            float speed = game.Player.playerMovement.HorizontalSpeed;

            position.X += (speed * moveScale) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position2.X += (speed * moveScale) * (float)gameTime.ElapsedGameTime.TotalSeconds;


            CheckPosition(game.Player);
        }

        //kollar så att bakgrunden är fylld av bilder
        private void CheckPosition(Player player)
        {
            if (!player.camera.Window.Intersects(Rec1) && !player.camera.Window.Intersects(Rec2))
            {
                position.X = player.camera.Window.Left;
                position2.X = Rec1.Right;
                return;
            }

            if (player.camera.Window.Intersects(Rec1) && player.camera.Window.Intersects(Rec2) && position != position2)
                return;

            if (player.camera.Window.Intersects(Rec1))
            {
                float distanceBetweenLeftAndPlayer1 = player.camera.Window.Left - Rec1.Left;
                if (distanceBetweenLeftAndPlayer1 <= 0)
                {
                    position2.X = Rec1.Left - texture.Width + 2;
                    return;
                }
                position2.X = Rec1.Right - 2;
                return;
            }
            float distanceBetweenLeftAndPlayer = player.camera.Window.Left - Rec2.Left;
            if (distanceBetweenLeftAndPlayer <= 0)
            {
                position.X = Rec2.Left - texture.Width + 2;
                return;
            }
            position.X = Rec2.Right - 2;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
            spriteBatch.Draw(texture, position2, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, depth);
        }
    }
}
