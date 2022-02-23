using System.Collections.Generic;

namespace PcRGB.Model.Render
{
    public class HSB
    {
        public byte Hue { get; set; }
        public byte Saturation { get; set; }
        public byte Brightness { get; set; }

        public HSB(byte h, byte s, byte b)
        {
            Hue = h;
            Saturation = s;
            Brightness = b;
        }

        public void SetHueWithRange(int hue, int from, int to)
        {
            Hue = (byte)Map(hue, from, to, 0, 255);
        }

        public List<byte> ToBuffer()
        {
            return new List<byte> { Hue, Saturation, Brightness };
        }

        public static float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
    }
}