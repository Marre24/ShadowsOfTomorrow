using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsOfTomorrow
{
    public class Animation
    {
        public int CurrentFrame { get; set; }
        public int FrameCount { get; set; }
        public int FrameHeight { get { return RightTexture.Height; } }
        public int FrameWidth { get { return RightTexture.Width / FrameCount; } }
        public float FrameSpeed { get; set; }
        public bool IsLooping { get; set; }
        public Texture2D RightTexture { get; private set; }
        public Texture2D LeftTexture { get; private set; }

        public Animation(Texture2D left, Texture2D right, int frameCount)
        {
            RightTexture = right;
            LeftTexture = left;
            FrameCount = frameCount;

            IsLooping = true;
            FrameSpeed = 0.1f;
        }
    }
}
