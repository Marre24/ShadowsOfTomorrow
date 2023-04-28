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

        public PhaseManager(Boss boss, Game1 game)
        {
            this.boss = boss;
            this.game = game;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (boss.ActivePhase)
            {
                case Phase.StartDialogue:
                    game.windowManager.Draw(spriteBatch);
                    break;
                case Phase.Stunned:
                    break;
                case Phase.LeafFalling:
                    break;
                case Phase.LeafWind:
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
                case Phase.StartDialogue:
                    game.windowManager.Update(gameTime);
                    break;
                case Phase.Stunned:
                    break;
                case Phase.LeafFalling:
                    break;
                case Phase.LeafWind:
                    break;
                case Phase.BranchingSide:
                    break;
                case Phase.BranchingUp:
                    break;
            }
        }
    }
}
