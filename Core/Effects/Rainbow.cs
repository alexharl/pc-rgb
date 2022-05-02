using System;
using System.Drawing;
using System.Numerics;
using core.Model.Graphics;

namespace core.Effects
{
    public class MovingRainbowEffect : EffectLayer
    {
        PointF Center = new PointF(4, 13);
        Point Direction = new Point(0, 0);

        byte BaseHue = 0;

        public MovingRainbowEffect(int x, int y, int width, int height) : base("Moving Rainbow", x, y, width, height) { }

        void UpdatePixels()
        {
            Pixels.ForEach(pixel =>
            {
                float distanceToCenter = Vector2.Distance(new Vector2(pixel.Position.X, pixel.Position.Y), new Vector2(Center.X, Center.Y));
                pixel.Color = new HSB(0, 255, 128, 1);
                var distanceHue = HSB.MapToValue(distanceToCenter, 0, 20);

                pixel.Color.Hue = (byte)(distanceHue + BaseHue);
            });
        }

        public void MoveCenter()
        {
            BaseHue++;
            switch (Direction.Y)
            {
                case 1:
                    Center.Y -= 0.2f;
                    if (Center.Y <= 0)
                    {
                        Center.Y = 0;
                        Direction.Y = 0;
                    }
                    break;
                default:
                    Center.Y += 0.2f;
                    if (Center.Y >= Rect.Size.Height - 1)
                    {
                        Center.Y = Rect.Size.Height - 1;
                        Direction.Y = 1;
                    }
                    break;
            }

            switch (Direction.X)
            {
                case 1:
                    Center.X -= 0.2f;
                    if (Center.X <= 0)
                    {
                        Center.X = 0;
                        Direction.X = 0;
                    }
                    break;
                default:
                    Center.X += 0.2f;
                    if (Center.X >= Rect.Size.Width - 1)
                    {
                        Center.X = Rect.Size.Width - 1;
                        Direction.X = 1;
                    }
                    break;
            }

            if (BaseHue >= 254)
            {
                BaseHue = 0;
            }
        }

        public override void Update()
        {
            if (!Active) return;

            UpdatePixels();

            MoveCenter();
        }
    }
}