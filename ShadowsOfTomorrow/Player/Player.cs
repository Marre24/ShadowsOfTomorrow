using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using SharpDX.Direct3D9;
using SharpDX.WIC;

namespace ShadowsOfTomorrow
{
    public enum Mach
    {
        Standing,
        Walking,
        Running,
        Sprinting,
    }

    public enum Action
    {
        Standing,
        Crouching,
        GroundPounding,
        Attacking,
        Turning,
        Rolling,
    }

    public enum Facing
    {
        Right,
        Left,
    }

    public class Player : IUpdateAndDraw
    {
        readonly AnimationManager animationManager = new();

        private readonly Animation idle;


        public Point Location { get => hitBox.Location; set => hitBox.Location = value; }
        public Mach ActiveMach { get => _activeMach; private set => _activeMach = value; }
        public Mach OldMach { get; private set; }
        public Action CurrentAction { get; private set; } = Action.Standing;
        public Action OldAction { get; private set; } = Action.Standing;
        public Point Size { get => activeSize; private set { hitBox.Size = value; activeSize = value; } }
        public Vector2 Speed { get => speed; }
        public Rectangle HitBox { get => hitBox; }

        private readonly Game1 game;
        public readonly Camera camera = new();

        private Facing facing;
        private Rectangle hitBox;
        private Mach _activeMach;
        private Vector2 speed = Vector2.Zero;
        private Point activeSize = new(32 * scale, 32 * scale);
        private Point size = new(32 * scale, 32 * scale);
        private Point crouchingSize = new(32 * scale, 16 * scale);
        private readonly SpriteFont font;
        private KeyboardState oldState = Keyboard.GetState();

        public bool isGrounded;
        private float speedBeforeTurn;

        private const int scale = 2;

        private const int walkingSpeed = 5;
        private const int runningSpeed = 10;
        private const int sprintingSpeed = 15;
        private const int maxYSpeed = 10;
        private const int jumpForce = -10;
        private const float brakeSpeed = 0.2f;
        private const float acceleration = 0.25f;
        private const float gravitation = 0.4f;
        private const float groundPoundSpeed = 12.0f;
        private const float groundPoundAcceleration = 1.0f;

        public Player(Game1 game)
        {
            hitBox = new(Point.Zero, size);

            idle = new(game.Content.Load<Texture2D>("Sprites/Player/IdleLeft"), game.Content.Load<Texture2D>("Sprites/Player/IdleRight"), 18);
            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");

            this.game = game;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (ActiveMach)
            {
                case Mach.Standing:
                    switch (CurrentAction)
                    {
                        case Action.Standing:
                            animationManager.Play(idle);
                            break;
                        case Action.Crouching:
                            break;
                        case Action.GroundPounding:
                            break;
                        case Action.Attacking:
                            break;
                    }

                    break;
                case Mach.Walking:
                    break;
                case Mach.Running:
                    break;
                case Mach.Sprinting:
                    break;
                default:
                    break;
            }


            animationManager.Draw(spriteBatch, Location.ToVector2(), facing);

            spriteBatch.DrawString(font, "Speed: " + Math.Round(speed.X).ToString(), camera.Window.Location.ToVector2(), Color.White);
            spriteBatch.DrawString(font, ActiveMach.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 25), Color.White);
            spriteBatch.DrawString(font, CurrentAction.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 50), Color.White);
            spriteBatch.DrawString(font, isGrounded.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 75), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            camera.Follow(new(new(hitBox.Left, hitBox.Bottom - size.Y), size), game.mapManager.ActiveMap);

            SetPlayerMach(); 
            SetPlayerDirection();

            CheckPlayerInput();
            CheckPlayerAction();
            MovePlayer();

            animationManager.Update(gameTime);

