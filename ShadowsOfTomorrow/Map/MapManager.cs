﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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
        public List<Map> Maps => _maps.Keys.ToList();

        public int ActiveMapIndex { get; private set; }

        private readonly Dictionary<Map, List<BackgroundLayer>> _maps = new();
        private readonly Game1 game;

        public MapManager(Game1 game) 
        {
            this.game = game;
        }

        public void AddMaps()
        {
            List<BackgroundLayer> layers = new()
            {
                new(game, 0.0f, 0.0f, new(0, - 4 * 48), "Sky_x3"),
                new(game, 0.1f, 2.5f, new(0, 18 * 48), "FarMontains_x3"),
                new(game, 0.15f, 3.5f, new(0, 18 * 48 + 24), "CloseMountains_x3"),
                new(game, 0.25f, 5.0f, new(0, 20 * 48), "Grass_x3"),
                new(game, 0.4f, 0f, new(0, 9 * 48), "Clouds_x3", -30f)
            };
            _maps.Add(new(game, "LandingSite", this), layers.ToList());
            _maps.Add(new(game, "LearnControllsMap", this), layers.ToList());
            _maps.Add(new(game, "CrashSite", this), layers.ToList());
            _maps.Add(new(game, "LearnMelee", this), layers.ToList());
            _maps.Add(new(game, "RunFromBranches", this), layers.ToList());
            _maps.Add(new(game, "PlantCity", this), layers.ToList());
            _maps.Add(new(game, "BossRoom", this), layers.ToList());
        }

        public void SetActiveMapTo(string name, TmxObject spawnPoint)
        {
            ActiveMapIndex = Maps.FindIndex(map => map.MapName == name);
            game.Player.Location = new ((int)spawnPoint.X, (int)spawnPoint.Y);
        }

        public void Update(GameTime gameTime)
        {
            ActiveMap.Update(gameTime);
            foreach (var backgroundLayer in _maps[ActiveMap].ToList())
                backgroundLayer.Update(gameTime);

            switch (ActiveMapIndex)
            {
                case 0 or 1:
                    game.MusicManager.Play(game.Content.Load<Song>("Music/Mondays"));
                    break;
                case 2 or 3:
                    game.MusicManager.Play(game.Content.Load<Song>("Music/UnearthlyBlues"));
                    break;
                case 4:
                    if (ActiveMap.branchCutScene.HaveEnded)
                        game.MusicManager.Play(game.Content.Load<Song>("Music/ItsPizzaTime"));
                    else
                        game.MusicManager.Play(game.Content.Load<Song>("Music/Meatophobia"));
                    break;
                case 5:
                    game.MusicManager.Play(game.Content.Load<Song>("Music/HoppinOutdoors"));
                    break;
                case 6:
                    game.MusicManager.Play(game.Content.Load<Song>("Music/TheDeathIDeservioli"));
                    break;

                default:
                    game.MusicManager.Stop();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ActiveMap.Draw(spriteBatch);
            foreach (var backgroundLayer in _maps[ActiveMap].ToList())
                backgroundLayer.Draw(spriteBatch);
        }

        public void GoToSpawnPoint(int spawnpoint)
        {
            if (spawnpoint == 13)
            {
                if (!game.WindowManager.dialogueWindow.haveGivenInformation && game.Player.LastSpawnPoint != 0 && game.Player.Health > 0)
                    return;

                game.WindowManager.SetDialogue(Maps.First(map => map.MapName == "BossRoom").boss.Dialogue);
                Maps.First(map => map.MapName == "BossRoom").boss.Reset();
                game.Player.CurrentAction = Action.Talking;
            }

            if (spawnpoint == 0)
            {
                game.Player.CurrentAction = Action.InMainMenu;
                return;
            }

            game.Player.LastSpawnPoint = spawnpoint;
            foreach (Map map in Maps)
            {
                TmxObjectGroup spawnpoints = map.TmxMap.ObjectGroups.First(group => group.Name.ToLower() == "spawnpoints");

                foreach (TmxObject obj in spawnpoints.Objects)
                    if (obj.Name == spawnpoint.ToString())
                    {
                        SetActiveMapTo(map.MapName, obj);
                        map.Reset();
                    }
            }
        }
    }
}
