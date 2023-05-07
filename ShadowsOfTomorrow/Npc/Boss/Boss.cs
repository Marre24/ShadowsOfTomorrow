﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShadowsOfTomorrow
{
    public enum Phase
    {
        Dialogue,
        LeafFalling,
        LeafWind,
        BranchingUp,
        BranchingSide,
    }

    public class Boss : Speech, IUpdateAndDraw
    {
        public Rectangle HitBox { get => new(location, new(texture.Width, texture.Height)); }
        public Phase ActivePhase => activePhase;
        public BaseClassPhase ActivePhaseClass 
        { 
            get 
            {
                switch (health)
                {
                    //case 1:
                    //    return phaseManager.;
                    //case 2:
                    //    return Phase.BranchingUp;
                    case 3:
                        return phaseManager.leafBlowing;
                    case 4:
                        return phaseManager.leafFalling;
                    default:
                        return phaseManager.leafFalling;
                }
            } 
        }

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
        private readonly SpriteFont font;
        private Phase activePhase = Phase.Dialogue;
        private Phase oldPhase = Phase.Dialogue;
        public bool isStunned = false;

        private int stunOMeter = 0;

        private const int startingHealth = 4;
        public int health = startingHealth;
        public bool wasKilled;
        public int talkingIndex = 0;

        private const double timeStunned = 3;
        private double timeScinceStun = 0;

        public Boss(Game1 game, string name, Point location) : base(name, true)
        {
            texture = game.Content.Load<Texture2D>("Sprites/Bosses/TreevorLeaf_x3");
            font = game.Content.Load<SpriteFont>("Fonts/DefaultFont");
            phaseManager = new(this, game);
            this.game = game;
            this.location = location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (wasKilled)
                return;
            spriteBatch.DrawString(font, health.ToString(), game.player.camera.Window.Location.ToVector2() + new Vector2(500, 50), Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0.91f);
            spriteBatch.Draw(texture, location.ToVector2(), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9f);

            phaseManager.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            if (health <= 0)
            {
                wasKilled = true;
                return;
            }

            if (oldPhase == Phase.Dialogue && game.player.CurrentAction != Action.Talking)
            {
                switch (health)
                {
                    case 1:
                        activePhase = Phase.BranchingSide;
                        break;
                    case 2:
                        activePhase = Phase.BranchingUp;
                        break;
                    case 3:
                        activePhase = Phase.LeafWind;
                        break;
                    case 4:
                        activePhase = Phase.LeafFalling;
                        break;
                }
            }


            if (stunOMeter >= ActivePhaseClass.maxStunOMeter)
            {
                isStunned = true;

                if (timeScinceStun == 0)
                {
                    timeScinceStun = gameTime.TotalGameTime.TotalSeconds;
                    return;
                }

                if (timeScinceStun + timeStunned < gameTime.TotalGameTime.TotalSeconds)
                {
                    switch (health)
                    {
                        case 1:
                            activePhase = Phase.BranchingSide;
                            break;
                        case 2:
                            activePhase = Phase.BranchingUp;
                            break;
                        case 3:
                            activePhase = Phase.LeafWind;
                            break;
                        case 4:
                            activePhase = Phase.LeafFalling;
                            break;
                    }
                    timeScinceStun = 0;
                    stunOMeter = 0;
                    isStunned = false;
                }
            }
            if (ActivePhase != Phase.Dialogue)
                phaseManager.Update(gameTime);

            if (game.player.animationManager.CurrentCropTexture == null || isStunned || activePhase == Phase.Dialogue|| !game.player.HitBox.Intersects(HitBox))
                return;

            game.player.animationManager.CurrentCropTexture.GetData(game.player.TextureData);
            texture.GetData(TextureData);

            if (HasIntersectingPixels(game.player.HitBox, game.player.TextureData, HitBox, TextureData))
                game.player.OnHit();

            oldPhase = activePhase;
        }

        internal void OnHit()
        {
            health--;
            talkingIndex++;

            timeScinceStun = 0;
            stunOMeter = 0;
            isStunned = false;

            game.player.playerMovement.HorizontalSpeed = -10;
            game.player.playerMovement.VerticalSpeed = -10;
            game.player.isGrounded = false;

            game.player.CurrentAction = Action.Talking;
            game.windowManager.SetDialogue(Dialogue);
            activePhase = Phase.Dialogue;
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

        internal void Reset()
        {
            activePhase = Phase.Dialogue;
            talkingIndex = 0;
            health = startingHealth;
            phaseManager.Reset();
        }

        internal void GetHitByLeaf()
        {
            stunOMeter++;
        }
    }
}
