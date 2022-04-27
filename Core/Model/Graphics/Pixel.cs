using System.Drawing;

namespace core.Model.Graphics
{
    public class Pixel
    {
        public Point Position { get; set; }
        public HSB Color { get; set; }

        public Pixel() : this(0, 0) { }
        public Pixel(int x, int y) : this(x, y, new HSB(0, 0, 0, 0)) { }
        public Pixel(int x, int y, HSB color) : this(new Point(x, y), color) { }
        public Pixel(Point position) : this(position, new HSB(0, 0, 0, 0)) { }

        public Pixel(Point position, HSB color)
        {
            Position = position;
            Color = color;
        }

        public void Apply(Pixel pixel)
        {
            if (pixel.Color.Alpha == 0) return;
            if (pixel.Color.Alpha == 1)
            {
                Color = new HSB(pixel.Color.Hue, pixel.Color.Saturation, pixel.Color.Brightness, pixel.Color.Alpha);
            }
        }
    }

}