using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShadowsOfTomorrow
{
    public class PlayerMovement
    {
        public Vector2 Speed { get => _speed; set => _speed = value; }
        public float VerticalSpeed { get => _speed.Y; set => _speed.Y = value; }
        public float HorisontalSpeed { get => _speed.X; set => _speed.X = value; }

        private readonly Player player;
        private readonly Game1 game;

        public const int walkingSpeed = 5;
        public const int runningSpeed = 10;
        public const int sprintingSpeed = 15;
        private const int maxYSpeed = 10;
        private const int jumpForce = -10;
        private const float brakeSpeed = 0.2f;
        private const float acceleration = 0.25f;
        private const float gravitation = 0.4f;
        private const float groundPoundSpeed = 12.0f;
        private const float groundPoundAcceleration = 1.0f;

        private Vector2 _speed = Vector2.Zero;
        private KeyboardState oldState = Keyboard.GetState();
        private float speedBeforeTurn;

        public PlayerMovement(Player player, Game1 game) 
        {
            this.player = player;
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            CheckPlayerInput();
            CheckPlayerAction();
            MovePlayer();

            oldState = Keyboard.GetState();
        }

        private void CheckPlayerInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            switch (player.ActiveMach)
            {
                case Mach.Running or Mach.Sprinting:
                    if (keyboardState.IsKeyDown(Keys.S) && player.isGrounded)
                        player.CurrentAction = Action.Rolling;
                    else if (player.OldAction == Action.Rolling && keyboardState.IsKeyUp(Keys.S))
                        StandUp();
                    if (keyboardState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        Jump();
                    break;
                case Mach.Standing or Mach.Walking:
                    if (keyboardState.IsKeyDown(Keys.S) && player.isGrounded && player.OldAction == Action.Standing)
                        player.CurrentAction = Action.Crouching;
                    else if (player.OldAction == Action.Crouching && keyboardState.IsKeyUp(Keys.S))
                        StandUp();
                    if (keyboardState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        Jump();
                    if (keyboardState.IsKeyDown(Keys.S) && !player.isGrounded && player.CurrentAction != Action.Crouching)
                        GroundPound();
                    else if (player.isGrounded && player.CurrentAction == Action.GroundPounding)
                        player.CurrentAction = Action.Standing;
                    break;
            }

            if (keyboardState.IsKeyDown(Keys.J) && oldState.IsKeyUp(Keys.J) && player.CurrentAction != Action.GroundPounding && player.CurrentAction != Action.Turning)
                player.playerAttacking.Attack();

            UpdateSpeed(keyboardState);
        }

        private void CheckPlayerAction()
        {
            switch (player.CurrentAction)
            {
                case Action.Crouching:
                    Crouch();
                    break;
                case Action.Attacking:
                    break;
                case Action.Turning:
                    StandUp();
                    Turn();
                    break;
                case Action.Rolling:
                    if (HorisontalSpeed <= 5 && HorisontalSpeed >= -5)
                        StandUp();
                    break;
                default:
                    break;
            }
        }

        private void Turn()
        {
            if (speedBeforeTurn < 0)
            {
                HorisontalSpeed += brakeSpeed;
                if (HorisontalSpeed >= 0)
                {
                    if (player.isGrounded)
                    {
                        
                        HorisontalSpeed = -1 * speedBeforeTurn;
                        StandUp();
                        if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching)
                            player.CurrentAction = Action.Standing;
                        if (speedBeforeTurn < -walkingSpeed)
                            player.ActiveMach = Mach.Running;
                        if (speedBeforeTurn < -runningSpeed)
                            player.ActiveMach = Mach.Sprinting;
                        return;
                    }
                    HorisontalSpeed = 0;
                }
                return;
            }
            HorisontalSpeed -= brakeSpeed;
            if (HorisontalSpeed <= 0)
            {
                if (player.isGrounded)
                {
                    HorisontalSpeed = -1 * speedBeforeTurn;
                    StandUp();
                    if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching)
                        player.CurrentAction = Action.Standing;
                    player.ActiveMach = Mach.Sprinting;
                    if (speedBeforeTurn > walkingSpeed)
                        player.ActiveMach = Mach.Running;
                    if (speedBeforeTurn > runningSpeed)
                        player.ActiveMach = Mach.Sprinting;
                    return;
                }
                HorisontalSpeed = 0;
            }
        }

        private void GroundPound()
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
            if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching)
                return;
            player.CurrentAction = Action.Standing;
        }

        private void Jump()
        {
            VerticalSpeed = jumpForce;
        }

        private void UpdateSpeed(KeyboardState state)
        {
            if (player.CurrentAction == Action.Turning)
                return;
            switch (player.ActiveMach)
            {
                case Mach.Running:
                    if (state.IsKeyDown(Keys.A) && HorisontalSpeed > 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorisontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.A) && HorisontalSpeed >= -runningSpeed)
                        HorisontalSpeed -= acceleration / 4;
                    if (state.IsKeyDown(Keys.D) && HorisontalSpeed < 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorisontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.D) && HorisontalSpeed <= runningSpeed)
                        HorisontalSpeed += acceleration / 4;
                    break;
                case Mach.Sprinting:
                    if (state.IsKeyDown(Keys.A) && HorisontalSpeed > 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorisontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.A) && HorisontalSpeed >= -sprintingSpeed)
                        HorisontalSpeed -= acceleration / 8;
                    if (state.IsKeyDown(Keys.D) && HorisontalSpeed < 0 && player.CurrentAction != Action.Turning)
                    {
                        player.CurrentAction = Action.Turning;
                        speedBeforeTurn = HorisontalSpeed;
                    }
                    else if (state.IsKeyDown(Keys.D) && HorisontalSpeed <= sprintingSpeed)
                        HorisontalSpeed += acceleration / 8;
                    break;
                case Mach.Standing or Mach.Walking:
                    if (state.IsKeyDown(Keys.A) && HorisontalSpeed >= -walkingSpeed)
                        HorisontalSpeed -= acceleration;
                    if (state.IsKeyDown(Keys.D) && HorisontalSpeed <= walkingSpeed)
                        HorisontalSpeed += acceleration;
                    break;
            }
            WillSlowDown();
        }

        private void WillSlowDown()
        {
            if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D))
            {
                if (HorisontalSpeed < 0)
                    HorisontalSpeed += brakeSpeed;
                if (HorisontalSpeed > 0)
                    HorisontalSpeed -= brakeSpeed;
                if (-brakeSpeed < HorisontalSpeed && HorisontalSpeed < brakeSpeed)
                    HorisontalSpeed = 0;
            }
        }

        private void MovePlayer()
        {
            if (player.CurrentAction == Action.GroundPounding && VerticalSpeed < groundPoundSpeed)
                VerticalSpeed += groundPoundAcceleration;
            else if (VerticalSpeed < maxYSpeed)
                VerticalSpeed += gravitation;

            (bool canMoveX, bool canMoveY) = game.mapManager.ActiveMap.WillCollide(player);

            if (canMoveX && player.CurrentAction != Action.GroundPounding)
                player.Location += new Point((int)HorisontalSpeed, 0);
            else
                HorisontalSpeed = 0;

            if (canMoveY)
                player.Location += new Point(0, (int)VerticalSpeed);
            else
                VerticalSpeed = 1;
        }
    }
}
