using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;

namespace core.Model.Serial
{
    enum FastLEDStreamCommand
    {
        set_controller = 1,
        show = 2
    }

    public class FastLEDStream
    {
        private SerialPort port { get; set; }

        public FastLEDStream(string portName, int portBaud = 115200)
        {
            port = new SerialPort(portName, portBaud, Parity.None, 8, StopBits.One);
            port.Open();
        }

        public bool Open()
        {
            if (IsOpen) return true;

            port.Open();
            return port.IsOpen;
        }

        public bool Close()
        {
            if (!IsOpen) return false;

            port.Close();
            return port.IsOpen;
        }

        public bool IsOpen
        {
            get { return port.IsOpen; }
        }

        public void Write(IEnumerable<byte> buffer)
        {
            if (!IsOpen)
                return;

            port.Write(buffer.ToArray(), 0, buffer.Count());
        }

        public void Show()
        {
            Write(new byte[] { (byte)FastLEDStreamCommand.show });
        }

        public void SetController(byte controller, IEnumerable<byte> buffer)
        {
            Write(new byte[] { (byte)FastLEDStreamCommand.set_controller, controller });
            Write(buffer);
        }
    }
}