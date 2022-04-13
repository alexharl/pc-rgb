using System.Numerics;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class ScanningLinesEffect : EffectLayer
    {
        Vector2 Center = new Vector2(0, 0);
        Vector2 Direction = new Vector2(1, 1);

        public ScanningLinesEffect(int width, int height) : base("Scanning Lines", width, height) { }

        void UpdatePixels()
        {
            Each((x, y) =>
            {
                var pixel = PixelAt(x, y);
                byte hue = 0;
                byte saturation = 0;
                byte brightness = 0;
                float alpha = 1;
                if (pixel.Position.X == Center.X && pixel.Position.Y != Center.Y)
                {
                    // horizontale Linie
                    hue = 50;
                    saturation = 0;
                    brightness = 128;
                }
                else if (pixel.Position.X != Center.X && pixel.Position.Y == Center.Y)
                {
                    // vertikale Linie
                    hue = 160;
                    saturation = 0;
                    brightness = 128;
                }
                else if (pixel.Position.X == Center.X && pixel.Position.Y == Center.Y)
                {
                    // exakter Punkt
                    hue = 255;
                    saturation = 0;
                    brightness = 0;
                }
                else
                {
                    // Alle anderen Punkte
                    alpha = 0;
                }
                pixel.Color = new HSB(hue, saturation, brightness, alpha);
            });
        }

        public void MoveCenter()
        {
            switch (Direction.X)
            {
                case 1:
                    if (Center.X-- <= 0)
                    {
                        Center.X = 0;
                        Direction.X = 0;
                    }
                    break;
                default:
                    if (Center.X++ >= Size.Height)
                    {
                        Center.X = Size.Height - 1;
                        Direction.X = 1;
                    }
                    break;
            }
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
                    if (Center.Y++ >= Size.Width)
                    {
                        Center.Y = Size.Width - 1;
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