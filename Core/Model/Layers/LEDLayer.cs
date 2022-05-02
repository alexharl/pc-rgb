using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using core.Model.Cofig;
using core.Model.Extensions;
using core.Model.Graphics;

namespace core.Model.Layers
{
    public class LEDLayer : Layer
    {
        public byte HardwareId { get; set; }
        public List<Point> PixelPositions { get; set; } = new List<Point>();

        public LEDLayer(string name, int x, int y, int w, int h) : base(name, x, y, w, h) { }

        public List<Pixel> PixelsFrom(Layer layer)
        {
            return PixelPositions.Select(position =>
            {
                return layer.PixelAt(position.X + Rect.X, position.Y + Rect.Y) ?? new Pixel(position);
            }).ToList();
        }
    }
}