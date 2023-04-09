﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiledSharp;

namespace ShadowsOfTomorrow
{
    public class Map : IUpdateAndDraw
    {
        public Point Size {  get => size; }
        public float Top { get => _top; set => _top = value; }
        public float Bottom { get => _bottom; set => _bottom = value; }
        public float Left { get => _left; set => _left = value; }
        public float Right { get => _right; set => _right = value; }
        public string MapName { get; private set; }

        public TmxMap TmxMap => map;

        private float _top;
        private float _bottom;
        private float _left;
        private float _right;

        private readonly TmxMap map;
        private readonly Texture2D tileSet;

        private readonly Game1 game;
        private readonly MapManager mapManager;
        private readonly Point size = Point.Zero;
        private readonly Point tileAmount = Point.Zero;
        public List<Point> destroyedTiles = new();

        public Map(Game1 game, string mapName, MapManager mapManager) 
        {
            map = new($"Content/TileSets/{mapName}.tmx");
            tileSet = game.Content.Load<Texture2D>("TileSets/" + map.Tilesets[0].Name.ToString());

            size.X = map.Tilesets[0].TileWidth;
            size.Y = map.Tilesets[0].TileHeight;

            tileAmount.X = tileSet.Width / size.X;
            tileAmount.Y = tileSet.Height / size.Y;

            Top = 0;
            Left = 0;
            Right = map.Width * size.X;
            Bottom = map.Height * size.Y;

            MapName = mapName;
            this.game = game;
            this.mapManager = mapManager;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (TmxLayer layer in map.Layers)
                for (var i = 0; i < layer.Tiles.Count; i++)
                    if (layer.Tiles[i].Gid != 0)
                    {
                        
                        int tileFrame = layer.Tiles[i].Gid - 1;
                        int column = tileFrame % tileAmount.X;
                        int row = (int)Math.Floor((double)tileFrame / (double)tileAmount.X);

                        float x = (i % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                        Rectangle tilesetRec = new(size.X * column, size.Y * row, size.X, size.Y);

                        if (layer.Name != "DestroyableTiles" || !TileIsDestroyed(layer.Tiles[i]))
                            spriteBatch.Draw(tileSet, new Rectangle((int)x, (int)y, size.X, size.Y), tilesetRec, Color.White);
                    }
        }

        public void Update(GameTime gameTime)
        {
            CheckIfColliedesWithDoor();
        }

        private bool TileIsDestroyed(TmxLayerTile tile)
        {
            foreach (Point point in destroyedTiles)
                if (point == new Point(tile.X, tile.Y) * Size)
                    return true;
            return false;
        }

        private void CheckIfColliedesWithDoor()
        {
            TmxObjectGroup group = map.ObjectGroups.First(group => group.Name.ToLower() == "doors");
            Dictionary<Rectangle, TmxObject> doors = new();

            foreach (TmxObject obj in group.Objects)
                doors.Add(new((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height), obj);

            foreach (KeyValuePair<Rectangle, TmxObject> pair in doors)
                if (game.player.HitBox.Intersects(pair.Key))
                    mapManager.GoToSpawnPoint(pair.Value.Name.Split('-')[1]);
        }

        public (bool, bool) WillCollide(Player player)
        {
            bool canMoveX = true, canMoveY = true;
            List<TmxLayer> collisionLayers = GetCollisionLayers();
            foreach (TmxLayer layer in collisionLayers)
                foreach (var tile in layer.Tiles)
                    if (!destroyedTiles.Contains(new(tile.X * size.X, tile.Y * size.Y)))
                    {
                        Rectangle rec = new(new(tile.X * size.X, tile.Y * size.Y), size);
                        if (player.NextHorizontalHitBox.Intersects(rec) && tile.Gid != 0)
                        {
                            canMoveX = false;
                        }
                        if (player.NextVerticalHitBox.Intersects(rec) && tile.Gid != 0)
                        {
                            if (IsGroundPoundingBlock(player.NextVerticalHitBox) && player.CurrentAction == Action.GroundPounding)
                                RemoveTiles(player.NextVerticalHitBox);
                            else
                            {
                                canMoveY = false;
                                player.isGrounded = true;
                                player.playerMovement.VerticalSpeed = 0;
                                break;
                            }
                        }
                        else if (player.playerMovement.VerticalSpeed != 0)
                            player.isGrounded = false;
                    }
            return (canMoveX, canMoveY);
        }

        private (bool, bool) CheckCollision(TmxLayerTile tile, Player player)
        {
            bool canMoveX = true, canMoveY = true;
            
            return (canMoveX, canMoveY);
        }

        private void RemoveTiles(Rectangle rectangle)
        {
            foreach (TmxLayerTile tile in map.Layers["DestroyableTiles"].Tiles)
                if (!destroyedTiles.Contains(new(tile.X * Size.X, tile.Y * Size.Y)))
                    if (rectangle.Intersects(new(new(tile.X * Size.X, tile.Y * Size.Y), Size)) && tile.Gid != 0)
                        destroyedTiles.Add(new(tile.X * Size.X, tile.Y * Size.Y));
        }

        private bool IsGroundPoundingBlock(Rectangle rectangle)
        {
            TmxLayer layer = map.Layers["DestroyableTiles"];

            foreach (TmxLayerTile tile in layer.Tiles)
                if (!destroyedTiles.Contains(new(tile.X * Size.X, tile.Y * Size.Y)))
                    if (rectangle.Intersects(new(new(tile.X * Size.X, tile.Y * Size.Y), Size)) && tile.Gid != 0)
                        return true;
            return false;
        }

        public List<TmxLayer> GetCollisionLayers()
        {
            List<TmxLayer> platformLayer = new();

            foreach (TmxLayer layer in map.Layers)
                if (layer.Name.ToLower() == "platforms" || layer.Name.ToLower() == "destroyabletiles" || layer.Name.ToLower() == "walls")
                    platformLayer.Add(layer);

            return platformLayer;
        }
    }
}
