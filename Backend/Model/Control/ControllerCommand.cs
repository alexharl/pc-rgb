using System.Collections.Generic;

namespace PcRGB.Model.Control
{
    public class ControllerCommand
    {
        public static byte NOOP = 0;
        public static byte SET_COMPONENT = 1;
        public static byte SHOW = 2;

        public byte Command;
        public List<byte> Buffer;

        public static ControllerCommand SetComponent(byte component)
        {
            return new ControllerCommand { Command = SET_COMPONENT, Buffer = new List<byte> { SET_COMPONENT, component } };
        }

        public static ControllerCommand Show()
        {
            return new ControllerCommand { Command = SHOW, Buffer = new List<byte> { SHOW } };
        }
    }
}