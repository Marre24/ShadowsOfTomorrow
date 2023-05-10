using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ShadowsOfTomorrow
{
    public class StartCutScene : IUpdateAndDraw
    {
        public bool HaveEnded { get; internal set; }

        private Rectangle rec;
        private readonly Texture2D texture;
        private readonly Camera camera;
        private readonly SpriteFont font;
        public Player player;
        int phaseCounter = 0;
        int dialogueCounter = 0;
        float transparency = 0;
        float whiteTransparency = 0;
        bool isGoingUp = true;

        readonly List<string> dialogueList1 = new()
        {
            "T-Minus 10",
            "9",
            "8",
            "7",
            "6",
            "5",
            "4",
            "3",
            "2",
            "1",
            "And we have liftoff",
            "Bring us a habitable planet ____ , \n the human population is counting on you",
            "15 months later...",
        };
        readonly List<string> dialogueList2 = new()
        {
            "_____ , can you hear us?",
            "Mayday, mayday, mayday",
            "The ship is going down",
            "I repeat the ship is going down",
        };
        private readonly Texture2D red;
        private readonly Texture2D white;
        readonly Point size = new(900, 420);

        public StartCutScene(Game1 game, Camera camera, Player player)
        {
            texture = game.Content.Load<Texture2D>("UI/DialogueBox_x3");
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            rec = new(camera.Window.Center, new(900, 420));
            red = new Texture2D(game.GraphicsDevice, 1, 1);
            red.SetData<Color>(new Color[] { Color.Red });
            white = new Texture2D(game.GraphicsDevice, 1, 1);
            white.SetData<Color>(new Color[] { Color.White });

            HaveEnded = false;
            this.camera = camera;
            this.player = player;
        }

        double timeSinceWordUpdate = 5;

        public void Update(GameTime gameTime)
        {
            

            rec = new(camera.Window.Center - new Point(size.X / 2, size.Y / 2), size);
            switch (phaseCounter)
            {
                case 0:
                    PhaseOne(gameTime);
                    break;
                case 1:
                    PhaseTwo(gameTime);
                    break;
                case 2:
                    PhaseThree(gameTime);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rec, null, Color.White * (1 - whiteTransparency), 0, Vector2.Zero, SpriteEffects.None, 0.9f);

            switch (phaseCounter)
            {
                case 0:
                    PhaseOne(spriteBatch);
                    break;
                case 1:
                    PhaseTwo(spriteBatch);
                    break;
                case 2:
                    PhaseThree(spriteBatch);
                    break;
            }
        }

        public void PhaseOne(GameTime gameTime)
        {
            camera.Follow(Point.Zero);

            if (gameTime.TotalGameTime.TotalSeconds > dialogueList1[dialogueCounter].Split(" ").Length * 1.6 + timeSinceWordUpdate)
            {
                timeSinceWordUpdate = gameTime.TotalGameTime.TotalSeconds;
                dialogueCounter++;
            }

            if (dialogueCounter < dialogueList1.Count)
                return;

            phaseCounter++;
            dialogueCounter = 0;
        }

        public void PhaseOne(SpriteBatch spriteBatch)
        {
            camera.Follow(Point.Zero, true);

            spriteBatch.DrawString(font, dialogueList1[dialogueCounter], rec.Location.ToVector2() + new Vector2(52, 100), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
        }

        public void PhaseTwo(GameTime gameTime)
        {
            camera.Follow(Point.Zero, true);

            if (gameTime.TotalGameTime.TotalSeconds > dialogueList2[dialogueCounter].Split(" ").Length * 1.5 + timeSinceWordUpdate)
            {
                timeSinceWordUpdate = gameTime.TotalGameTime.TotalSeconds;
                dialogueCounter++;
            }

            if (isGoingUp)
                transparency += 0.005f;
            else
                transparency -= 0.005f;

            if (isGoingUp && transparency > 0.4f)
                isGoingUp = false;

            if (!isGoingUp && transparency < 0.1f)
                isGoingUp = true;

            if (dialogueCounter < dialogueList2.Count)
                return;

            phaseCounter++;
            time = gameTime.TotalGameTime.TotalSeconds;
        }

        public void PhaseTwo(SpriteBatch spriteBatch)
        {
            camera.Follow(Point.Zero, true);

            spriteBatch.DrawString(font, dialogueList2[dialogueCounter], rec.Location.ToVector2() + new Vector2(52, 100), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);

            spriteBatch.Draw(red, camera.Window, null, Color.White * transparency, 0, Vector2.Zero, SpriteEffects.None, 0.93f);
        }

        private double time;

        public void PhaseThree(GameTime gameTime)
        {
            camera.Follow(Point.Zero, true);

            whiteTransparency += 0.007f;

            if (whiteTransparency <= 1 || time + 5 > gameTime.TotalGameTime.TotalSeconds)
                return;

            End(player);
        }

        public void PhaseThree(SpriteBatch spriteBatch)
        {
            camera.Follow(Point.Zero, true);

            spriteBatch.DrawString(font, dialogueList2[dialogueList2.Count - 1], rec.Location.ToVector2() + new Vector2(52, 100), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);

            spriteBatch.Draw(white, camera.Window, null, Color.White * whiteTransparency, 0, Vector2.Zero, SpriteEffects.None, 0.94f);
        }

        private void End(Player player)
        {
            HaveEnded = true;
            player.CurrentAction = Action.InMainMenu;
        }

    }
}
