using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace ShadowsOfTomorrow
{
    public class MapManager : IUpdateAndDraw
    {
        public Map ActiveMap { get => Maps[ActiveMapIndex]; }
        public List<Map> Maps => _maps;

        public int ActiveMapIndex { get => _activeMapIndex; private set => _activeMapIndex = value; }

        private readonly List<Map> _maps = new();
        private readonly Game1 game;
        private int _activeMapIndex;

        public MapManager(Game1 game) 
        {
            this.game = game;
        }

        public void Add(Map map)
        {
            Maps.Add(map);
        }

        public void SetActiveMapTo(int i, TmxObject spawnPoint)
        {
            ActiveMapIndex = i;
        }

        public void SetActiveMapTo(string name, TmxObject spawnPoint)
        {
            ActiveMapIndex = Maps.FindIndex(map => map.MapName == name);
            game.player.Location = new ((int)spawnPoint.X, (int)spawnPoint.Y);
        }

        public void Update(GameTime gameTime)
        {
            ActiveMap.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ActiveMap.Draw(spriteBatch);
        }

        public void GoToSpawnPoint(string spawnpoint)
        {
            foreach (Map map in Maps)
            {
                TmxObjectGroup spawnpoints = map.TmxMap.ObjectGroups.First(group => group.Name.ToLower() == "spawnpoints");

                foreach (TmxObject obj in spawnpoints.Objects)
                    if (obj.Name == spawnpoint)
                        SetActiveMapTo(map.MapName, obj);
            }
        }
    }
}
