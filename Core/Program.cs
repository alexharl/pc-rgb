using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using core.Model.Graphics;
using core.Model.Layers;
using core.Model.Extensions;
using core.Model.Cofig;
using core.Model.Serial;
using core.Effects;


namespace core
{
    class Program
    {
        static Renderer _renderer { get; set; }
        static FastLEDStream _stream { get; set; }

        static void Main(string[] args)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            Console.WriteLine("Startup");

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
                    if (string.IsNullOrWhiteSpace(portBaud)) portBaud = "115200";

                    _stream = new FastLEDStream(portName, Int32.Parse(portBaud));
                    _stream.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[FastLEDStream] Failed to connect to port {e.Message}");
                }
            }


            var thread = new Thread(async () =>
            {
                List<LEDLayer> components = new List<LEDLayer>();

                var path = Environment.GetEnvironmentVariable("PCRGB__ControllerConfig");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    RendererConfig config = RendererConfig.Load(path);
                    if (config.Controllers?.Count() > 0)
                    {
                        components.AddRange(config.Controllers.Select(c => LEDLayer.FromConfig(c)).ToArray());
                    }

                    _renderer = Renderer.FromConfig(config, (layer) =>
                    {
                        foreach (var component in components)
                        {
                            var pixelBuffer = new List<byte>().AddPixels(component.PixelsFrom(layer));
                            _stream.SetController(component.HardwareId, pixelBuffer);
                        }

                        _stream.Show();
                    });

                    var movingRainbowEffect = new MovingRainbowEffect(0, 0, _renderer.Rect.Size.Width, _renderer.Rect.Size.Height);
                    movingRainbowEffect.Activate();
                    _renderer.Layers.Add(movingRainbowEffect);

                    await _renderer.Animate();
                }

            });
            thread.Start();

            manualResetEvent.WaitOne();
        }

    }
}
