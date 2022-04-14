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
            for (int x = rect.X; x < rect.X + rect.Size.Width; x++)
            {
                for (int y = rect.Y; y < rect.Y + rect.Size.Height; y++)
                {
                    point(x, y);
                    // point(x - rect.X, y - rect.Y);
                }
            }
        }
    }
}