using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShadowsOfTomorrow
{
    public class PlayerMovement
    {
        public Vector2 Speed { get => _speed; set => _speed = value; }
        public float VerticalSpeed { get => _speed.Y; set => _speed.Y = value; }
        public float HorizontalSpeed { get => _speed.X; set => _speed.X = value; }

        private readonly Player player;
        private readonly Game1 game;

        public const int walkingSpeed = 6;
        public const int runningSpeed = 10;
        public const int sprintingSpeed = 13;
        private const int maxYSpeed = 10;
        private const int jumpForce = -12;
        private const float brakeSpeed = 0.2f;
        private const float acceleration = 0.25f;
        private const float gravitation = 0.6f;
        private const float groundPoundSpeed = 14.0f;
        private const float groundPoundAcceleration = 1.0f;
        private const float crouchingSpeed = 3.0f;

        Facing facingWall;
        float wallClimbingSpeed;
        float speedBeforeWallClimb;

        private Vector2 _speed = Vector2.Zero;
        private float speedBeforeTurn;

        public PlayerMovement(Player player, Game1 game) 
        {
            this.player = player;
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            UpdateSpeed(Keyboard.GetState());
            CheckPlayerAction();
            MovePlayer(gameTime);
        }

        private void CheckPlayerAction()
        {
            switch (player.CurrentAction)
            {
                case Action.Crouching:
                    Crouch();
                    break;
                case Action.WallClimbing:
                    ClimbWall();
                    break;
                case Action.Stunned:
                    if (player.isGrounded)
                        StandUp();
                    break;
                case Action.Turning:
                    StandUp();
                    Turn();
                    break;
                case Action.Rolling:
                    if (HorizontalSpeed <= walkingSpeed && HorizontalSpeed >= -walkingSpeed)
                        StandUp();
                    break;
            }
        }

        private void Turn()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(player.Keybinds.RightKey) && state.IsKeyDown(player.Keybinds.LeftKey))
            {
                player.CurrentAction = Action.Standing;
                return;
            }

            if (speedBeforeTurn < 0 && state.IsKeyDown(player.Keybinds.RightKey) && state.IsKeyUp(player.Keybinds.LeftKey))
            {
                HorizontalSpeed += brakeSpeed;
                if (HorizontalSpeed >= 0 && player.isGrounded)
                {
                    HorizontalSpeed = -1 * speedBeforeTurn;
                    StandUp();
                    if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching)
                        player.CurrentAction = Action.Standing;
                    if (speedBeforeTurn < -walkingSpeed)
                        player.ActiveMach = Mach.Running;
                    if (speedBeforeTurn < -runningSpeed)
                        player.ActiveMach = Mach.Sprinting;
                    return;
                }
                else if (HorizontalSpeed >= 0 && !player.isGrounded)
                    HorizontalSpeed = 0;
                return;
            }

            if (speedBeforeTurn > 0 && state.IsKeyDown(player.Keybinds.LeftKey) && state.IsKeyUp(player.Keybinds.RightKey))
            {
                HorizontalSpeed -= brakeSpeed;
                if (HorizontalSpeed <= 0 && player.isGrounded)
                {
                    HorizontalSpeed = -1 * speedBeforeTurn;
                    StandUp();
                    if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching)
                        player.CurrentAction = Action.Standing;
                    if (speedBeforeTurn > walkingSpeed)
                        player.ActiveMach = Mach.Running;
                    if (speedBeforeTurn > runningSpeed)
                        player.ActiveMach = Mach.Sprinting;
                    return;
                }
                else if (HorizontalSpeed <= 0 && !player.isGrounded)
                    HorizontalSpeed = 0;
            }

            if (speedBeforeTurn < 0 && state.IsKeyUp(player.Keybinds.RightKey))
                player.CurrentAction = Action.Standing;
            if (speedBeforeTurn > 0 && state.IsKeyUp(player.Keybinds.LeftKey))
                player.CurrentAction = Action.Standing;
        }

        public void GroundPound()
        {
            if (player.CurrentAction == Action.GroundPounding)
                return;
            player.CurrentAction = Action.GroundPounding;
            VerticalSpeed = 0;
        }

        private void Crouch()
        {
            if (HorizontalSpeed > crouchingSpeed)
                HorizontalSpeed = crouchingSpeed;
            
            if (HorizontalSpeed < -crouchingSpeed)
                HorizontalSpeed = -crouchingSpeed;
        }

        public void StandUp()
        {
            if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching && player.CurrentAction != Action.WallClimbing && player.CurrentAction != Action.Stunned)
                return;
            player.CurrentAction = Action.Standing;
        }

        public void Jump()
        {
            if (player.isGrounded)
                VerticalSpeed = jumpForce;
        }

        private void ClimbWall()
        {
            VerticalSpeed = wallClimbingSpeed;
        }

        private void UpdateSpeed(KeyboardState state)
        {
            if (player.CurrentAction == Action.Talking || player.CurrentAction == Action.WachingCutScene)
                goto SlowDown;
            if (player.CurrentAction == Action.Turning || player.isNextToWall || player.CurrentAction == Action.Stunned)
                return;
            switch (player.ActiveMach)
            {
                case Mach.Running:
                    if (state.IsKeyDown(player.Keybinds.LeftKey) && HorizontalSpeed > 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(player.Keybinds.LeftKey) && HorizontalSpeed >= -runningSpeed)
                        HorizontalSpeed -= acceleration / 4;
                    if (state.IsKeyDown(player.Keybinds.RightKey) && HorizontalSpeed < 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(player.Keybinds.RightKey) && HorizontalSpeed <= runningSpeed)
                        HorizontalSpeed += acceleration / 4;
                    break;
                case Mach.Sprinting:
                    if (state.IsKeyDown(player.Keybinds.LeftKey) && HorizontalSpeed > 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(player.Keybinds.LeftKey) && HorizontalSpeed >= -sprintingSpeed)
                        HorizontalSpeed -= acceleration / 8;
                    if (state.IsKeyDown(player.Keybinds.RightKey) && HorizontalSpeed < 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(player.Keybinds.RightKey) && HorizontalSpeed <= sprintingSpeed)
                        HorizontalSpeed += acceleration / 8;
                    break;
                case Mach.Standing or Mach.Walking:
                    if (state.IsKeyDown(player.Keybinds.LeftKey) && HorizontalSpeed >= -walkingSpeed)
                        HorizontalSpeed -= acceleration;
                    if (state.IsKeyDown(player.Keybinds.RightKey) && HorizontalSpeed <= walkingSpeed)
                        HorizontalSpeed += acceleration;
                    break;
            }
            SlowDown:
                WillSlowDown();
        }

        private void WillSlowDown()
        {
            if (Keyboard.GetState().IsKeyUp(player.Keybinds.LeftKey) && Keyboard.GetState().IsKeyUp(player.Keybinds.RightKey) || player.CurrentAction == Action.Talking || player.CurrentAction == Action.WachingCutScene)
            {
                if (HorizontalSpeed < 0)
                    HorizontalSpeed += brakeSpeed;
                if (HorizontalSpeed > 0)
                    HorizontalSpeed -= brakeSpeed;
                if (-brakeSpeed < HorizontalSpeed && HorizontalSpeed < brakeSpeed)
                    HorizontalSpeed = 0;
            }
        }

        private void MovePlayer(GameTime gameTime)
        {
            if (player.CurrentAction == Action.GroundPounding && VerticalSpeed < groundPoundSpeed)
                VerticalSpeed += groundPoundAcceleration;
            else if (VerticalSpeed < maxYSpeed)
                VerticalSpeed += gravitation;

            if (player.isNextToWall && player.ActiveMach == Mach.Walking)
                HorizontalSpeed = 0;

            (bool canMoveX, bool canMoveY) = game.mapManager.ActiveMap.WillCollide(player, gameTime);

            CheckForWallClimb(canMoveX, canMoveY);

            if (canMoveX && player.CurrentAction != Action.GroundPounding && player.CurrentAction != Action.WallClimbing)
                player.Location += new Point((int)HorizontalSpeed, 0);

            if (canMoveY)
                player.Location += new Point(0, (int)VerticalSpeed);
            else
                VerticalSpeed = 1;
        }

        private void CheckForWallClimb(bool x, bool y)
        {
            if (player.CurrentAction == Action.Turning && player.OldAction != Action.WallClimbing)
                return;

            if (x && player.CurrentAction == Action.WallClimbing && !player.isNextToWall && player.Facing == facingWall)
            {
                HorizontalSpeed = speedBeforeWallClimb;
                VerticalSpeed = -4;
                StandUp();
            }
            else if (x && player.OldAction == Action.WallClimbing && HorizontalSpeed != 0)
            {
                StandUp();
                HorizontalSpeed = -speedBeforeWallClimb;
            }


            if (player.isNextToWall && (player.ActiveMach == Mach.Running || player.ActiveMach == Mach.Sprinting) && player.CurrentAction != Action.WallClimbing) 
                StartWallClimb();

            if (!y && player.CurrentAction == Action.WallClimbing && !player.isGrounded)
            {
                player.CurrentAction = Action.Stunned;
                VerticalSpeed = 5;
                if (facingWall == Facing.Right)
                    HorizontalSpeed = -walkingSpeed;
                else
                    HorizontalSpeed = walkingSpeed;
            }
        }

        private void StartWallClimb()
        {
            player.CurrentAction = Action.WallClimbing;
            facingWall = player.Facing;
            speedBeforeWallClimb = HorizontalSpeed;
            wallClimbingSpeed = - Math.Abs(HorizontalSpeed);
            ClimbWall();
            HorizontalSpeed = 0;
        }
    }
}
