using System.Collections.Generic;
using System.Linq;
using PcRGB.Model.Control;

namespace PcRGB.Model.Render
{
    public class Component
    {
        public string Name { get; set; }
        public byte Id { get; set; }
        public List<Pixel> Pixels { get; set; }

        public List<byte> ToBuffer()
        {
            var command = ControllerCommand.SetComponent(Id);
            var colors = Pixels.Select(p => p.Color);
            foreach (var c in colors)
            {
                command.Buffer.AddRange(c.ToBuffer());
            }

            return command.Buffer;
        }
    }
}