            oldState = Keyboard.GetState();
            OldMach = ActiveMach;
            OldAction = CurrentAction;
        }

        private void SetPlayerMach()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (Speed.X == 0)
                ActiveMach = Mach.Standing;
            else if (speed.X <= -walkingSpeed || speed.X <= walkingSpeed)
                ActiveMach = Mach.Walking;
            if ((speed.X < -walkingSpeed || speed.X > walkingSpeed) && (keyboardState.IsKeyDown(Keys.LeftShift) || OldMach == Mach.Running) && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Running;
            if ((speed.X < -runningSpeed || speed.X > runningSpeed) && (keyboardState.IsKeyDown(Keys.LeftShift) || OldMach == Mach.Sprinting) && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Sprinting;
        }
        
        private void SetPlayerDirection()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                facing = Facing.Right;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                facing = Facing.Left;
        }

        private void CheckPlayerInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            switch (ActiveMach)
            {
                case Mach.Running or Mach.Sprinting:
                    if (keyboardState.IsKeyDown(Keys.S) && isGrounded)
                        CurrentAction = Action.Rolling;
                    else if (OldAction == Action.Rolling && keyboardState.IsKeyUp(Keys.S))
                        StandUp();
                    if (keyboardState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        Jump();
                    break;

                case Mach.Standing or Mach.Walking:
                    if (keyboardState.IsKeyDown(Keys.S) && isGrounded && OldAction == Action.Standing)
                        CurrentAction = Action.Crouching;
                    else if (OldAction == Action.Crouching && keyboardState.IsKeyUp(Keys.S))
                        StandUp();
                    if (keyboardState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        Jump();
                    if (keyboardState.IsKeyDown(Keys.S) && !isGrounded && CurrentAction != Action.Crouching)
                        GroundPound();
                    else if (isGrounded && CurrentAction == Action.GroundPounding)
                        CurrentAction = Action.Standing;
                    break;
            }
            
            UpdateSpeed(keyboardState);
        }

        private void CheckPlayerAction()
        {
            switch (CurrentAction)
            {
                case Action.Crouching:
                    Crouch();
                    break;
                case Action.Rolling:
                    Roll();
                    if (-walkingSpeed <= speed.X && speed.X <= walkingSpeed)
                        StandUp();
                    break;
                case Action.Attacking:
                    break;
                case Action.Turning:
                    StandUp();
                    if (speedBeforeTurn < 0)
                    {
                        speed.X += brakeSpeed;
                        if (speed.X >= 0)
                        {
                            speed.X = -1 * speedBeforeTurn;
                            StandUp();
                            if (OldAction != Action.Rolling && OldAction != Action.Crouching)
                                CurrentAction = Action.Standing; 
                            if (speedBeforeTurn < -walkingSpeed)
                                ActiveMach = Mach.Running;
                            if (speedBeforeTurn < -runningSpeed)
                                ActiveMach = Mach.Sprinting;
                        }
                    }
                    if (speedBeforeTurn > 0)
                    {
                        speed.X -= brakeSpeed;
                        if (speed.X <= 0)
                        {
                            speed.X = -1 * speedBeforeTurn;
                            StandUp();
                            if (OldAction != Action.Rolling && OldAction != Action.Crouching)
                                CurrentAction = Action.Standing;
                            ActiveMach = Mach.Sprinting;
                            if (speedBeforeTurn > walkingSpeed)
                                ActiveMach = Mach.Running;
                            if (speedBeforeTurn > runningSpeed)
                                ActiveMach = Mach.Sprinting;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Roll()
        {
            if (OldAction == Action.Rolling || OldAction == Action.Crouching)
                return;
            Size = crouchingSize;
            Location += new Point(0, crouchingSize.Y);
        }

        private void GroundPound()
        {
            if (CurrentAction == Action.GroundPounding)
                return;
            CurrentAction = Action.GroundPounding;
            speed.Y = 0;
        }

        private void Crouch()
        {
            if (OldAction == Action.Crouching ||OldAction == Action.Rolling)
                return;
            Size = crouchingSize;
            Location += new Point(0, crouchingSize.Y);
        }

        private void StandUp()
        {
            if (OldAction != Action.Rolling && OldAction != Action.Crouching)
                return; 
            Size = size;
            Location -= new Point(0, crouchingSize.Y);
            CurrentAction = Action.Standing;
        }

        private void Jump()
        {
            speed.Y = jumpForce;
        }

        private void UpdateSpeed(KeyboardState state)
        {
            switch (ActiveMach)
            {
                case Mach.Running:
                    if (state.IsKeyDown(Keys.A) && speed.X > 0 && CurrentAction != Action.Turning)
                    {
                        CurrentAction = Action.Turning;
                        speedBeforeTurn = speed.X;
                    }
                    else if (state.IsKeyDown(Keys.A) && speed.X >= -runningSpeed)
                        speed.X -= acceleration / 4;
                    if (state.IsKeyDown(Keys.D) && speed.X < 0 && CurrentAction != Action.Turning)
                    {
                        CurrentAction = Action.Turning;
                        speedBeforeTurn = speed.X;
                    }
                    else if (state.IsKeyDown(Keys.D) && speed.X <= runningSpeed)
                        speed.X += acceleration / 4;
                    break;
                case Mach.Sprinting:
                    if (state.IsKeyDown(Keys.A) && speed.X > 0 && CurrentAction != Action.Turning)
                    {
                        CurrentAction = Action.Turning;
                        speedBeforeTurn = speed.X;
                    }
                    else if (state.IsKeyDown(Keys.A) && speed.X >= -sprintingSpeed)
                        speed.X -= acceleration / 8;
                    if (state.IsKeyDown(Keys.D) && speed.X < 0 && CurrentAction != Action.Turning)
                    {
                        CurrentAction = Action.Turning;
                        speedBeforeTurn = speed.X;
                    }
                    else if (state.IsKeyDown(Keys.D) && speed.X <= sprintingSpeed)
                        speed.X += acceleration / 8;
                    break;
                case Mach.Standing or Mach.Walking:
                    if (state.IsKeyDown(Keys.A) && speed.X >= -walkingSpeed)
                        speed.X -= acceleration;
                    if (state.IsKeyDown(Keys.D) && speed.X <= walkingSpeed)
                        speed.X += acceleration;
                    break;
            }
            WillSlowDown();
        }

        private void WillSlowDown()
        {
            if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D))
            {
                if (speed.X < 0)
                    speed.X += brakeSpeed;
                if (speed.X > 0)
                    speed.X -= brakeSpeed;
                if (-brakeSpeed < speed.X && speed.X < brakeSpeed)
                    speed.X = 0;
            }
        }

        private void MovePlayer()
        {
            if (CurrentAction == Action.GroundPounding && speed.Y < groundPoundSpeed)
                speed.Y += groundPoundAcceleration;
            else if (speed.Y < maxYSpeed)
                speed.Y += gravitation;

            (bool canMoveX, bool canMoveY) = game.mapManager.ActiveMap.WillCollide(this);

            if (canMoveX)
                Location += new Point((int)speed.X, 0);
            else
                speed.X = 0;

            if (canMoveY)
                Location += new Point(0, (int)speed.Y);
            else
                speed.Y = 1;
        }
    }
}
