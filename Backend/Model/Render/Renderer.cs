using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PcRGB.Model.Cofig;
using PcRGB.Model.Control;
using PcRGB.Model.Extensions;

namespace PcRGB.Model.Render
{
    public delegate void OnRenderedDelegate(Layer outputLayer);

    public class Renderer : Layer
    {
        private SerialPort port;

        public List<Controller> Components = new List<Controller>();

        public int FrameTime { get; set; } = 20;
        public bool Animating
        {
            get
            {
                return RenderCancellationTokenSource != null && RenderCancallationToken != null && !RenderCancallationToken.IsCancellationRequested;
            }
            set
            {
                if (!value)
                {
                    RenderCancellationTokenSource.Cancel();
                }
                else
                {
                    RenderCancellationTokenSource = new CancellationTokenSource();
                    RenderCancallationToken = RenderCancellationTokenSource.Token;
                }
            }
        }

        private CancellationToken RenderCancallationToken;
        private CancellationTokenSource RenderCancellationTokenSource;

        private OnRenderedDelegate OnRendered;

        public Renderer(string name, int width, int height, OnRenderedDelegate onRendered) : base(name, 0, 0, width, height)
        {
            OnRendered = onRendered;
        }

        public static Renderer FromConfig(RendererConfig config, OnRenderedDelegate onRendered)
        {
            if (config == null) return null;

            var renderer = new Renderer(config.Name, config.Width, config.Height, onRendered);

            if (config.Controllers?.Count() > 0)
            {
                renderer.Components.AddRange(config.Controllers.Select(c => Controller.FromConfig(c)).ToArray());
            }

            return renderer;
        }

        public bool SerialOpen
        {
            get { return port != null && port.IsOpen; }
        }

        public bool SerialConnect(string portName, int portBaud = 115200)
        {
            if (SerialOpen) return true;

            port = new SerialPort(portName, portBaud, Parity.None, 8, StopBits.One);
            port.Open();
            return SerialOpen;
        }

        public bool SerialDisconnect()
        {
            port.Close();
            return SerialOpen;
        }

        public void SerialWrite(IEnumerable<byte> buffer)
        {
            if (SerialOpen)
            {
                port.Write(buffer.ToArray(), 0, buffer.Count());
            }
        }

        public void Next()
        {
            Clear();
            Update();

            var layer = Render();

            // send "SET_CONTROLLER" command with pixel values for each component
            SerialWrite(Components.BufferFrom(layer));

            // send "SHOW" command to display new data
            SerialWrite(ControllerCommand.Show().Buffer);

            OnRendered(layer);
        }

        public async Task Animate()
        {
            if (Animating)
            {
                // is running
                Animating = false;
                return;
            }

            // start
            Animating = true;

            var sw = new Stopwatch();
            while (Animating)
            {
                sw.Restart();
                Next();

                var nextFrameIn = FrameTime - sw.ElapsedMilliseconds;
                if (nextFrameIn <= 0) nextFrameIn = 1;

                await Task.Delay((int)nextFrameIn);
            }
        }
    }
}