﻿using Microsoft.Xna.Framework;
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
        private readonly Boss boss;
        private double time = 0;

        public LeafBlowing(Game1 game, double leafSpawnInterval, Player player, Boss boss)
        {
            this.leafSpawnInterval = leafSpawnInterval;
            this.player = player;
            this.boss = boss;
            this.game = game;
            maxStunOMeter = 2;
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

            for (int i = 0; i < leaves.Count; i++)
                if (leaves[i].Hitbox.Top >= game.player.camera.Window.Bottom || leaves[i].hasHitSomeone)
                    leaves.Remove(leaves[i]);

            if (boss.isStunned)
                return;

            player.Location += new Point(-1, 0);
            
            if (gameTime.TotalGameTime.TotalSeconds < time + leafSpawnInterval)
                return;
            time = gameTime.TotalGameTime.TotalSeconds;

            int leafAmount = 4;
            float max = 0.1f;

            for (int i = 1; i <= leafAmount; i++)
            {
                int min = game.player.camera.Window.Left + ((i - 1) * game.player.camera.Window.Right / leafAmount);
                int maxi = i * game.player.camera.Window.Right / leafAmount;
                Vector2 location = new(random.Next(min, maxi), game.player.camera.Window.Top);
                Vector2 dir = location - game.player.Location.ToVector2();
                dir.Normalize();
                float x = -dir.X;
                if (x >= max)
                    x = max;
                if (x <= -max)
                    x = -max;
                leaves.Add(new(game, "Sprites/Bosses/Leaf_x3", location, new(x * 5, 5), boss));
            }
        }

        internal void Reset()
        {
            leaves.Clear();
        }
    }
}
