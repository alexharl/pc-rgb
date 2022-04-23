using System.Collections.Generic;
using System.Drawing;
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

        public static void Each(this Rectangle rect, PointEachDelegate point)
        {
            for (int y = 0; y < rect.Size.Height; y++)
                for (int x = 0; x < rect.Size.Width; x++)
                    point(x, y);
        }
    }
}