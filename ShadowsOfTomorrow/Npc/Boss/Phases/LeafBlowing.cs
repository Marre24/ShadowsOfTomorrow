using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadowsOfTomorrow
{
    public class LeafBlowing : BaseClassPhase
    {
        readonly List<Leaf> leaves = new();
        readonly Random random = new();
        private readonly Game1 game;
        private readonly double leafSpawnInterval;
        private readonly Player player;
        private double time = 0;

        public LeafBlowing(Game1 game, double leafSpawnInterval, Player player)
        {
            this.leafSpawnInterval = leafSpawnInterval;
            this.player = player;
            this.game = game;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var leaf in leaves)
                leaf.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var leaf in leaves)
                if (!leaf.hasHit)
                    leaf.Update(gameTime);

            player.Location += new Point(-1, 0);
            
            if (gameTime.TotalGameTime.TotalSeconds < time + leafSpawnInterval)
                return;
            time = gameTime.TotalGameTime.TotalSeconds;

            int leafAmount = 4;
            float max = 0.1f;

            for (int i = 1; i <= leafAmount; i++)
            {
                Vector2 location = new(random.Next(game.player.camera.Window.Left + ((i - 1) * game.player.camera.Window.Right / leafAmount), i * game.player.camera.Window.Right / leafAmount), game.player.camera.Window.Top);
                Vector2 dir = location - game.player.Location.ToVector2();
                dir.Normalize();
                float x = -dir.X;
                if (x >= max)
                    x = max;
                if (x <= -max)
                    x = -max;
                leaves.Add(new(game, "Sprites/Bosses/Leaf_x3", location, new(x * 5, 5)));
            }
        }

        internal void Reset()
        {
            leaves.Clear();
        }
    }
}
