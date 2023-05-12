using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiledSharp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        public Boss boss;
        public readonly BranchHuntStart branchCutScene = new();
        public BranchWall branchWall;
        private readonly List<Npc> npcs = new();
        private readonly Dictionary<string, bool> npcNames = new()
        {
            { "Blossom", true},
            { "Marigold", false},
            { "Violet", true}
        };

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
        private readonly SpriteFont font;
        private readonly Texture2D texture;
        public List<Point> destroyedTiles = new();

        public Map(Game1 game, string mapName, MapManager mapManager) 
        {
            map = new($"Content/TileSets/{mapName}.tmx");
            tileSet = game.Content.Load<Texture2D>("TileSets/" + map.Tilesets[0].Name.ToString());
            
            font = game.Content.Load<SpriteFont>("Fonts/DialogueFont");
            texture = game.Content.Load<Texture2D>("UI/DialogueBox_x3");

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

            LoadBranchWall();
            LoadBoss();
            LoadNpcs();
        }

        private void LoadBranchWall()
        {
            if (!MapName.Equals("RunFromBranches"))
                return;
            branchWall = new(game, "UI/DialogueBox_x3", new(0,0), new(1000, 3000));
        }

        private void LoadBoss()
        {
            if (!map.ObjectGroups.Contains("BossSpawnpoint"))
                return;

            boss = new(game, "Treevor", new((int)map.ObjectGroups["BossSpawnpoint"].Objects.First().X, (int)map.ObjectGroups["BossSpawnpoint"].Objects.First().Y));
        }

        private void LoadNpcs()
        {
            if (!map.ObjectGroups.Contains("NpcSpawnPoint"))
                return;

            TmxList<TmxObject> tmxObject = map.ObjectGroups["NpcSpawnPoint"].Objects;

            for (int i = 0; i < tmxObject.Count; i++)
                npcs.Add(new(game, new((int)tmxObject[i].X, (int)tmxObject[i].Y), game.Player, npcNames.ElementAt(i).Key, npcNames.ElementAt(i).Value));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            WriteHelpingText(game.Player.Keybinds, spriteBatch);

            foreach (TmxLayer layer in map.Layers)
                for (var i = 0; i < layer.Tiles.Count; i++)
                    if (layer.Tiles[i].Gid != 0 && layer.Name != "prototype")
                    {
                        
                        int tileFrame = layer.Tiles[i].Gid - 1;
                        int column = tileFrame % tileAmount.X;
                        int row = (int)Math.Floor((double)tileFrame / (double)tileAmount.X);

                        float x = (i % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                        Rectangle tilesetRec = new(size.X * column, size.Y * row, size.X, size.Y);

                        if (layer.Name == "platforms")
                            spriteBatch.Draw(tileSet, new Rectangle((int)x, (int)y, size.X, size.Y), tilesetRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.6f);
                        else if (layer.Name == "DestroyableTiles" && !TileIsDestroyed(layer.Tiles[i]))
                            spriteBatch.Draw(tileSet, new Rectangle((int)x, (int)y, size.X, size.Y), tilesetRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.55f);
                        else if (layer.Name.ToLower() == "background")
                            spriteBatch.Draw(tileSet, new Rectangle((int)x, (int)y, size.X, size.Y), tilesetRec, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                    }
            boss?.Draw(spriteBatch);
            branchWall?.Draw(spriteBatch);
            foreach (Npc npc in npcs)
                npc.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            CheckIfCollidesWithDoor();

            boss?.Update(gameTime);
            if (branchCutScene.HaveEnded)
                branchWall?.Update(gameTime);
            foreach (Npc npc in npcs)
                npc.Update(gameTime);

            if (branchWall == null)
                return;


            if (MapName.Equals("RunFromBranches") && game.Player.Location.X > 35 * 48 && !branchCutScene.HaveBeenTriggered)
                branchCutScene.Start(game.Player);

            if (branchCutScene.HaveBeenTriggered && !branchCutScene.HaveEnded)
                branchCutScene.Play(this, game.Player.camera, game.Player, gameTime);

            if (branchCutScene.HaveEnded)
                branchWall.Update(gameTime);

        }

        private bool TileIsDestroyed(TmxLayerTile tile)
        {
            foreach (Point point in destroyedTiles)
                if (point == new Point(tile.X, tile.Y) * Size)
                    return true;
            return false;
        }

        private void CheckIfCollidesWithDoor()
        {
            TmxObjectGroup group = map.ObjectGroups.First(group => group.Name.ToLower() == "doors");
            Dictionary<Rectangle, TmxObject> doors = new();

            foreach (TmxObject obj in group.Objects)
                doors.Add(new((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height), obj);

            foreach (KeyValuePair<Rectangle, TmxObject> pair in doors)
                if (game.Player.HitBox.Intersects(pair.Key))
                {
                    if (pair.Value.Name.ToLower() == "end")
                    {
                        game.WindowManager.SetEnd(boss);
                        game.Player.CurrentAction = Action.Ended;
                        break;
                    }
                    mapManager.GoToSpawnPoint(int.Parse(pair.Value.Name.Split('-')[1]));
                }
        }
        bool oldX = true;
        public (bool, bool) WillCollide(Player player, GameTime gameTime)
        {
            bool canMoveX = true, canMoveY = true;
            List<TmxLayer> collisionLayers = GetCollisionLayers();
            foreach (TmxLayer layer in collisionLayers)
                foreach (var tile in layer.Tiles)
                    if (!destroyedTiles.Contains(new(tile.X * size.X, tile.Y * size.Y)))
                    {
                        Rectangle rec = new(new(tile.X * size.X, tile.Y * size.Y), size);
                        if (tile.Gid != 0)
                            CheckForUnWantedCollision(player, rec);

                        if (player.NextHorizontalHitBox.Intersects(rec) && tile.Gid != 0)
                        {
                            if ((player.ActiveMach == Mach.Running || player.ActiveMach == Mach.Sprinting) && IsTouchingDestroyableBlock(player.NextHorizontalHitBox))
                                RemoveTiles(player.NextHorizontalHitBox, gameTime);
                            else
                            {
                                canMoveX = false;
                                if (player.Facing == Facing.Right && player.playerMovement.HorizontalSpeed > 0 && oldX)
                                    player.Location += new Point(tile.X * size.X - player.HitBox.Right, 0);
                                else if (player.playerMovement.HorizontalSpeed < 0 && oldX)
                                    player.Location += new Point(rec.Right - player.HitBox.Left, 0);
                            }
                        }
                        if (player.NextVerticalHitBox.Intersects(rec) && tile.Gid != 0)
                        {
                            if (IsTouchingDestroyableBlock(player.NextVerticalHitBox) && player.CurrentAction == Action.GroundPounding)
                                RemoveTiles(player.NextVerticalHitBox, gameTime);
                            else
                            {
                                canMoveY = false;
                                if (player.playerMovement.VerticalSpeed > 0)
                                    player.isGrounded = true;
                                player.playerMovement.VerticalSpeed = 0;
                                break;
                            }
                        }
                        else if (player.playerMovement.VerticalSpeed != 0)
                            player.isGrounded = false;
                    }
            oldX = canMoveX;
            return (canMoveX, canMoveY);
        }

        private static void CheckForUnWantedCollision(Player player, Rectangle tile)
        {
            if (!player.HitBox.Intersects(tile))
                return;

            if (tile.Left - player.HitBox.Right <= tile.Right - player.HitBox.Left)
                player.Location -= new Point(player.HitBox.Right - tile.Left, 0);
            else
                player.Location -= new Point(tile.Right - player.HitBox.Left, 0);

        }

        private void RemoveTiles(Rectangle rectangle, GameTime gameTime)
        {
            foreach (TmxLayerTile tile in map.Layers["DestroyableTiles"].Tiles)
                if (!destroyedTiles.Contains(new(tile.X * Size.X, tile.Y * Size.Y)))
                    if (rectangle.Intersects(new(new(tile.X * Size.X, tile.Y * Size.Y), Size)) && tile.Gid != 0)
                    {
                        game.Player.hasDestroyedBlock = true;
                        game.Player.destroyBlockTime = gameTime.TotalGameTime.TotalSeconds;

                        destroyedTiles.Add(new(tile.X * Size.X, tile.Y * Size.Y));
                    }
        }

        private bool IsTouchingDestroyableBlock(Rectangle rectangle)
        {
            if (!map.Layers.Contains("DestroyableTiles"))
                return false;
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
                if (layer.Name.ToLower() == "platforms" || layer.Name.ToLower() == "destroyabletiles")
                    platformLayer.Add(layer);

            return platformLayer;
        }

        internal void IsNextToWall(Player player)
        {
            foreach (var layer in GetCollisionLayers())
                foreach (var tile in layer.Tiles)
                    if (layer.Name.ToLower() != "destroyabletiles" && tile.Gid != 0)
                    {
                        Rectangle rec = new(new(tile.X * size.X, tile.Y * size.Y), size);
                        if (player.Facing == Facing.Right && rec.Intersects(new(player.Location + new Point(5, 0), player.Size)))
                        {
                            player.isNextToWall = true;
                            return;
                        }
                        if (player.Facing == Facing.Left && rec.Intersects(new(player.Location + new Point(-5, 0), player.Size)))
                        {
                            player.isNextToWall = true;
                            return;
                        }
                    }

            player.isNextToWall = false;
        }

        private void WriteHelpingText(Keybinds keybinds, SpriteBatch spriteBatch)
        {
            if (!TmxMap.ObjectGroups.Contains("TutorialTextPoints"))
                return;

            var layer = TmxMap.ObjectGroups.First(objGroup => objGroup.Name == "TutorialTextPoints");

            List<string> text = new()
            { 
                $"You can move RIGHT and LEFT with the keys {keybinds.RightKey} and {keybinds.LeftKey}",
                $"JUMP with {keybinds.JumpKey} and CROUCH with {keybinds.CrouchKey}",
                $"You can ACCELARATE your speed by holding down {keybinds.AccelerateKey}",
                $"Hold the other direction while running to preform a TURN",
                $"Sprint into the wall to preform a WALL CLIMB",
                $"Press {keybinds.CrouchKey} while in the air to preform a GROUND POUND",
                $"While standing next to a BREAKABLE BLOCK press {keybinds.AttackKey} to DESTROY",
                $"You can also RUN into BREAKABLE BLOCKS to destroy them",
                $"Press {keybinds.TalkKey} to start a CONVERSATION",
                "Talk to the locals for information to progress"
            };


            foreach (var obj in layer.Objects)
            {
                if (obj.Name != "Progress")
                {
                    string t = text[int.Parse(obj.Name) - 1];
                    Rectangle rec = new((int)obj.X, (int)obj.Y, (int)font.MeasureString(t).X + 50, 80);
                    spriteBatch.Draw(texture, rec, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.94f);
                    spriteBatch.DrawString(font, t, rec.Location.ToVector2() + new Vector2(20, 20), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.95f);
                }
                else if (game.WindowManager.dialogueWindow != null && !game.WindowManager.dialogueWindow.haveGivenInformation)
                {
                    Rectangle rec = new((int)obj.X, (int)obj.Y, (int)font.MeasureString(text[^1]).X + 50, 80);
                    spriteBatch.Draw(texture, rec, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.94f);
                    spriteBatch.DrawString(font, text[^1], rec.Location.ToVector2() + new Vector2(20, 20), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.95f);
                }
            }
        }

        public void Reset()
        {
            game.MusicManager.Reset();
            destroyedTiles = new();
            if (branchWall != null)
            {
                LoadBranchWall();
                branchCutScene.Reset();
            }
            if (boss != null)
                LoadBoss();
        }

        internal bool IsInsideWall(Rectangle rectangle)
        {
            foreach (TmxLayer layer in GetCollisionLayers())
                foreach (var tile in layer.Tiles)
                    if (!destroyedTiles.Contains(new(tile.X * size.X, tile.Y * size.Y)))
                        if (rectangle.Intersects(new(new(tile.X * size.X, tile.Y * size.Y), size)) && tile.Gid != 0)
                            return true;
            return false;
        }
    }
}
