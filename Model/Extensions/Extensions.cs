using System.Collections.Generic;
using PcRGB.Model.Control;

namespace PcRGB.Model.Extensions
{
    public static class Extensions
    {
        public static void AddCommand(this List<byte> buffer, ControllerCommand command)
        {
            buffer.AddRange(command.ToBuffer());
            buffer.AddRange(command.Buffer);
        }
    }
}