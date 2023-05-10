using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ShadowsOfTomorrow
{
    public class BranchingUp : BaseClassPhase
    {

        readonly List<Branch> branches = new();
        readonly List<Leaf> leaves = new();
        readonly Random random = new();
        private readonly Game1 game;
        private readonly Boss boss;
        private double time = 0;
        private const double leafSpawnInterval = 2;
        private int amountOfBranchesSpawned = 0;

        public BranchingUp(Game1 game, Boss boss)
        {
            this.game = game;
            maxStunOMeter = 2;
            this.boss = boss;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var branch in branches)
                branch.Draw(spriteBatch);
            foreach (var leaf in leaves)
                leaf.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var branch in branches)
                branch.Update(gameTime);

            foreach (var leaf in leaves)
                leaf.Update(gameTime);

            for (int i = 0; i < leaves.Count; i++)
                if (leaves[i].Hitbox.Top >= game.player.camera.Window.Bottom || leaves[i].hasHitSomeone)
                    leaves.Remove(leaves[i]);

            for (int i = 0; i < branches.Count; i++)
                if (branches[i].HitBox.Top > 30 * 48)
                    branches.Remove(branches[i]);

            if (boss.isStunned)
                return;

            if (gameTime.TotalGameTime.TotalSeconds < time + leafSpawnInterval)
                return;

            time = gameTime.TotalGameTime.TotalSeconds;

            int branchAmount = 5;

            for (int i = 1; i <= branchAmount; i++)
            {
                if (i == 1)
                {
                    int min = 48 * 4;
                    int max = 48 * 30 / branchAmount;
                    Vector2 location = new(random.Next(min, max), 30 * 48);
                    branches.Add(new(game, "UI/DialogueBox_x3", location, false));
                    
                }
                else
                {
                    int min = ((i - 1) * 48 * 30 / branchAmount);
                    int max = i * 48 * 30 / branchAmount;
                    Vector2 location = new(random.Next(min, max), 30 * 48);
                    branches.Add(new(game, "UI/DialogueBox_x3", location, false));
                }
            }

            if (++amountOfBranchesSpawned % 5 == 0)
            {
                int leafAmount = 4;

                for (int i = 1; i <= leafAmount; i++)
                {
                    int min = game.player.camera.Window.Left + ((i - 1) * boss.HitBox.Left / leafAmount);
                    int max = i * boss.HitBox.Left / leafAmount;
                    Vector2 location = new(random.Next(min, max), game.player.camera.Window.Top);
                    leaves.Add(new(game, "Sprites/Bosses/Leaf_x3", location, new(0, 5), boss));
                }
            }
        }
    }
}
