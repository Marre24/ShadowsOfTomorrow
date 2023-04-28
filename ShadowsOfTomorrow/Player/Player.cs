﻿using Microsoft.Xna.Framework.Graphics;
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

        public Player(Game1 game)
        {
            idle = new(game.Content.Load<Texture2D>("Sprites/Player/IdleLeft"), game.Content.Load<Texture2D>("Sprites/Player/IdleRight"), 18);
            walking = new(game.Content.Load<Texture2D>("Sprites/Player/WalkingLeft"), game.Content.Load<Texture2D>("Sprites/Player/WalkingRight"), 8);
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
            playerAttacking.Draw(spriteBatch);

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
                camera.Follow(new(new(hitBox.Left, hitBox.Bottom - 32), new(32, 32)), game.mapManager.ActiveMap);


            input.CheckPlayerInput();
            game.mapManager.ActiveMap.IsNextToWall(this);
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

            if (Size.Y < OldSize.Y)
                Location += new Point(0, yDiff);
            if (Size.Y > OldSize.Y)
                Location -= new Point(0, yDiff);

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

        public void OnHit()
        {
            health--;
            CurrentAction = Action.Stunned;
            playerMovement.HorizontalSpeed = ((int)Facing) * -7;
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
            CurrentAction = Action.Standing;
            playerMovement.Speed = Vector2.Zero;
        }
    }
}
