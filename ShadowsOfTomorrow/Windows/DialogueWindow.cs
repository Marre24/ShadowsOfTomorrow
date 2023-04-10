using Microsoft.Xna.Framework;
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
        private readonly Dialogue dialogue;
        private string goTo = "First";
        private string displayAnswer = "";
        private int questionIndex = 0;
        private bool showingQuestions = true;
        private KeyboardState oldState = Keyboard.GetState();

        public DialogueWindow(Game1 game, Dialogue dialogue) 
        {
            this.game = game;
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
                if (showingQuestions)
                {
                    displayAnswer = dialogue.GetAnswer(goTo, questionIndex);
                    goTo = dialogue.GoTo(displayAnswer);
                }
                showingQuestions = !showingQuestions;
            }
            

            UpdateIndex(state);
            
            oldState = state;
        }

        private void UpdateIndex(KeyboardState state)
        {
            if (!showingQuestions)
                return;
            if (state.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
                questionIndex++;

            else if (state.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
                questionIndex--;

            if (questionIndex > dialogue.GetQuestions(goTo).Count - 1)
                questionIndex = 0;
            if (questionIndex < 0)
                questionIndex = dialogue.GetQuestions(goTo).Count - 1;

            window = new(game.player.HitBox.Center + new Point(-size.X / 2, 500 - size.Y), size);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, window, Color.White);
            if (showingQuestions)
                ShowQuestions(spriteBatch);
            else
                ShowAnswer(spriteBatch);
            
            spriteBatch.DrawString(font, questionIndex.ToString(), window.Location.ToVector2() + new Vector2(50, 300), Color.White);
        }

        private void ShowAnswer(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, displayAnswer, window.Location.ToVector2() + new Vector2(50, 200), Color.White);
        }

        private void ShowQuestions(SpriteBatch spriteBatch)
        {
            List<string> questions = dialogue.GetQuestions(goTo);

            for (int i = 0; i < questions.Count; i++)
            {
                if (i == questionIndex)
                    spriteBatch.DrawString(font, questions[i], window.Location.ToVector2() + new Vector2(52, i * 40), Color.Red);
                else
                    spriteBatch.DrawString(font, questions[i], window.Location.ToVector2() + new Vector2(50, i * 40), Color.White);
            }
        }
    }
}
