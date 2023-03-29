using Microsoft.Xna.Framework;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ShadowsOfTomorrow
{
    public class Camera
    {
        public Rectangle Window { get => _window; private set => _window = value; }
        private Rectangle _window;

        public Matrix Transform { get => _transform; private set => _transform = value; }
        private Matrix _transform;

        public Camera() { }

        public void Follow(Rectangle target, Map map)
        {
            Point playerCenter = new(target.Location.X + (target.Width / 2), target.Location.Y + (target.Height / 2));
            Point screenCenter = new(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);

            Point cameraCenter = playerCenter;

            if (playerCenter.Y - screenCenter.Y <= map.Top)
                cameraCenter.Y = screenCenter.Y + (int)map.Top;
            if (playerCenter.Y + screenCenter.Y >= map.Bottom)
                cameraCenter.Y = (int)map.Bottom - screenCenter.Y;

            if (playerCenter.X - screenCenter.X <= map.Left)
                cameraCenter.X = screenCenter.X + (int)map.Left;
            if (playerCenter.X + screenCenter.X >= map.Right)
                cameraCenter.X = (int)map.Right - screenCenter.X;


            var position = Matrix.CreateTranslation(-cameraCenter.X, -cameraCenter.Y, 0);
            var offset = Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);

            Transform = offset * position;

            UpdateWindow(cameraCenter);
        }

        private void UpdateWindow(Point target)
        {
            Window = new(target.X - Screen.PrimaryScreen.Bounds.Width / 2, target.Y - Screen.PrimaryScreen.Bounds.Height / 2, 
                Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.Height);
        }
    }
}
