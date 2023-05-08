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
        Stunned,
        Ended,
        WachingCutScene,
        Dead
    }

    public enum Facing
    {
        Left = -1,
        Right = 1,
    }

    public class Player : IUpdateAndDraw
    {
        public readonly AnimationManager animationManager;


        private readonly Animation idle;
        private readonly Animation walking;
        private readonly Animation crawling;
        private readonly Animation crouching;
        private readonly Animation rolling;
        private readonly Animation running;
        private readonly Animation climbing;


        private readonly SpriteFont font;

        public Point Location { get => hitBox.Location; set => hitBox.Location = value;  }
        public Mach ActiveMach { get => _activeMach; set => _activeMach = value; }
        public Mach OldMach { get; private set; }
        public Action CurrentAction { get; set; } = Action.Standing;
        public Action OldAction { get; private set; } = Action.Standing;
        public Point Size { get => new(animationManager.Animation.FrameWidth, animationManager.Animation.FrameHeight); }
        public Point OldSize { get; set; }
        public Rectangle HitBox { get => hitBox; }
        public Rectangle NextVerticalHitBox { get => new(new(Location.X, Location.Y + (int)Math.Round(playerMovement.VerticalSpeed)), Size); }
        public Rectangle NextHorizontalHitBox { get => new(new(Location.X + (int)Math.Round(playerMovement.HorizontalSpeed), Location.Y), Size); }
        public Facing Facing { get; set; }

        public Color[] TextureData
        {
            get
            {
                Color[] color = new Color[animationManager.Animation.FrameWidth * animationManager.Animation.FrameHeight];
                animationManager.CurrentCropTexture.GetData(color);
                return color;
            }
        }

        public int LastSpawnPoint { get; internal set; }

        private readonly Game1 game;
        public readonly Camera camera = new();
        public readonly PlayerMovement playerMovement;
        public readonly PlayerAttacking playerAttacking;
        private readonly Input input;

        private Rectangle hitBox;
        private Mach _activeMach;

        public bool isGrounded;
        public bool isNextToWall = false;

        private int health = 5;

        public bool hasDestroyedBlock = false;
        public double destroyBlockTime = 0;

        public Player(Game1 game)
        {
            idle = new(game.Content.Load<Texture2D>("Sprites/Player/IdleLeft_x3"), game.Content.Load<Texture2D>("Sprites/Player/IdleRight_x3"), 15);
            walking = new(game.Content.Load<Texture2D>("Sprites/Player/WalkingLeft_x3"), game.Content.Load<Texture2D>("Sprites/Player/WalkingRight_x3"), 13);
            crawling = new(game.Content.Load<Texture2D>("Sprites/Player/CrawlingLeft_x3"), game.Content.Load<Texture2D>("Sprites/Player/CrawlingRight_x3"), 6);
            crouching = new(game.Content.Load<Texture2D>("Sprites/Player/CrochingLeft_x3"), game.Content.Load<Texture2D>("Sprites/Player/CrochingRight_x3"), 1);
            rolling = new(game.Content.Load<Texture2D>("Sprites/Player/RollLeft_x3"), game.Content.Load<Texture2D>("Sprites/Player/RollRight_x3"), 1);
            running = new(game.Content.Load<Texture2D>("Sprites/Player/RunningLeft_x3"), game.Content.Load<Texture2D>("Sprites/Player/RunningRight_x3"), 10);
            climbing = new(game.Content.Load<Texture2D>("Sprites/Player/ClimbLeft_x3"), game.Content.Load<Texture2D>("Sprites/Player/ClimbRight_x3"), 12);

            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");

            this.game = game;

            animationManager = new(idle, game);
            playerMovement = new(this, game);
            playerAttacking = new(this, game);
            input = new(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animationManager.Draw(spriteBatch, Location.ToVector2(), Facing);

            spriteBatch.DrawString(font, "Horizontal Speed: " + Math.Round(playerMovement.HorizontalSpeed, 2).ToString(), camera.Window.Location.ToVector2(), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            spriteBatch.DrawString(font, "Vertical Speed: " + Math.Round(playerMovement.VerticalSpeed, 2).ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 25), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            spriteBatch.DrawString(font, ActiveMach.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 50), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            spriteBatch.DrawString(font, CurrentAction.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 75), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            spriteBatch.DrawString(font, isGrounded.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 100), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            spriteBatch.DrawString(font, isNextToWall.ToString(), camera.Window.Location.ToVector2() + new Vector2(0, 125), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
        }

        public void Update(GameTime gameTime)
        {
            if (health <= 0)
                Die();

            SetAnimation();
            UpdateHitBox();
            SetPlayerMach();
            SetPlayerDirection();

            if (CurrentAction != Action.WachingCutScene)
            {
                if (hasDestroyedBlock && destroyBlockTime + 0.6 > gameTime.TotalGameTime.TotalSeconds)
                    camera.Follow(new(new(hitBox.Left, hitBox.Bottom - 32), new(32, 32)), game.mapManager.ActiveMap, true);
                else
                {
                    destroyBlockTime = 0;
                    hasDestroyedBlock = false;
                    camera.Follow(new(new(hitBox.Left, hitBox.Bottom - 32), new(32, 32)), game.mapManager.ActiveMap);
                }
            }


            input.CheckPlayerInput(gameTime);
            game.mapManager.ActiveMap.IsNextToWall(this);
            playerMovement.Update(gameTime);
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
            if (Facing == Facing.Right && xDiff > 0 && xDiff < 20)
                Location += new Point(xDiff, 0);

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
                        case Action.WachingCutScene:
                            animationManager.Play(idle);
                            break;
                        case Action.Crouching:
                            animationManager.Play(crouching);
                            break;
                        case Action.GroundPounding:
                            animationManager.Play(rolling);
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
                            animationManager.Play(crawling);
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
                    switch (CurrentAction)
                    {
                        case Action.Standing:
                            animationManager.Play(running);
                            break;
                        case Action.Rolling:
                            animationManager.Play(rolling);
                            break;
                        case Action.WallClimbing:
                            animationManager.Play(climbing);
                            break;
                    }
                    break;
                case Mach.Sprinting:
                    switch (CurrentAction)
                    {
                        case Action.Standing:
                            animationManager.Play(running);
                            break;
                        case Action.Rolling:
                            animationManager.Play(rolling);
                            break;
                        case Action.WallClimbing:
                            animationManager.Play(climbing);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetPlayerMach()
        {
            KeyboardState keyboardState = Keyboard.GetState();


            if (playerMovement.HorizontalSpeed == 0)
                ActiveMach = Mach.Standing;
            if ((playerMovement.HorizontalSpeed <= -PlayerMovement.walkingSpeed || playerMovement.HorizontalSpeed <= PlayerMovement.walkingSpeed) && playerMovement.HorizontalSpeed != 0)
                ActiveMach = Mach.Walking;
            if ((playerMovement.HorizontalSpeed < -PlayerMovement.walkingSpeed || playerMovement.HorizontalSpeed > PlayerMovement.walkingSpeed) &&
                (keyboardState.IsKeyDown(Keys.LeftShift) || playerMovement.HorizontalSpeed < -PlayerMovement.walkingSpeed - 1 || playerMovement.HorizontalSpeed > PlayerMovement.walkingSpeed + 1)
                && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Running;
            if ((playerMovement.HorizontalSpeed < -PlayerMovement.runningSpeed || playerMovement.HorizontalSpeed > PlayerMovement.runningSpeed) &&
                (keyboardState.IsKeyDown(Keys.LeftShift) || playerMovement.HorizontalSpeed < -PlayerMovement.runningSpeed - 1 || playerMovement.HorizontalSpeed > PlayerMovement.runningSpeed + 1)
                && CurrentAction != Action.Crouching)
                ActiveMach = Mach.Sprinting;

        }

        private void SetPlayerDirection()
        {
            if (CurrentAction == Action.Talking || CurrentAction == Action.WachingCutScene)
                return;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Facing = Facing.Right;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Facing = Facing.Left;
        }

        public void OnHit(Facing facing)
        {
            if (CurrentAction == Action.Stunned)
                return;
            playerMovement.StandUp();
            health--;
            CurrentAction = Action.Stunned;
            if (facing == Facing.Left)
                playerMovement.HorizontalSpeed = 7;
            else
                playerMovement.HorizontalSpeed = -7;
            playerMovement.VerticalSpeed = -10;
            isGrounded = false;
        }

        internal void Die()
        {
            CurrentAction = Action.Dead;
        }

        internal void Reset()
        {
            health = 5;
            if (CurrentAction != Action.Talking)
                CurrentAction = Action.Standing;
            playerMovement.Speed = Vector2.Zero;
        }

        internal bool HaveBlockOverHead(Rectangle rec)
        {
            return game.mapManager.ActiveMap.IsInsideWall(new Rectangle(new(rec.Left, rec.Bottom - 95), new(57, 95)));
        }
    }
}
