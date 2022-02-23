using System.Drawing;

namespace PcRGB.Model.Render
{
    public class Pixel
    {
        public Point Position { get; set; }
        public HSB Color { get; set; }

        public void Apply(Pixel pixel)
        {
            if (pixel.Color.Alpha == 0) return;

            Color = new HSB(pixel.Color.Hue, pixel.Color.Saturation, pixel.Color.Brightness, pixel.Color.Alpha);
        }
    }

}