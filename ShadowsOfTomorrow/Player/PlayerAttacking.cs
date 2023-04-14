﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace ShadowsOfTomorrow
{
    public class PlayerAttacking : IUpdateAndDraw
    {
        Texture2D texture;
        
        private readonly Player player;
        private readonly Game1 game;

        public Rectangle Hitbox { get => _hitbox; }
        private Rectangle _hitbox;

        private double timeSinceAttack = 0;
        private const double interval = 0.4;

        public PlayerAttacking(Player player, Game1 game)
        {
            this.player = player;
            this.game = game;
            _hitbox = new(player.Location + new Point(player.Size.X, 0), new(30,30));
            texture = game.Content.Load<Texture2D>("Box");
        }

        public void Attack()
        {
            player.CurrentAction = Action.Attacking;

            CheckIfHittingTiles();
            CheckIfHittingBoss();
        }

        private void CheckIfHittingBoss()
        {
            if (game.mapManager.ActiveMap.boss == null)
                return;

            if (game.mapManager.ActiveMap.boss.HitBox.Intersects(Hitbox))
                game.mapManager.ActiveMap.boss.OnHit();

        }

        private void CheckIfHittingTiles()
        {
            Map map = game.mapManager.ActiveMap;
            if (!map.TmxMap.Layers.Contains("DestroyableTiles"))
                return;
            TmxLayer layer = map.TmxMap.Layers["DestroyableTiles"];

            foreach (TmxLayerTile tile in layer.Tiles)
            {
                if (!map.destroyedTiles.Contains(new(tile.X * map.Size.X, tile.Y * map.Size.Y)))
                    if (Hitbox.Intersects(new(new(tile.X * map.Size.X, tile.Y * map.Size.Y), map.Size)) && tile.Gid != 0)
                        map.destroyedTiles.Add(new(tile.X * map.Size.X, tile.Y * map.Size.Y));
            }
        }

        public void Update(GameTime gameTime)
        {
            if (player.CurrentAction == Action.Attacking && player.OldAction != Action.Attacking)
                timeSinceAttack = gameTime.TotalGameTime.TotalSeconds;

            if (timeSinceAttack + interval < gameTime.TotalGameTime.TotalSeconds && player.CurrentAction == Action.Attacking)
                player.CurrentAction = Action.Standing;


            if (player.Facing == Facing.Right)
            {
                _hitbox = new(player.Location + new Point(player.Size.X, 10), new(30, 30));
                return;
            }
            _hitbox = new(player.Location + new Point(-30, 10), new(30, 30));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Hitbox, Color.White);
        }
    }
}
