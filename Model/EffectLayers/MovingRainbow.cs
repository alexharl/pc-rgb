using System;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class MovingRainbowEffect : EffectLayer
    {
        Position Center = new Position(0, 0);

        int DirectionY = 0;
        int MinY = 0;
        int MaxY = 20;

        public MovingRainbowEffect(int width, int height) : base("Moving Rainbow", width, height) { }

        float map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public override void Update()
        {
            if (!Running) return;

            foreach (var row in Pixels)
            {
                foreach (var pixel in row)
                {
                    double distanceToCenter = pixel.Position.DistanceTo(Center);
                    byte h = (byte)map((float)distanceToCenter, 0, 20, 0, 255);
                    byte s = 255;
                    byte b = 128;
                    pixel.Color = new HSB(h, s, b);
                }
            }

            if (DirectionY == 0)
            {
                Center.Y++;
                if (Center.Y >= MaxY)
                {
                    Center.Y = MaxY - 1;
                    DirectionY = 1;
                }
            }
            else
            {
                Center.Y--;
                if (Center.Y <= MinY)
                {
                    Center.Y = MinY;
                    DirectionY = 0;
                }
            }
        }
    }
}