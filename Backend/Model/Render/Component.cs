using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PcRGB.Model.Cofig;
using PcRGB.Model.Control;
using PcRGB.Model.Extensions;

namespace PcRGB.Model.Render
{
    public class Component : Layer
    {
        public byte HardwareId { get; set; }
        public List<Point> PixelPositions { get; set; }
        public Component(string name, byte hardwareId, int x, int y, int width, int height) : base(name, x, y, width, height)
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
                var pixel = layer.PixelAt(position.X + Rect.X, position.Y + Rect.Y);
                if (pixel != null) return pixel;

                return new Pixel(position);
            }).ToList();
        }

        public static Component FromConfig(ComponentConfig config)
        {
            var component = new Component(config.Name, (byte)config.Id, config.X, config.Y, config.Width, config.Height);
            component.PixelPositions = config.PixelPositions.Select(p => new Point(p.X, p.Y)).ToList();
            return component;
        }
    }
}