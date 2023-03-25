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
    internal class Map
    {

        public Map(Game1 game) 
        {
            TmxMap tmxMap = new("Content/TileSets/StartMap.tmx");
            Texture2D tileSet = game.Content.Load<Texture2D>("TileSets/" + tmxMap.Tilesets[0].Name.ToString());


        }
    }
}
