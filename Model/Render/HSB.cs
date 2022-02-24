using System.Collections.Generic;
using PcRGB.Model.Helper;

namespace PcRGB.Model.Render
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

        public void SetHueWithRange(int hue, int from, int to)
        {
            Hue = (byte)Math.Map(hue, from, to, 0, 255);
        }

        public List<byte> ToBuffer()
        {
            return new List<byte> { Hue, Saturation, Brightness };
        }
    }
}