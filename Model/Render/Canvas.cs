using System.Collections.Generic;

namespace PcRGB.Model.Render
{
    public class Canvas : Layer
    {
        public List<Component> Components { get; set; }
        public Canvas(int width, int height) : base(width, height)
        {
            Components = new List<Component> { };
        }

        public List<byte> ToBuffer()
        {
            var buffer = new List<byte>();

            foreach (var component in Components)
            {
                buffer.AddRange(component.ToBuffer());
            }

            return buffer;
        }
    }
}