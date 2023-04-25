using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class AnimationManager
    {
        public Animation Animation { get => _animation; }
        public Texture2D CurrentCropTexture { get; private set; }

        private Animation _animation;
        private readonly Game1 game;
        private float _timer;
        private Facing? facing;

        public AnimationManager(Animation animation, Game1 game)
        {
            _animation = animation;
            this.game = game;
        }

        public void Play(Animation animation)
        {
            if (animation == _animation)
                return;

            _animation = animation;

            animation.CurrentFrame = 0;

            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0;

            _animation.CurrentFrame = 0;
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 position, Facing? facing)
        {
            if (_animation == null) 
                return;

            if (facing == null)
            {
                //för annat
                return;
            }

            if (facing == Facing.Left)
                spriteBatch.Draw(_animation.LeftTexture, position, new Rectangle(_animation.CurrentFrame * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight), 
                    Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1.0f);
            else
                spriteBatch.Draw(_animation.RightTexture, position, new(_animation.CurrentFrame * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight), 
                    Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1.0f);

            this.facing = facing;
        }

        public void Update(GameTime gameTime)
        {
            if (_animation == null)
                return;

            Rectangle cropSource = new (_animation.CurrentFrame * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight);
            CurrentCropTexture = new Texture2D(game.GraphicsDevice, cropSource.Width, cropSource.Height);
            Color[] cropData = new Color[cropSource.Width * cropSource.Height];

            if (facing == null && Animation.NormalTexture != null)
                Animation.NormalTexture.GetData(0, cropSource, cropData, 0, cropData.Length);

            if (facing == Facing.Right)
                Animation.RightTexture.GetData(0, cropSource, cropData, 0, cropData.Length);

            if (facing == Facing.Left)
                Animation.RightTexture.GetData(0, cropSource, cropData, 0, cropData.Length);

            CurrentCropTexture.SetData(cropData);

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0;

                _animation.CurrentFrame++;

                if (_animation.CurrentFrame >= _animation.FrameCount)
                    _animation.CurrentFrame = 0;
            }
        }
    }
}
