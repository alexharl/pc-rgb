using System;
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
            foreach (var row in Pixels)
            {
                foreach (var pixel in row)
                {
                    byte hue = 0;
                    byte saturation = 0;
                    byte brightness = 0;
                    bool transparent = false;
                    if (pixel.Position.X == Center.X && pixel.Position.Y != Center.Y)
                    {
                        // horizontale Linie
                        hue = 50;
                        saturation = 0;
                        brightness = 128;
                    }
                    else if (pixel.Position.X != Center.X && (pixel.Position.Y == Center.Y || pixel.Position.Y == Center.Y - 1))
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
                        transparent = true;
                    }
                    pixel.Color = new HSB(hue, saturation, brightness);
                    pixel.Transparent = transparent;
                }
            }
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
                    if (Center.X++ >= Size.Y)
                    {
                        Center.X = Size.Y - 1;
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
                    if (Center.Y++ >= Size.X)
                    {
                        Center.Y = Size.X - 1;
                        Direction.Y = 1;
                    }
                    break;
            }
        }

        public override void Update()
        {
            if (!Running) return;

            // Update Pixels
            UpdatePixels();

            // Check / Update Direction
            MoveCenter();
        }
    }
}