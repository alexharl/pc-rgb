using System.Drawing;
using System.Numerics;
using PcRGB.Model.Helper;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class MovingRainbowEffect : EffectLayer
    {
        PointF Center = new PointF(4, 13);
        Point Direction = new Point(0, 0);

        public MovingRainbowEffect(int width, int height) : base("Moving Rainbow", width, height) { }

        void UpdatePixels()
        {
            Each((x, y) =>
            {
                var pixel = PixelAt(x, y);
                float distanceToCenter = Vector2.Distance(new Vector2(pixel.Position.X, pixel.Position.Y), new Vector2(Center.X, Center.Y));
                pixel.Color = new HSB(0, 255, 128, 1);
                pixel.Color.Hue = HSB.MapToValue(distanceToCenter, 0, 20);
            });
        }

        public void MoveCenter()
        {
            switch (Direction.Y)
            {
                case 1:
                    Center.Y -= 0.5f;
                    if (Center.Y <= 0)
                    {
                        Center.Y = 0;
                        Direction.Y = 0;
                    }
                    break;
                default:
                    Center.Y += 0.5f;
                    if (Center.Y >= Size.Height - 1)
                    {
                        Center.Y = Size.Height - 1;
                        Direction.Y = 1;
                    }
                    break;
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