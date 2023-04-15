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
        private const int jumpForce = -10;
        private const float brakeSpeed = 0.2f;
        private const float acceleration = 0.25f;
        private const float gravitation = 0.5f;
        private const float groundPoundSpeed = 12.0f;
        private const float groundPoundAcceleration = 1.0f;

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

        public void Update()
        {
            UpdateSpeed(Keyboard.GetState());
            CheckPlayerAction();
            MovePlayer();
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
            if (state.IsKeyDown(Keys.D) && state.IsKeyDown(Keys.A))
            {
                player.CurrentAction = Action.Standing;
                return;
            }

            if (speedBeforeTurn < 0 && state.IsKeyDown(Keys.D) && state.IsKeyUp(Keys.A))
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

            if (speedBeforeTurn > 0 && state.IsKeyDown(Keys.A) && state.IsKeyUp(Keys.D))
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

            if (speedBeforeTurn < 0 && state.IsKeyUp(Keys.D))
                player.CurrentAction = Action.Standing;
            if (speedBeforeTurn > 0 && state.IsKeyUp(Keys.A))
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
            if (player.OldAction == Action.Crouching || player.OldAction == Action.Rolling)
                return;
        }

        private void StandUp()
        {
            if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching && player.CurrentAction != Action.WallClimbing && player.CurrentAction != Action.Stunned)
                return;
            player.CurrentAction = Action.Standing;
        }

        public void Jump()
        {
            VerticalSpeed = jumpForce;
        }

        private void ClimbWall()
        {
            VerticalSpeed = wallClimbingSpeed;
        }

        private void UpdateSpeed(KeyboardState state)
        {
            if (player.CurrentAction == Action.Talking)
                goto SlowDown;
            if (player.CurrentAction == Action.Turning || game.mapManager.ActiveMap.IsNextToWall(player) || player.CurrentAction == Action.Stunned)
                return;
            switch (player.ActiveMach)
            {
                case Mach.Running:
                    if (state.IsKeyDown(Keys.A) && HorizontalSpeed > 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.A) && HorizontalSpeed >= -runningSpeed)
                        HorizontalSpeed -= acceleration / 4;
                    if (state.IsKeyDown(Keys.D) && HorizontalSpeed < 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.D) && HorizontalSpeed <= runningSpeed)
                        HorizontalSpeed += acceleration / 4;
                    break;
                case Mach.Sprinting:
                    if (state.IsKeyDown(Keys.A) && HorizontalSpeed > 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.A) && HorizontalSpeed >= -sprintingSpeed)
                        HorizontalSpeed -= acceleration / 8;
                    if (state.IsKeyDown(Keys.D) && HorizontalSpeed < 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorizontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.D) && HorizontalSpeed <= sprintingSpeed)
                        HorizontalSpeed += acceleration / 8;
                    break;
                case Mach.Standing or Mach.Walking:
                    if (state.IsKeyDown(Keys.A) && HorizontalSpeed >= -walkingSpeed)
                        HorizontalSpeed -= acceleration;
                    if (state.IsKeyDown(Keys.D) && HorizontalSpeed <= walkingSpeed)
                        HorizontalSpeed += acceleration;
                    break;
            }
            SlowDown:
                WillSlowDown();
        }

        private void WillSlowDown()
        {
            if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D) || player.CurrentAction == Action.Talking)
            {
                if (HorizontalSpeed < 0)
                    HorizontalSpeed += brakeSpeed;
                if (HorizontalSpeed > 0)
                    HorizontalSpeed -= brakeSpeed;
                if (-brakeSpeed < HorizontalSpeed && HorizontalSpeed < brakeSpeed)
                    HorizontalSpeed = 0;
            }
        }

        private void MovePlayer()
        {
            if (player.CurrentAction == Action.GroundPounding && VerticalSpeed < groundPoundSpeed)
                VerticalSpeed += groundPoundAcceleration;
            else if (VerticalSpeed < maxYSpeed)
                VerticalSpeed += gravitation;

            if (game.mapManager.ActiveMap.IsNextToWall(player))
                HorizontalSpeed = 0;

            (bool canMoveX, bool canMoveY) = game.mapManager.ActiveMap.WillCollide(player);

            CheckForWallClimb(canMoveX, canMoveY);

            if (canMoveX && player.CurrentAction != Action.GroundPounding && player.CurrentAction != Action.WallClimbing)
                player.Location += new Point((int)HorizontalSpeed, 0);
            else 
                HorizontalSpeed = 0;

            if (canMoveY)
                player.Location += new Point(0, (int)VerticalSpeed);
            else
                VerticalSpeed = 1;
        }

        private void CheckForWallClimb(bool x, bool y)
        {
            if (player.CurrentAction == Action.Turning)
                return;

            if (x && player.CurrentAction == Action.WallClimbing && !game.mapManager.ActiveMap.IsNextToWall(player) && player.Facing == facingWall)
            {
                HorizontalSpeed = speedBeforeWallClimb;
                VerticalSpeed = -4;
                StandUp();
            }
            else if (x && player.OldAction == Action.WallClimbing && !game.mapManager.ActiveMap.IsNextToWall(player))
            {
                StandUp();
                HorizontalSpeed = -speedBeforeWallClimb;
            }
            if (game.mapManager.ActiveMap.IsNextToWall(player) && (HorizontalSpeed < -PlayerMovement.walkingSpeed - 1 || HorizontalSpeed > PlayerMovement.walkingSpeed + 1) && 
                player.CurrentAction != Action.WallClimbing)
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
        }
    }
}
