using System;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class MovingRainbowEffect : EffectLayer
    {
        Vector2 Center = new Vector2(4, 13);

        Vector2 Direction = new Vector2(0, 0);

        public MovingRainbowEffect(int width, int height) : base("Moving Rainbow", width, height) { }

        void UpdatePixels()
        {
            foreach (var row in Pixels)
            {
                foreach (var pixel in row)
                {
                    double distanceToCenter = pixel.Position.DistanceTo(Center);
                    pixel.Color = new HSB(0, 255, 128);
                    pixel.Color.SetHueWithRange((int)distanceToCenter, 0, 20);
                }
            }
        }

        public void MoveCenter()
        {
            switch (Direction.Y)
            {
                case 1:
                    if (Center.Y-- <= 0)
                    {
                        Center.Y = 0;
                        Direction.Y = 0;
                    }
                    break;
                default:
                    if (Center.Y++ >= Size.Y)
                    {
                        Center.Y = Size.Y - 1;
                        Direction.Y = 1;
                    }
                    break;
            }
        }

        public override void Update()
        {
            if (!Running) return;

            UpdatePixels();

            MoveCenter();
        }
    }
}