﻿using Microsoft.Xna.Framework.Input;
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
        private readonly Game1 game;

        KeyboardState oldState = Keyboard.GetState();

        public Input(Player player, Game1 game)
        {
            this.player = player;
            this.game = game;
        }

        public void CheckPlayerInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (player.CurrentAction == Action.Talking)
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
            if (game.mapManager.ActiveMap.boss != null && player.HitBox.Intersects(game.mapManager.ActiveMap.boss.HitBox) && Keyboard.GetState().IsKeyDown(Keys.K))
            {
                game.windowManager.SetDialogue(game.mapManager.ActiveMap.boss.Dialogue);
                player.CurrentAction = Action.Talking;
            }
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
