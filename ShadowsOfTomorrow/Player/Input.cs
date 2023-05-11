using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Input
    {
        private readonly Player player;

        KeyboardState oldState = Keyboard.GetState();

        public Input(Player player)
        {
            this.player = player;
        }

        public void CheckPlayerInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (player.CurrentAction == Action.Talking || player.CurrentAction == Action.WachingCutScene || player.OldAction == Action.Paused)
                return;

            if (keyboardState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape) && player.OldAction != Action.Talking)
            {
                player.CurrentAction = Action.Paused;
                return;
            }

            switch (player.ActiveMach)
            {
                case Mach.Running or Mach.Sprinting:
                    if (keyboardState.IsKeyDown(player.Keybinds.CrouchKey) && player.isGrounded)
                        player.CurrentAction = Action.Rolling;
                    else if (player.OldAction == Action.Rolling && keyboardState.IsKeyUp(player.Keybinds.CrouchKey) && !player.HaveBlockOverHead(player.HitBox))
                        StandUp();
                    if (keyboardState.IsKeyDown(player.Keybinds.JumpKey) && oldState.IsKeyUp(player.Keybinds.JumpKey))
                        player.playerMovement.Jump();
                    if (keyboardState.IsKeyUp(player.Keybinds.CrouchKey) && player.isGrounded && player.OldAction == Action.Rolling && player.HaveBlockOverHead(player.HitBox))
                        player.CurrentAction = Action.Crouching;
                    if (keyboardState.IsKeyDown(player.Keybinds.CrouchKey) && !player.isGrounded && player.CurrentAction != Action.Stunned && player.CurrentAction != Action.Rolling)
                        player.playerMovement.GroundPound();
                    else if (player.isGrounded && player.CurrentAction == Action.GroundPounding)
                        StandUp();
                    break;
                case Mach.Standing or Mach.Walking:
                    if (keyboardState.IsKeyDown(player.Keybinds.CrouchKey) && player.isGrounded && player.OldAction == Action.Standing)
                        player.CurrentAction = Action.Crouching;
                    else if (player.OldAction == Action.Crouching && keyboardState.IsKeyUp(player.Keybinds.CrouchKey) && !player.HaveBlockOverHead(player.HitBox))
                        StandUp();
                    if (keyboardState.IsKeyDown(player.Keybinds.JumpKey) && oldState.IsKeyUp(player.Keybinds.JumpKey))
                        player.playerMovement.Jump();
                    if (keyboardState.IsKeyDown(player.Keybinds.CrouchKey) && !player.isGrounded && player.CurrentAction != Action.Stunned)
                        player.playerMovement.GroundPound();
                    else if (player.isGrounded && player.CurrentAction == Action.GroundPounding)
                        StandUp();
                    break;
            }

            if (keyboardState.IsKeyDown(player.Keybinds.AttackKey) && oldState.IsKeyUp(player.Keybinds.AttackKey) && player.CurrentAction != Action.GroundPounding && player.CurrentAction != Action.Turning)
                player.playerAttacking.Attack(gameTime);
            
            oldState = keyboardState;
        }

        private void StandUp()
        {
            if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching && player.OldAction != Action.GroundPounding)
                return;
            player.CurrentAction = Action.Standing;
        }
    }
}
