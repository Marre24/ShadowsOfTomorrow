using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        public Phase ActivePhase => _activePhase;
        public BaseClassPhase ActivePhaseClass 
        { 
            get 
            {
                return health switch
                {
                    1 => phaseManager.branchingUp,
                    2 => phaseManager.branchingSide,
                    3 => phaseManager.leafBlowing,
                    4 => phaseManager.leafFalling,
                    _ => phaseManager.leafFalling,
                };
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
        private Phase _activePhase = Phase.Dialogue;
        private Phase oldPhase = Phase.Dialogue;
        public bool isStunned = false;

        private int stunOMeter = 0;

        private const int startingHealth = 4;
        public int health = startingHealth;
        public bool wasKilled;
        public int talkingIndex = 0;

        private const double timeStunned = 4;
        private double timeSinceStun = 0;
        private Rectangle blackRectangle;
        private Rectangle greenRectangle;

        private readonly Texture2D blackPixel;
        private readonly Texture2D greenPixel;

        public Boss(Game1 game, string name, Point location) : base(name, true)
        {
            blackPixel = new Texture2D(game.GraphicsDevice, 1, 1);
            blackPixel.SetData<Color>(new Color[] { Color.Black });

            greenPixel = new Texture2D(game.GraphicsDevice, 1, 1);
            greenPixel.SetData<Color>(new Color[] { Color.Green });
            

            texture = game.Content.Load<Texture2D>("Sprites/Bosses/TreevorLeaf_x3");
            phaseManager = new(this, game);
            this.game = game;
            this.location = location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (wasKilled && game.player.CurrentAction != Action.Talking)
                return;
            spriteBatch.Draw(texture, location.ToVector2(), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.9f);

            if (ActivePhase == Phase.Dialogue)
                return;

            spriteBatch.Draw(blackPixel, blackRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.94f);
            spriteBatch.Draw(greenPixel, greenRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.95f);

            phaseManager.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            if (health <= 0)
            {
                wasKilled = true;
                return;
            }
            blackRectangle = new(HitBox.Center + new Point(-200, - (texture.Height / 2) -120), new(400, 100));
            greenRectangle = new(blackRectangle.Location + new Point(5,5), new(50 + (stunOMeter * blackRectangle.Size.X - 90) / ActivePhaseClass.maxStunOMeter, blackRectangle.Size.Y - 10));

            if (oldPhase == Phase.Dialogue && game.player.CurrentAction != Action.Talking)
                SetNewPhase();

            if (stunOMeter >= ActivePhaseClass.maxStunOMeter)
                StunPhase(gameTime);
            
            if (ActivePhase != Phase.Dialogue)
                phaseManager.Update(gameTime);


            if (game.player.animationManager.CurrentCropTexture != null && !isStunned && _activePhase != Phase.Dialogue && game.player.HitBox.Intersects(HitBox))
            {
                game.player.animationManager.CurrentCropTexture.GetData(game.player.TextureData);
                texture.GetData(TextureData);

                if (HasIntersectingPixels(game.player.HitBox, game.player.TextureData, HitBox, TextureData))
                    game.player.OnHit(Facing.Right);
            }

            oldPhase = _activePhase;
        }

        private void StunPhase(GameTime gameTime)
        {
            isStunned = true;

            if (timeSinceStun == 0)
            {
                timeSinceStun = gameTime.TotalGameTime.TotalSeconds;
                return;
            }

            if (timeSinceStun + timeStunned < gameTime.TotalGameTime.TotalSeconds)     //Break out of stun
            {
                SetNewPhase();
                timeSinceStun = 0;
                stunOMeter = 0;
                isStunned = false;
            }
        }

        private void SetNewPhase()
        {
            switch (health)
            {
                case 1:
                    _activePhase = Phase.BranchingSide;
                    break;
                case 2:
                    _activePhase = Phase.BranchingUp;
                    break;
                case 3:
                    _activePhase = Phase.LeafWind;
                    break;
                case 4:
                    _activePhase = Phase.LeafFalling;
                    break;
            }
        }

        internal void OnHit()
        {
            if (!isStunned)
                return;

            game.musicManager.Play(game.Content.Load<SoundEffect>("Music/OofMine"), true);

            health--;
            talkingIndex++;

            timeSinceStun = 0;
            stunOMeter = 0;
            isStunned = false;

            game.player.playerMovement.HorizontalSpeed = -20;
            game.player.playerMovement.VerticalSpeed = -15;
            game.player.isGrounded = false;

            game.player.CurrentAction = Action.Talking;
            game.windowManager.SetDialogue(Dialogue);
            _activePhase = Phase.Dialogue;
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
            _activePhase = Phase.Dialogue;
            talkingIndex = 0;
            health = startingHealth;
            phaseManager.Reset();
        }

        internal void GetHitByLeaf()
        {
            game.musicManager.Play(game.Content.Load<SoundEffect>("Music/OofMine"), true);
            stunOMeter++;
        }
    }
}
