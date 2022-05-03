using System.Collections.Generic;
using core.Common.Helper;

namespace core.Model.Graphics
{
    public class HSB
    {
        public byte Hue { get; set; }
        public byte Saturation { get; set; }
        public byte Brightness { get; set; }
        public float Alpha { get; set; }

        public HSB(byte h, byte s, byte b)
        {
            Hue = h;
            Saturation = s;
            Brightness = b;
            Alpha = 1;
        }

        public HSB(byte h, byte s, byte b, float a)
        {
            Hue = h;
            Saturation = s;
            Brightness = b;
            Alpha = a;
        }

        public static HSB Copy(HSB color)
        {
            return new HSB(color.Hue, color.Saturation, color.Brightness, color.Alpha);
        }

        public void CopyFrom(HSB color)
        {
            Hue = color.Hue;
            Saturation = color.Saturation;
            Brightness = color.Brightness;
            Alpha = color.Alpha;
        }

        public static byte MapToValue(float value, float from, float to)
        {
            return (byte)Math.Map(value, from, to, 0, 255);
        }

        public List<byte> Raw()
        {
            return new List<byte>
            {
                Hue,
                Saturation,
                Brightness
            };
        }
    }
}