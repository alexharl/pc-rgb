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

        public List<byte> ToBuffer()
        {
            return new List<byte> { Hue, Saturation, Brightness };
        }

        public float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
    }
}