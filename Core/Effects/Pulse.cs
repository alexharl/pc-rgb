using System;
using System.Drawing;
using System.Numerics;
using core.Model.Graphics;

namespace core.Effects
{
    public class PulseEffect : EffectLayer
    {
        float Position = 0;
        int Direction = 0;
        byte Hue = 0;

        public PulseEffect(int x, int y, int width, int height) : base("Pulse", x, y, width, height) { }

        void UpdatePixels()
        {
            Pixels.ForEach(pixel =>
            {

                if (pixel.Position.Y >= Position)
                {
                    pixel.Color = new HSB(0, 255, 255, 1);
                }
                else
                {
                    var distance = HSB.MapToValue(Position - pixel.Position.Y, 19, 0);
                    pixel.Color = new HSB(0, 255, distance, 1);
                }
            });
        }

        public void MoveCenter()
        {
            switch (Direction)
            {
                case 1:
                    Position -= 0.5f;
                    if (Position < 0)
                    {
                        Position = 0;
                        Direction = 0;

                        Hue = (byte)(Hue + 1);
                    }
                    break;
                default:
                    Position += 0.5f;
                    if (Position >= Rect.Size.Height - 1)
                    {
                        Position = Rect.Size.Height - 1;
                        Direction = 1;

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