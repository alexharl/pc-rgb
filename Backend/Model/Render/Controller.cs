using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PcRGB.Model.Cofig;
using PcRGB.Model.Control;
using PcRGB.Model.Extensions;

namespace PcRGB.Model.Render
{
    public class Controller : Layer
    {
        public byte HardwareId { get; set; }
        public List<Point> PixelPositions { get; set; }
        public Controller(string name, byte hardwareId, int x, int y, int width, int height) : base(name, x, y, width, height)
        {
            HardwareId = hardwareId;
            PixelPositions = new List<Point>();
        }

        public List<byte> BufferFrom(Layer layer)
        {
            return ControllerCommand
                .SetComponent(HardwareId)
                .Buffer
                .AddPixels(PixelsFrom(layer));
        }

        public List<Pixel> PixelsFrom(Layer layer)
        {
            return PixelPositions.Select(position =>
            {
                return layer.PixelAt(position.X + Rect.X, position.Y + Rect.Y) ?? new Pixel(position);
            }).ToList();
        }

        public static Controller FromConfig(ControllerConfig config)
        {
            var component = new Controller(config.Name, (byte)config.HardwareId, config.X, config.Y, config.Width, config.Height);
            component.PixelPositions = config.PixelPositions.Select(p => new Point(p.X, p.Y)).ToList();
            return component;
        }
    }
}