using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace ShadowsOfTomorrow
{
    public class Map : IUpdateAndDraw
    {
        private readonly TmxMap map;
        private readonly Texture2D tileSet;

        private readonly Point size = Point.Zero;
        private readonly Point tileAmount = Point.Zero;
        

        public Map(Game1 game) 
        {
            map = new("Content/TileSets/StartMap.tmx");
            tileSet = game.Content.Load<Texture2D>("TileSets/" + map.Tilesets[0].Name.ToString());

            size.X = map.Tilesets[0].TileWidth;
            size.Y = map.Tilesets[0].TileHeight;

            tileAmount.X = tileSet.Width / size.X;
            tileAmount.Y = tileSet.Height / size.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (TmxLayer layer in map.Layers)
            {
                for (var i = 0; i < layer.Tiles.Count; i++)
                {
                    int gid = layer.Tiles[i].Gid;

                    if (gid != 0)
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % tileAmount.X;
                        int row = (int)Math.Floor((double)tileFrame / (double)tileAmount.X);

                        float x = (i % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                        Rectangle tilesetRec = new Rectangle(size.X * column, size.Y * row, size.X, size.Y);

                        spriteBatch.Draw(tileSet, new Rectangle((int)x, (int)y, size.X, size.Y), tilesetRec, Color.White);
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {


        }


        public (bool, bool) WillCollide(Player player)
        {
            bool canMoveX = true, canMoveY = true;

            TmxLayer platformLayer = GetPlatformLayer();

            for (int i = 0; i < platformLayer.Tiles.Count; i++)
            {
                Rectangle tile = new(new(platformLayer.Tiles[i].X * size.X, platformLayer.Tiles[i].Y * size.Y), size);
                if (new Rectangle(player.Location + new Point((int)player.Speed.X, 0), player.Size).Intersects(tile) && platformLayer.Tiles[i].Gid != 0)
                {
                    canMoveX = false;
                    break;
                }
                if (tile.Intersects(new(player.Location + new Point(0, (int)player.Speed.Y), player.Size)) && platformLayer.Tiles[i].Gid != 0)
                {
                    canMoveY = false;
                    break;
                }
            }
            return (canMoveX, canMoveY);
        }

        private TmxLayer GetPlatformLayer()
        {
            TmxLayer platformLayer = null;

            foreach (TmxLayer layer in map.Layers)
                if (layer.Name.ToLower() == "platforms")
                    platformLayer = layer;

            if (platformLayer == null)
                throw new Exception("There was no layer with the name platforms in the tmx file");

            return platformLayer;
        }
    }
}
