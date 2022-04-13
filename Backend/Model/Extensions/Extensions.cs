using System.Collections.Generic;
using PcRGB.Model.Render;

namespace PcRGB.Model.Extensions
{
    public static class Extensions
    {
        public static List<byte> AddPixels(this List<byte> buffer, List<Pixel> pixels)
        {
            foreach (var pixel in pixels)
            {
                buffer.AddRange(new List<byte>
                {
                    pixel.Color.Hue,
                    pixel.Color.Saturation,
                    pixel.Color.Brightness
                });
            }
            return buffer;
        }
    }
}