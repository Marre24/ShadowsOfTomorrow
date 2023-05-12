using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class PhaseManager : IUpdateAndDraw
    {
        private readonly Boss boss;
        private readonly Game1 game;
        public readonly LeafFalling leafFalling;
        public readonly LeafBlowing leafBlowing;
        public readonly BranchingSide branchingSide;
        public readonly BranchingUp branchingUp;


        public PhaseManager(Boss boss, Game1 game)
        {
            this.boss = boss;
            this.game = game;
            leafFalling = new(game, 1, boss);
            leafBlowing = new(game, 1, game.Player, boss);
            branchingSide = new(game, boss);
            branchingUp = new(game, boss);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (boss.ActivePhase)
            {
                case Phase.Dialogue:
                    game.WindowManager.Draw(spriteBatch);
                    break;
                case Phase.LeafFalling:
                    leafFalling.Draw(spriteBatch);
                    break;
                case Phase.LeafWind:
                    leafBlowing.Draw(spriteBatch);
                    break;
                case Phase.BranchingSide:
                    branchingSide.Draw(spriteBatch);
                    break;
                case Phase.BranchingUp:
                    branchingUp.Draw(spriteBatch);
                    break;
            }
            
        }

        public void Update(GameTime gameTime)
        {
            switch (boss.ActivePhase)
            {
                case Phase.Dialogue:
                    game.WindowManager.Update(gameTime);
                    break;
                case Phase.LeafFalling:
                    leafFalling.Update(gameTime);
                    break;
                case Phase.LeafWind:
                    leafBlowing.Update(gameTime);
                    break;
                case Phase.BranchingSide:
                    branchingSide.Update(gameTime);
                    break;
                case Phase.BranchingUp:
                    branchingUp.Update(gameTime);
                    break;
            }
        }

        internal void Reset()
        {
            leafFalling.Reset();
        }
    }
}
