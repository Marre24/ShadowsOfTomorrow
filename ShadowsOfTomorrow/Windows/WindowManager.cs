﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class WindowManager : IUpdateAndDraw
    {
        private readonly Game1 game;
        private DialogueWindow dialogueWindow;

        public WindowManager(Game1 game) 
        {
            this.game = game;
        }

        public void SetDialogue(Dialogue dialogue)
        {
            dialogueWindow = new(game, dialogue);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (game.player.CurrentAction == Action.Talking)
                dialogueWindow.Draw(spriteBatch);

        }

        public void Update(GameTime gameTime)
        {
            if (game.player.CurrentAction == Action.Talking)
                dialogueWindow.Update(gameTime);
        }
    }
}