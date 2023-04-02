using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private Animation _animation;

        private float _timer;

        public AnimationManager(Animation animation)
        {
            _animation = animation;
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


        public void Draw(SpriteBatch spriteBatch, Vector2 position, Facing facing)
        {
            if (_animation == null) 
                return;

            if (facing == Facing.Left)
                spriteBatch.Draw(_animation.LeftTexture, position, new(_animation.CurrentFrame * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight), Color.White);
            if (facing == Facing.Right)
                spriteBatch.Draw(_animation.RightTexture, position, new(_animation.CurrentFrame * _animation.FrameWidth, 0, _animation.FrameWidth, _animation.FrameHeight), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            if (_animation == null)
                return;

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
