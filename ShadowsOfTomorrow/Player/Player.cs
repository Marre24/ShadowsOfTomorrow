using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public enum State
    {
        Running,
        Charging,
        Standing,
    }

    public class Player : IUpdateAndDraw
    {
        public Point Location { get => hitBox.Location; private set => hitBox.Location = value; }
        public Point Size { get => size; }
        public Vector2 Speed { get => speed; }

        private Vector2 speed = Vector2.Zero;
        private const int maxXSpeed = 15;
        private const int maxYSpeed = 8;
        private const float brakeSpeed = 0.4f;
        private const float acceleration = 0.3f;
        private const float amountOfSecondsTilFullCharge = 1.0f;
        private float chargeProcent = 0;
        private float startProcent;
        private float chargeStartTime;
        private bool firstFrameInCharge = true;
        private const float gravitation = 0.4f;
        private const float fastFallSpeed = gravitation * 3;
        private float currentGravitation = gravitation;
        private const int jumpForce = -10;

        public State ActiveState { get => activeState; private set => activeState = value; }
        private State activeState;

        private readonly Point size = new(50, 50);
        private Rectangle hitBox;
        private readonly Texture2D playerTexture;
        private readonly Game1 game;
        public readonly Camera camera;
        KeyboardState oldState = Keyboard.GetState();


        public Player(Point startLocation, Game1 game)
        {
            hitBox = new(startLocation, size);
            playerTexture = game.Content.Load<Texture2D>("Sprites/walterwhite");
            this.game = game;
            camera = new();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, hitBox, Color.White);
            //spriteBatch.DrawString(game.Content.Load<SpriteFont>("Fonts/DefaultFont"), "Speed: " + Math.Round(speed.X).ToString(), Vector2.Zero, Color.White);
            //spriteBatch.DrawString(game.Content.Load<SpriteFont>("Fonts/DefaultFont"), "ChargeProcent: " + Math.Round(100 * chargeProcent).ToString() + "%", new(100, 0), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            camera.Follow(hitBox, game.map);
            SetPlayerState();

            switch (ActiveState)
            {
                case State.Running:
                    CheckPlayerAction();
                    break;
                case State.Charging:
                    Charge(gameTime);
                    break;
                case State.Standing:
                    CheckPlayerAction();
                    break;
                default:
                    break;
            }

            ApplyGravity();

            (bool canMoveX, bool canMoveY) = game.map.WillCollide(this);

            if (canMoveX)
                Location += new Point((int)speed.X, 0);
            else
                speed.X = 0;
            if (canMoveY)
                Location += new Point(0, (int)speed.Y);
            else
                speed.Y = 0;
        }

        private void SetPlayerState()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.LeftShift))
            {
                ActiveState = State.Charging;
                return;
            }

            if (ActiveState == State.Charging) // True if the last state was Charging
                ReleaseCharge(state);

            if (speed == Vector2.Zero)
                ActiveState = State.Standing;
            else
                ActiveState = State.Running;
        }

        private void ApplyGravity()
        {
            if (speed.Y < maxYSpeed)
                speed.Y += currentGravitation;
        }

        private void ReleaseCharge(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.A))
                speed.X = -maxXSpeed * chargeProcent;
            else if (state.IsKeyDown(Keys.D))
                speed.X = maxXSpeed * chargeProcent;

            firstFrameInCharge = true;
            chargeProcent = 0;
        }

        private void CheckPlayerAction()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                Jump();

            if (state.IsKeyDown(Keys.S))
                FastFall();
            else
                currentGravitation = gravitation;

            MoveSideWays(state);

            oldState = Keyboard.GetState();
        }



        private void Charge(GameTime gameTime)
        {
            SlowDown();                                             //You slow down because you can not move while charging

            if (firstFrameInCharge)
            {
                firstFrameInCharge = false;
                chargeStartTime = (float)gameTime.TotalGameTime.TotalSeconds;
                startProcent = Math.Abs(speed.X) / maxXSpeed;
            }

            chargeProcent = startProcent + ((float)gameTime.TotalGameTime.TotalSeconds - chargeStartTime) / amountOfSecondsTilFullCharge;

            if (chargeProcent > 1)
                chargeProcent = 1;
        }

        private void SlowDown()
        {
            if ((Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D)) || ActiveState == State.Charging)
            {
                if (speed.X < 0)
                    speed.X += brakeSpeed;
                if (speed.X > 0)
                    speed.X -= brakeSpeed;
            }
        }

        private void FastFall()
        {
            if (oldState.IsKeyUp(Keys.S) && speed.Y < 0)
                speed.Y = 0.1f;
            currentGravitation = fastFallSpeed;
        }

        private void Jump()
        {
            speed.Y = jumpForce;
        }

        private void MoveSideWays(KeyboardState state)
        {

            if (state.IsKeyDown(Keys.A))
            {
                if (speed.X >= 0)
                    speed.X -= brakeSpeed;
                else if (speed.X >= -5)
                    speed.X -= acceleration;
                else if (speed.X >= -10)
                    speed.X -= acceleration / 2;
                else if (speed.X >= -maxXSpeed)
                    speed.X -= acceleration / 4;
            }

            if (state.IsKeyDown(Keys.D))
            {
                if (speed.X <= 0)
                    speed.X += brakeSpeed;
                if (speed.X <= 5)
                    speed.X += acceleration;
                else if (speed.X <= 10)
                    speed.X += acceleration / 2;
                else if (speed.X <= maxXSpeed)
                    speed.X += acceleration / 4;
            }
            SlowDown();
        }
    }
}
