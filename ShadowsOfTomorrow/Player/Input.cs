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

        public void CheckPlayerInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (player.CurrentAction == Action.Talking || player.CurrentAction == Action.WachingCutScene)
                return;

            switch (player.ActiveMach)
            {
                case Mach.Running or Mach.Sprinting:
                    if (keyboardState.IsKeyDown(Keys.S) && player.isGrounded)
                        player.CurrentAction = Action.Rolling;
                    else if (player.OldAction == Action.Rolling && keyboardState.IsKeyUp(Keys.S))
                        StandUp();
                    if (keyboardState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        player.playerMovement.Jump();
                    break;
                case Mach.Standing or Mach.Walking:
                    if (keyboardState.IsKeyDown(Keys.S) && player.isGrounded && player.OldAction == Action.Standing)
                        player.CurrentAction = Action.Crouching;
                    else if (player.OldAction == Action.Crouching && keyboardState.IsKeyUp(Keys.S))
                        StandUp();
                    if (keyboardState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                        player.playerMovement.Jump();
                    if (keyboardState.IsKeyDown(Keys.S) && !player.isGrounded && player.CurrentAction != Action.Crouching)
                        player.playerMovement.GroundPound();
                    else if (player.isGrounded && player.CurrentAction == Action.GroundPounding)
                        player.CurrentAction = Action.Standing;
                    break;
            }

            if (keyboardState.IsKeyDown(Keys.J) && oldState.IsKeyUp(Keys.J) && player.CurrentAction != Action.GroundPounding && player.CurrentAction != Action.Turning)
                player.playerAttacking.Attack();
            
            oldState = keyboardState;
        }

        private void StandUp()
        {
            if (player.OldAction != Action.Rolling && player.OldAction != Action.Crouching)
                return;
            player.CurrentAction = Action.Standing;
        }
    }
}
