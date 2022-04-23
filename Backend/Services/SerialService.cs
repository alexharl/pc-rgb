using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var portName = Environment.GetEnvironmentVariable("PCRGB__ComPortName");
            var portBaud = Environment.GetEnvironmentVariable("PCRGB__ComPortBaudrate");

            if (string.IsNullOrWhiteSpace(portName))
            {
                Console.WriteLine("[SerialService] Port not specified");
            }
            else
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(portBaud)) portBaud = "19200";

                    port = new SerialPort(portName, Int32.Parse(portBaud), Parity.None, 8, StopBits.One);
                    port.DataReceived += new SerialDataReceivedEventHandler(handlePortDataReceived);
                    port.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[SerialService] {e.Message}");
                }
            }

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
            port?.Write(buffer.ToArray(), 0, buffer.Count());
        }
    }
}