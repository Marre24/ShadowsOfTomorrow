﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public DialogueWindow dialogueWindow;
        private EndingScreen endingScreen;
        private readonly DeadScreen deadScreen;
        private readonly StartScreen startScreen;
        private readonly ChangeKeybindWindow changeKeybindWindow;
        private readonly ChangeVolyme changeVolyme;
        private readonly PausScreen pausScreen;

        public WindowManager(Game1 game) 
        {
            this.game = game;
            deadScreen = new(game, game.Player);
            startScreen = new(game);
            pausScreen = new(game.GraphicsDevice, game.Player.camera, game);
            changeKeybindWindow = new(game, game.Player.Keybinds, startScreen, pausScreen);
            dialogueWindow = new(game);
            changeVolyme = new(game, startScreen, pausScreen);
        }

        public void SetDialogue(Dialogue dialogue)
        {
            dialogueWindow.SetDialogue(dialogue);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (game.Player.CurrentAction == Action.Talking)
                dialogueWindow.Draw(spriteBatch);

            if (game.Player.CurrentAction == Action.Ended)
                endingScreen.Draw(spriteBatch);

            if (game.Player.CurrentAction == Action.Dead)
                deadScreen.Draw(spriteBatch);

            if (game.Player.CurrentAction == Action.InMainMenu)
                startScreen.Draw(spriteBatch);

            if (game.Player.CurrentAction == Action.ChangingKeybinds)
                changeKeybindWindow.Draw(spriteBatch);

            if (game.Player.CurrentAction == Action.ChangingVolyme)
                changeVolyme.Draw(spriteBatch);

            if (game.Player.CurrentAction == Action.Paused)
                pausScreen.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            if (game.Player.CurrentAction == Action.Talking)
                dialogueWindow.Update(gameTime);
            if (game.Player.CurrentAction == Action.Ended)
                endingScreen.Update(gameTime);
            if (game.Player.CurrentAction == Action.Dead)
                deadScreen.Update(gameTime);
            if (game.Player.CurrentAction == Action.InMainMenu)
                startScreen.Update(gameTime);
            if (game.Player.CurrentAction == Action.ChangingKeybinds)
                changeKeybindWindow.Update();
            if (game.Player.CurrentAction == Action.Paused)
                pausScreen.Update(gameTime);
            if (game.Player.CurrentAction == Action.ChangingVolyme)
                changeVolyme.Update(gameTime);
        }

        public void SetEnd(Boss boss)
        {
            endingScreen = new(game, game.Player, boss);
        }
    }
}
