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
        private readonly LeafFalling leafFalling;
        private readonly LeafBlowing leafBlowing;

        public PhaseManager(Boss boss, Game1 game)
        {
            this.boss = boss;
            this.game = game;
            leafFalling = new(game, 1);
            leafBlowing = new(game, 1, game.player);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (boss.ActivePhase)
            {
                case Phase.Dialogue:
                    game.windowManager.Draw(spriteBatch);
                    break;
                case Phase.Stunned:
                    break;
                case Phase.LeafFalling:
                    leafFalling.Draw(spriteBatch);
                    break;
                case Phase.LeafWind:
                    leafBlowing.Draw(spriteBatch);
                    break;
                case Phase.BranchingSide:
                    break;
                case Phase.BranchingUp:
                    break;
            }
            
        }

        public void Update(GameTime gameTime)
        {
            switch (boss.ActivePhase)
            {
                case Phase.Dialogue:
                    game.windowManager.Update(gameTime);
                    break;
                case Phase.Stunned:
                    break;
                case Phase.LeafFalling:
                    leafFalling.Update(gameTime);
                    break;
                case Phase.LeafWind:
                    leafBlowing.Update(gameTime);
                    break;
                case Phase.BranchingSide:
                    break;
                case Phase.BranchingUp:
                    break;
            }
        }

        internal void Reset()
        {
            leafFalling.Reset();
        }
    }
}
