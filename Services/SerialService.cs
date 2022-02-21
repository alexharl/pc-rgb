using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
namespace PcRGB.Services
{
    public class SerialService : BackgroundService
    {
        private SerialPort port;
        List<byte> bBuffer = new List<byte>();
        public SerialService() { }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[SerialService] ExecuteAsync");
            port = new SerialPort("COM3", 19200, Parity.None, 8, StopBits.One);
            port.DataReceived += new SerialDataReceivedEventHandler(handlePortDataReceived);
            port.Open();
            return Task.CompletedTask;
        }

        private void handlePortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Buffer and process binary data
            while (port.BytesToRead > 0)
            {
                var b = port.ReadByte();
                bBuffer.Add((byte)b);
            }
            ProcessBuffer(bBuffer);
        }

        private void ProcessBuffer(List<byte> bBuffer)
        {
        }

        public void Write(IEnumerable<byte> buffer)
        {
            port.Write(buffer.ToArray(), 0, buffer.Count());
        }
    }
}