using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class BranchingSide : BaseClassPhase
    {
        readonly List<Branch> branches = new();
        readonly List<Leaf> leaves = new();
        readonly Random random = new();
        private readonly Game1 game;
        private readonly Boss boss;
        private double time = 0;
        private const double leafSpawnInterval = 2;
        private int amountOfBranchesSpawned = 0;

        public BranchingSide(Game1 game, Boss boss) 
        {
            this.game = game;
            this.boss = boss;
            maxStunOMeter = 1;
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
                if (branches[i].HitBox.Left > 45 * 48)
                    branches.Remove(branches[i]);

            if (boss.isStunned)
                return;

            if (gameTime.TotalGameTime.TotalSeconds < time + leafSpawnInterval)
                return;

            time = gameTime.TotalGameTime.TotalSeconds;

            if (random.Next(0, 2) == 0)
                branches.Add(new(game, "UI/DialogueBox_x3", new(48 * 4, 21 * 48), true));
            else
                branches.Add(new(game, "UI/DialogueBox_x3", new(48 * 4, 20 * 48), true));

            if (++amountOfBranchesSpawned % 4 == 0)
            {
                int leafAmount = 3;

                for (int i = 1; i <= leafAmount; i++)
                {
                    Vector2 location = new(random.Next(game.player.camera.Window.Left + ((i - 1) * boss.HitBox.Left / leafAmount), i * boss.HitBox.Left / leafAmount), game.player.camera.Window.Top - 200);
                    leaves.Add(new(game, "Sprites/Bosses/Leaf_x3", location, new(0, 5), boss));
                }
            }
        }
    }
}
