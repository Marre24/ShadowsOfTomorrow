﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class DialogueWindow : IUpdateAndDraw
    {
        private Rectangle window;
        private readonly Point size = new(1000, 400);
        private readonly SpriteFont font;
        private readonly Texture2D texture;
        private readonly Game1 game;
        private readonly Boss boss;
        private readonly Dialogue dialogue;
        private string goTo = "First";
        private string displayAnswer = "";
        private int index = 0;
        private bool showingQuestions = true;
        private KeyboardState oldState = Keyboard.GetState();

        public DialogueWindow(Game1 game, Boss boss, Dialogue dialogue) 
        {
            this.game = game;
            this.boss = boss;
            this.dialogue = dialogue;

            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");
            texture = game.Content.Load<Texture2D>("Box");
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
                game.player.CurrentAction = Action.Standing;

            

            if (state.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
            {
                if (dialogue.IsBoss)
                {
                    if (dialogue.bossDialogue.Count <= ++index)
                    {
                        game.player.CurrentAction = Action.Standing;
                        index = 0;
                        return;
                    }
                    oldState = state;
                    return;
                }

                if (showingQuestions)
                {
                    displayAnswer = dialogue.GetAnswer(goTo, index);
                    goTo = dialogue.GoTo(displayAnswer);
                }
                showingQuestions = !showingQuestions;
            }

            if (!dialogue.IsBoss)
                UpdateIndex(state);
            else
                window = new(game.player.camera.Window.Center + new Point(-200, 500 - size.Y), size);

            oldState = state;
        }

        private void UpdateIndex(KeyboardState state)
        {
            if (!showingQuestions)
                return;
            if (state.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
                index++;

            else if (state.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
                index--;

            if (index > dialogue.GetQuestions(goTo).Count - 1)
                index = 0;
            if (index < 0)
                index = dialogue.GetQuestions(goTo).Count - 1;

            window = new(game.player.HitBox.Center + new Point(-size.X / 2, 500 - size.Y), size);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, window, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.91f);

            if (dialogue.IsBoss)
            {
                spriteBatch.DrawString(font, dialogue.bossDialogue[boss.talkingIndex][index], window.Location.ToVector2() + new Vector2(50, 200), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
                return;
            }

            if (showingQuestions)
                ShowQuestions(spriteBatch);
            else
                ShowAnswer(spriteBatch);
            
            spriteBatch.DrawString(font, index.ToString(), window.Location.ToVector2() + new Vector2(50, 300), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
        }

        private void ShowAnswer(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, displayAnswer, window.Location.ToVector2() + new Vector2(50, 200), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
        }

        private void ShowQuestions(SpriteBatch spriteBatch)
        {
            List<string> questions = dialogue.GetQuestions(goTo);

            for (int i = 0; i < questions.Count; i++)
            {
                if (i == index)
                    spriteBatch.DrawString(font, questions[i], window.Location.ToVector2() + new Vector2(52, 100 + i * 40), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
                else
                    spriteBatch.DrawString(font, questions[i], window.Location.ToVector2() + new Vector2(50, 100 + i * 40), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            }
        }
    }
}
