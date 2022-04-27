using System.Collections.Generic;
using System.Drawing;
using core.Model.Graphics;
using core.Model.Layers;

namespace core.Model.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Adds the HSB color values of provided pixels to a byte array
        /// </summary>
        /// <param name="buffer">the byte array</param>
        /// <param name="pixels">the pixels</param>
        /// <returns>buffer with pixel values</returns>
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

        /// <summary>
        /// Calls point for each position in the rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        public static void Each(this Rectangle rect, PointEachDelegate point)
        {
            for (int y = 0; y < rect.Size.Height; y++)
                for (int x = 0; x < rect.Size.Width; x++)
                    point(x, y);
        }
    }
}