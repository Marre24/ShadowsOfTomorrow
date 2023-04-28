using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ShadowsOfTomorrow
{
    public enum Phase
    {
        StartDialogue,
        Stunned,
        LeafFalling,
        LeafWind,
        BranchingUp,
        BranchingSide,
    }

    public class Boss : Speech, IUpdateAndDraw
    {
        public Rectangle HitBox { get => new(location, new(texture.Width, texture.Height)); }
        public Phase ActivePhase => activePhase;
        public Color[] TextureData
        {
            get
            {
                Color[] color = new Color[texture.Width * texture.Height];
                texture.GetData(color);
                return color;
            }
        }

        private readonly Game1 game;
        private readonly PhaseManager phaseManager;
        private Point location;
        readonly Texture2D texture;
        private readonly Phase activePhase = Phase.StartDialogue;

        private int health = 10;
        public bool wasKilled;

        public Boss(Game1 game, string name, Point location) : base(name, true)
        {
            texture = game.Content.Load<Texture2D>("Sprites/Bosses/TreevorLeaf_x3");
            phaseManager = new(this);
            this.game = game;
            this.location = location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            phaseManager.Draw(spriteBatch);
            spriteBatch.Draw(texture, location.ToVector2(), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9f);
        }

        public void Update(GameTime gameTime)
        {
            if (health <= 0)
            {
                wasKilled = true;
                return;
            }

            if (activePhase == Phase.StartDialogue && Keyboard.GetState().IsKeyDown(Keys.K)) // gör om
            {
                game.windowManager.SetDialogue(Dialogue);
                game.player.CurrentAction = Action.Talking;
            }

            if (game.player.animationManager.CurrentCropTexture == null)
                return;
            game.player.animationManager.CurrentCropTexture.GetData(game.player.TextureData);
            texture.GetData(TextureData);

            //if (HasIntersectingPixels(game.player.HitBox, game.player.TextureData, HitBox, TextureData))
            //    game.player.OnHit();
        }

        internal void OnHit()
        {
            health--;
        }

        public static bool HasIntersectingPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // GET BOUNDS
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // GET COLORS FROM CURRENT POINT
                    Color colorA = dataA[(x - rectangleA.Left) + (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) + (y - rectangleB.Top) * rectangleB.Width];

                    // IF BOTH NOT TRANSPARENT, INTERSECTION = true
                    if (colorA.A != 0 && colorB.A != 0)
                        return true;
                }
            }

            return false;
        }
    }
}
