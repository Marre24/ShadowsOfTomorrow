using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class LeafFalling : BaseClassPhase
    {
        readonly List<Leaf> leaves = new();
        readonly Random random = new();
        private readonly Game1 game;
        private readonly double leafSpawnInterval;
        private double time = 0;

        public LeafFalling(Game1 game, double leafSpawnInterval)
        {
            this.game = game;
            this.leafSpawnInterval = leafSpawnInterval;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var leaf in leaves)
                leaf.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var leaf in leaves)
                leaf.Update(gameTime);

            if (gameTime.TotalGameTime.TotalSeconds < time + leafSpawnInterval)
                return;

            time = gameTime.TotalGameTime.TotalSeconds;

            Vector2 location = new(random.Next(game.player.camera.Window.Left, game.player.camera.Window.Right), game.player.camera.Window.Top);
            Vector2 dir = location - game.player.Location.ToVector2();
            dir.Normalize();
            float x = -dir.X;
            if (x >= 0.6)
               x = 0.6f;
            if (x <= -0.6)
                x = -0.6f;
            leaves.Add(new(game, "Sprites/Bosses/Leaf", location, new(x * 5, 5)));
        }
    }
}
