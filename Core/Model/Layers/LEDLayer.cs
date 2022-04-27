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
        public List<Point> PixelPositions { get; set; }
        public LEDLayer(string name, byte hardwareId, int x, int y, int width, int height) : base(name, x, y, width, height)
        {
            HardwareId = hardwareId;
            PixelPositions = new List<Point>();
        }
        public List<Pixel> PixelsFrom(Layer layer)
        {
            return PixelPositions.Select(position =>
            {
                return layer.PixelAt(position.X + Rect.X, position.Y + Rect.Y) ?? new Pixel(position);
            }).ToList();
        }

        public static LEDLayer FromConfig(ControllerConfig config)
        {
            var component = new LEDLayer(config.Name, (byte)config.Id, config.X, config.Y, config.Width, config.Height);
            component.PixelPositions = config.PixelPositions.Select(p => new Point(p.X, p.Y)).ToList();
            return component;
        }
    }
}