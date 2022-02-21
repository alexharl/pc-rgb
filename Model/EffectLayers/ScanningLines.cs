using System;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class ScanningLinesEffect : EffectLayer
    {
        Position Center = new Position(0, 0);
        int DirectionX = 1;
        int DirectionY = 1;
        public ScanningLinesEffect(int width, int height) : base("Scanning Lines", width, height) { }

        public override void Update()
        {
            if (!Running) return;

            // Update Pixels
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
                    else if (pixel.Position.X != Center.X && pixel.Position.Y == Center.Y)
                    {
                        // vertikale Linie
                        hue = 160;
                        saturation = 150;
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

            // Check / Update Direction
            if (DirectionX == 0)
            {
                Center.X++;
                if (Center.X >= Height)
                {
                    Center.X = Height - 1;
                    DirectionX = 1;
                }
            }
            else
            {
                Center.X--;
                if (Center.X <= 0)
                {
                    Center.X = 0;
                    DirectionX = 0;
                }
            }

            if (DirectionY == 0)
            {
                Center.Y++;
                if (Center.Y >= Width)
                {
                    Center.Y = Width - 1;
                    DirectionY = 1;
                }
            }
            else
            {
                Center.Y--;
                if (Center.Y <= 0)
                {
                    Center.Y = 0;
                    DirectionY = 0;
                }
            }
        }
    }
}