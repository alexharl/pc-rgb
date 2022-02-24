using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using PcRGB.Model.Control;
using PcRGB.Model.Extensions;

namespace PcRGB.Model.Render
{
    public class Component
    {
        public string Name { get; set; }
        public byte Id { get; set; }
        public List<Point> PixelPositions { get; set; }

        public List<byte> BufferFrom(Layer layer)
        {
            return ControllerCommand
                .SetComponent(Id)
                .Buffer
                .AddPixels(PixelsFrom(layer));
        }

        public List<Pixel> PixelsFrom(Layer layer)
        {
            return PixelPositions.Select(position => layer.Pixels[position.X][position.Y]).ToList();
        }
    }
}