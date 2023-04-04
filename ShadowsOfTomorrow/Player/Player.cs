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
    }

    public enum Facing
    {
        Right,
        Left,
    }

    public class Player : IUpdateAndDraw
    {
        readonly AnimationManager animationManager;

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
        public Vector2 Speed { get => _speed; set => _speed = value; }
        public float VerticalSpeed { get => _speed.Y; set => _speed.Y = value; }
        public float HorisontalSpeed { get => _speed.X; set => _speed.X = value; }
        public Rectangle HitBox { get => hitBox; }
        public Facing Facing { get; set; }

        private readonly Game1 game;
        public readonly Camera camera = new();
        public readonly PlayerMovement playerMovement;

        private Rectangle hitBox;
        private Mach _activeMach;
        private Vector2 _speed = Vector2.Zero;

        public bool isGrounded;

        public Player(Game1 game)
        {
            idle = new(game.Content.Load<Texture2D>("Sprites/Player/IdleLeft"), game.Content.Load<Texture2D>("Sprites/Player/IdleRight"), 18);
            walking = new(game.Content.Load<Texture2D>("Sprites/Player/WalkingLeft"), game.Content.Load<Texture2D>("Sprites/Player/WalkingRight"), 8);
            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");

            this.game = game;

            animationManager = new(idle);
            playerMovement = new(this, game);
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


            animationManager.Draw(spriteBatch, Location.ToVector2(), Facing);

            spriteBatch.DrawString(font, "Speed: " + Math.Round(_speed.X).ToString(), camera.Window.Location.ToVector2(), Color.White);
            spriteBatch.DrawString(font, ActiveMach.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 25), Color.White);
            spriteBatch.DrawString(font, CurrentAction.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 50), Color.White);
            spriteBatch.DrawString(font, isGrounded.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 75), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            int diff = Math.Abs(OldSize.Y - Size.Y);

            if (Size.Y < OldSize.Y)
                Location += new Point(0, diff);
            if (Size.Y > OldSize.Y)
                Location -= new Point(0, diff);
            

            hitBox = new Rectangle(Location, Size);
            camera.Follow(new(new(hitBox.Left, hitBox.Bottom - 32), new(32, 32)), game.mapManager.ActiveMap);

            SetPlayerMach();
            SetPlayerDirection();

            playerMovement.Update(gameTime);

            animationManager.Update(gameTime);

            OldMach = ActiveMach;
            OldAction = CurrentAction;
            OldSize = Size;
        }

        private void SetPlayerMach()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (Speed.X == 0)
                ActiveMach = Mach.Standing;
            else if (_speed.X <= -PlayerMovement.walkingSpeed || _speed.X <= PlayerMovement.walkingSpeed)
                ActiveMach = Mach.Walking;
            if ((_speed.X < -PlayerMovement.walkingSpeed || _speed.X > PlayerMovement.walkingSpeed) && 
                (keyboardState.IsKeyDown(Keys.LeftShift) || OldMach == Mach.Running) && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Running;
            if ((_speed.X < -PlayerMovement.runningSpeed || _speed.X > PlayerMovement.runningSpeed) && 
                (keyboardState.IsKeyDown(Keys.LeftShift) || OldMach == Mach.Sprinting) && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Sprinting;
        }

        private void SetPlayerDirection()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Facing = Facing.Right;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Facing = Facing.Left;
        }
    }
}
