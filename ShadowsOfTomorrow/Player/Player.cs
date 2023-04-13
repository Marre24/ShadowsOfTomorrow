using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Talking,
        WallClimbing,
        Stunned
    }

    public enum Facing
    {
        Right,
        Left,
    }

    public class Player : IUpdateAndDraw
    {
        public readonly AnimationManager animationManager;
        

        private readonly Animation idle;
        private readonly Animation walking;

        private readonly SpriteFont font;

        public Point Location { get => hitBox.Location; set => hitBox.Location = value; }
        public Mach ActiveMach { get => _activeMach; set => _activeMach = value; }
        public Mach OldMach { get; private set; }
        public Action CurrentAction { get; set; } = Action.Standing;
        public Action OldAction { get; private set; } = Action.Standing;
        public Point Size { get => new(animationManager.Animation.FrameWidth, animationManager.Animation.FrameHeight); }
        public Point OldSize { get; set; }
        public Rectangle HitBox { get => hitBox; }
        public Rectangle NextVerticalHitBox { get => new(new(Location.X, Location.Y + (int)Math.Round(playerMovement.VerticalSpeed)), Size); }
        public Rectangle NextHorizontalHitBox { get => new(new(Location.X + (int)Math.Round(playerMovement.HorisontalSpeed), Location.Y), Size); }
        public Facing Facing { get; set; }

        private readonly Game1 game;
        public readonly Camera camera = new();
        public readonly PlayerMovement playerMovement;
        public readonly PlayerAttacking playerAttacking;
        private readonly Input input;

        private Rectangle hitBox;
        private Mach _activeMach;

        public bool isGrounded;

        public Player(Game1 game)
        {
            idle = new(game.Content.Load<Texture2D>("Sprites/Player/IdleLeft"), game.Content.Load<Texture2D>("Sprites/Player/IdleRight"), 18);
            walking = new(game.Content.Load<Texture2D>("Sprites/Player/WalkingLeft"), game.Content.Load<Texture2D>("Sprites/Player/WalkingRight"), 8);
            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");

            this.game = game;

            animationManager = new(idle);
            playerMovement = new(this, game);
            playerAttacking = new(this, game);
            input = new(this, game);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animationManager.Draw(spriteBatch, Location.ToVector2(), Facing);
            playerAttacking.Draw(spriteBatch);

            spriteBatch.DrawString(font, "Horisontal Speed: " + Math.Round(playerMovement.HorisontalSpeed, 2).ToString(), camera.Window.Location.ToVector2(), Color.White);
            spriteBatch.DrawString(font, "Vertical Speed: " + Math.Round(playerMovement.VerticalSpeed, 2).ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 25), Color.White);
            spriteBatch.DrawString(font, ActiveMach.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 50), Color.White);
            spriteBatch.DrawString(font, CurrentAction.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 75), Color.White);
            spriteBatch.DrawString(font, isGrounded.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 100), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            SetAnimation();
            UpdateHitBox();
            SetPlayerMach();
            SetPlayerDirection();

            camera.Follow(new(new(hitBox.Left, hitBox.Bottom - 32), new(32, 32)), game.mapManager.ActiveMap);

            input.CheckPlayerInput();
            playerMovement.Update();
            animationManager.Update(gameTime);
            playerAttacking.Update(gameTime);

            OldMach = ActiveMach;
            OldAction = CurrentAction;
            OldSize = Size;
        }

        private void UpdateHitBox()
        {
            int yDiff = Math.Abs(OldSize.Y - Size.Y);
            int xDiff = Math.Abs(OldSize.X - Size.X);


            if (Size.Y < OldSize.Y)
                Location += new Point(0, yDiff);
            if (Size.Y > OldSize.Y)
                Location -= new Point(0, yDiff);
            if (OldSize.X < Size.X && Facing == Facing.Right)
                Location -= new Point(xDiff, 0);

            hitBox = new Rectangle(hitBox.Location, Size);
        }

        private void SetAnimation()
        {
            switch (ActiveMach)
            {
                case Mach.Standing:
                    switch (CurrentAction)
                    {
                        case Action.Standing:
                            animationManager.Play(idle);
                            break;
                        case Action.Talking: 
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
                    switch (CurrentAction)
                    {
                        case Action.Standing:
                            animationManager.Play(walking);
                            break;
                        case Action.Crouching:
                            break;
                        case Action.GroundPounding:
                            break;
                        case Action.Attacking:
                            break;
                        case Action.Turning:
                            break;
                        default:
                            break;
                    }
                    break;
                case Mach.Running:
                    break;
                case Mach.Sprinting:
                    break;
                default:
                    break;
            }
        }

        private void SetPlayerMach()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (playerMovement.HorisontalSpeed == 0 && keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.D))
                ActiveMach = Mach.Standing;
            if ((playerMovement.HorisontalSpeed <= -PlayerMovement.walkingSpeed || playerMovement.HorisontalSpeed <= PlayerMovement.walkingSpeed) && playerMovement.HorisontalSpeed != 0)
                ActiveMach = Mach.Walking;
            if ((playerMovement.HorisontalSpeed < -PlayerMovement.walkingSpeed || playerMovement.HorisontalSpeed > PlayerMovement.walkingSpeed) && 
                (keyboardState.IsKeyDown(Keys.LeftShift) || OldMach == Mach.Running) && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Running;
            if ((playerMovement.HorisontalSpeed < -PlayerMovement.runningSpeed || playerMovement.HorisontalSpeed > PlayerMovement.runningSpeed) && 
                (keyboardState.IsKeyDown(Keys.LeftShift) || OldMach == Mach.Sprinting) && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Sprinting;
        }

        private void SetPlayerDirection()
        {
            if (CurrentAction == Action.Talking)
                return;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Facing = Facing.Right;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Facing = Facing.Left;
        }
    }
}
