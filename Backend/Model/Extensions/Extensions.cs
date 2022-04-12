using System.Collections.Generic;
using System.Drawing;
using PcRGB.Model.Control;
using PcRGB.Model.Render;

namespace PcRGB.Model.Extensions
{
    public static class Extensions
    {
        public static List<byte> AddPixels(this List<byte> buffer, List<Pixel> pixels)
        {
            foreach (var pixel in pixels)
            {
                buffer.AddRange(pixel.Color.ToBuffer());
            }
            return buffer;
        }
    }
}