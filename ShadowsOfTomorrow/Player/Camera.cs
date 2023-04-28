using Microsoft.Xna.Framework;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ShadowsOfTomorrow
{
    public class Camera
    {
        public Rectangle Window { get => _window; private set => _window = value; }
        private Rectangle _window;

        public Matrix Transform { get => _transform; private set => _transform = value; }
        private Matrix _transform;

        readonly Random rand = new();

        public Camera() { }

        public void Follow(Rectangle target, Map map, bool shaking)
        {
            if (!shaking)
                return;

            float shakeRadius = 3.0f;
            int shakeStartAngle = (150 + rand.Next(60));
            Vector2 offset = new ((float)(Math.Sin(shakeStartAngle) * shakeRadius), (float)(Math.Cos(shakeStartAngle) * shakeRadius));
            target.Location += offset.ToPoint();

            Follow(target, map);
        }

        public void Follow(Rectangle target, Map map)
        {
            Point playerCenter = new(target.Location.X + (target.Width / 2), target.Location.Y - 150);
            Point screenCenter = new(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            Point cameraCenter = playerCenter;

            if (map == null)
                goto SetMatrix;

            if (map.MapName.ToLower() == "bossroom")
            {
                cameraCenter = new Point(1100, 800);
                goto SetMatrix;
            }

            if (playerCenter.Y - screenCenter.Y <= map.Top + 2 * 48)
                cameraCenter.Y = screenCenter.Y + (int)map.Top + 2 * 48;
            if (playerCenter.Y + screenCenter.Y >= map.Bottom)
                cameraCenter.Y = (int)map.Bottom - screenCenter.Y;

            if (playerCenter.X - screenCenter.X <= map.Left + 2 * 48)
                cameraCenter.X = screenCenter.X + (int)map.Left + 2 * 48;
            if (playerCenter.X + screenCenter.X >= map.Right - 2 * 48)
                cameraCenter.X = (int)map.Right - screenCenter.X - 2 * 48;

            SetMatrix:
            var position = Matrix.CreateTranslation(-cameraCenter.X, -cameraCenter.Y, 0);
            var offset = Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);

            Transform = offset * position;

            UpdateWindow(cameraCenter);
        }

        public void Follow(Point target)
        {
            Point screenCenter = new(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);

            var position = Matrix.CreateTranslation(-target.X, -target.Y, 0);
            var offset = Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);

            Transform = offset * position;

            UpdateWindow(target);
        }

        private void UpdateWindow(Point target)
        {
            Window = new(target.X - Screen.PrimaryScreen.Bounds.Width / 2, target.Y - Screen.PrimaryScreen.Bounds.Height / 2, 
                Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.Height);
        }
    }
}
